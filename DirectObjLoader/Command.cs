#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using FileFormatWavefront;
using FileFormatWavefront.Model;
using Face = FileFormatWavefront.Model.Face;
using Group = FileFormatWavefront.Model.Group;
using FaceCollection = System.Collections.ObjectModel.ReadOnlyCollection<FileFormatWavefront.Model.Face>;
//using RvtFace = Autodesk.Revit.DB.Face;
#endregion

namespace DirectObjLoader
{
  [Transaction( TransactionMode.Manual )]
  public class Command : IExternalCommand
  {
    /// <summary>
    /// Remember last selected filename for the 
    /// duration of the current session.
    /// </summary>
    static string _filename = string.Empty;

    /// <summary>
    /// Category to use for the DirectShape elements
    /// generated.
    /// </summary>
    static ElementId _categoryId = new ElementId(
      BuiltInCategory.OST_GenericModel );

    /// <summary>
    /// Create a new DirectShape element from given
    /// list of faces and return the number of faces
    /// processed.
    /// Return -1 if a face vertex index exceeds the
    /// total number of available vertices, 
    /// representing a fatal error.
    /// </summary>
    static int NewDirectShape(
      List<XYZ> vertices,
      FaceCollection faces,
      Document doc,
      ElementId graphicsStyleId,
      string appGuid,
      string shapeName )
    {
      int nFaces = 0;
      int nFacesFailed = 0;

      TessellatedShapeBuilder builder
        = new TessellatedShapeBuilder();

      builder.LogString = shapeName;

      List<XYZ> corners = new List<XYZ>( 4 );

      builder.OpenConnectedFaceSet( false );

      foreach( Face f in faces )
      {
        builder.LogInteger = nFaces;

        if( corners.Capacity < f.Indices.Count )
        {
          corners = new List<XYZ>( f.Indices.Count );
        }

        corners.Clear();

        foreach( Index i in f.Indices )
        {
          Debug.Assert( vertices.Count > i.vertex,
            "how can the face vertex index be larger than the total number of vertices?" );

          if( i.vertex >= vertices.Count )
          {
            return -1;
          }

          corners.Add( vertices[i.vertex] );
        }


        try
        {
          builder.AddFace( new TessellatedFace( corners,
            ElementId.InvalidElementId ) );

          ++nFaces;
        }
        catch( Autodesk.Revit.Exceptions.ArgumentException ex )
        {
          // Remember something went wrong here.

          ++nFacesFailed;

          Debug.Print( 
            "Revit API argument exception {0}\r\n"
            + "Failed to add face with {1} corners: {2}",
            ex.Message, corners.Count, 
            string.Join( ", ", 
              corners.Select<XYZ, string>( 
                p => Util.PointString( p ) ) ) );
        }
      }
      builder.CloseConnectedFaceSet();

      // Refer to StlImport sample for more clever 
      // handling of target and fallback and the 
      // possible combinations.

      TessellatedShapeBuilderResult r
        = builder.Build(
          TessellatedShapeBuilderTarget.AnyGeometry,
          TessellatedShapeBuilderFallback.Mesh,
          graphicsStyleId );

      DirectShape ds = DirectShape.CreateElement(
        doc, _categoryId, appGuid, shapeName );

      ds.SetShape( r.GetGeometricalObjects() );
      ds.Name = shapeName;

      Debug.Print(
        "Shape '{0}': added {1} face{2}, {3} face{4} failed.",
        shapeName, nFaces, Util.PluralSuffix( nFaces ),
        nFacesFailed, Util.PluralSuffix( nFacesFailed ) );

      return nFaces;
    }

    /// <summary>
    /// External command mainline.
    /// </summary>
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      IWin32Window revit_window
        = new JtWindowHandle(
          ComponentManager.ApplicationWindow );

      if( !Util.FileSelectObj(
        Config.DefaultFolderObj,
        ref _filename ) )
      {
        return Result.Cancelled;
      }

      Config.DefaultFolderObj
        = Path.GetDirectoryName( _filename );

      FileLoadResult<Scene> obj_load_result = null;
      List<XYZ> vertices = null;

      try
      {
        bool loadTextureImages = true;

        obj_load_result = FileFormatObj.Load(
          _filename, loadTextureImages );

        foreach( var m in obj_load_result.Messages )
        {
          Debug.Print( "{0}: {1} line {2} in {3}",
            m.MessageType, m.Details,
            m.FileName, m.LineNumber );
        }

        // Convert OBJ vertices to Revit XYZ.
        // OBJ assumes X to the right, Y up and Z out of the screen.
        // Revit 3D view assumes X right, Y away 
        // from the screen and Z up.

        double scale = Config.InputScaleFactor;

        int n = obj_load_result.Model.Vertices.Count;

        vertices = new List<XYZ>( n );
        XYZ w;

        foreach( Vertex v in obj_load_result.Model.Vertices )
        {
          w = new XYZ( v.x * scale,
            -v.z * scale, v.y * scale );

          Debug.Print( "({0},{1},{2}) --> {3}",
            Util.RealString( v.x ),
            Util.RealString( v.y ),
            Util.RealString( v.z ),
            Util.PointString( w ) );

          vertices.Add( w );
        }

        foreach( Face f in obj_load_result.Model.UngroupedFaces )
        {
          n = f.Indices.Count;

          Debug.Assert( 3 == n || 4 == n,
            "expected triangles or quadrilaterals" );

          Debug.Print( string.Join( ", ",
            f.Indices.Select<Index,string>( 
              i => i.vertex.ToString() ) ) );
        }
      }
      catch( System.Exception ex )
      {
        message = string.Format(
          "Exception reading '{0}':\r\n{1}",
          _filename, ex.Message );

        return Result.Failed;
      }

      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Document doc = uidoc.Document;

      string appGuid
        = uiapp.ActiveAddInId.GetGUID().ToString();

      string shapeName = Util.Capitalize(
        Path.GetFileNameWithoutExtension( _filename )
          .Replace( '_', ' ' ) );

      // Retrieve "<Sketch>" graphics style, 
      // if it exists.

      FilteredElementCollector collector
        = new FilteredElementCollector( doc )
          .OfClass( typeof( GraphicsStyle ) );

      GraphicsStyle style
        = collector.Cast<GraphicsStyle>()
          .FirstOrDefault<GraphicsStyle>( gs
            => gs.Name.Equals( "<Sketch>" ) );

      ElementId graphicsStyleId = null;

      if( style != null )
      {
        graphicsStyleId = style.Id;
      }

      Result rc = Result.Failed;

      try
      {
        using( Transaction tx = new Transaction( doc ) )
        {
          tx.Start( "Create DirectShape from OBJ" );

          int nFaces = 0; // set to -1 on fatal error
          int nFacesTotal = 0;

          if( 0 < obj_load_result.Model.UngroupedFaces.Count )
          {
            nFacesTotal = nFaces = NewDirectShape( vertices,
              obj_load_result.Model.UngroupedFaces, doc,
              graphicsStyleId, appGuid, shapeName );
          }

          if( -1 < nFaces )
          {
            foreach( Group g in obj_load_result.Model.Groups )
            {
              string s = string.Join( ".", g.Names );

              if( 0 < s.Length ) { s = "." + s; }

              nFaces = NewDirectShape( vertices, g.Faces,
                doc, graphicsStyleId, appGuid,
                shapeName + s );

              if( -1 == nFaces )
              {
                break;
              }

              nFacesTotal += nFaces;
            }
          }

          if( -1 == nFaces )
          {
            message = "Invalid OBJ file. Error: face "
              + "vertex index exceeds total vertex count.";
          }
          else if( 0 == nFacesTotal )
          {
            message = "Invalid OBJ file. Zero faces found.";
          }
          else
          {
            tx.Commit();

            rc = Result.Succeeded;
          }
        }
      }
      catch( System.Exception ex )
      {
        message = string.Format(
          "Exception generating DirectShape '{0}':\r\n{1}",
          shapeName, ex.Message );

        return Result.Failed;
      }
      return rc;
    }
  }
}
