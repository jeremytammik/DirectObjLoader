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

      TessellatedShapeBuilder builder 
        = new TessellatedShapeBuilder();

      List<XYZ> corners = new List<XYZ>( 4 );

      builder.OpenConnectedFaceSet( false );

      foreach( Face f in faces )
      {
        //Debug.Assert( 4 >= f.Indices.Count,
        //  "I support only three or four vertices per face" );

        if( corners.Capacity < f.Indices.Count )
        {
          corners = new List<XYZ>( f.Indices.Count );
        }

        corners.Clear();

        foreach( Index i in f.Indices )
        {
          Debug.Assert( vertices.Count > i.vertex,
            "how can the face vertex index be larger than the total number of vertices?" );

          corners.Add( vertices[i.vertex] );
        }

        builder.AddFace( new TessellatedFace( corners,
          ElementId.InvalidElementId ) );

        ++nFaces;
      }
      builder.CloseConnectedFaceSet();

      // Refer to StlImport sample for more clever 
      // handling of target and fallback.

      TessellatedShapeBuilderResult r
        = builder.Build(
          TessellatedShapeBuilderTarget.Mesh, // Solid
          TessellatedShapeBuilderFallback.Salvage, // Abort
          graphicsStyleId );

      DirectShape ds = DirectShape.CreateElement(
        doc, _categoryId, appGuid, shapeName );

      ds.SetShape( r.GetGeometricalObjects() );
      ds.Name = shapeName;

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

      bool loadTextureImages = true;

      var result = FileFormatObj.Load(
        _filename, loadTextureImages );

      foreach( var m in result.Messages )
      {
        Debug.Print( "{0}: {1} line {2} in {3}",
          m.MessageType, m.Details,
          m.FileName, m.LineNumber );
      }

      // Convert OBJ vertices to Revit XYZ.
      // OBJ assumes X to the right, Y up and Z out of the screen.
      // Revit 3D view assumes X right, Y away from the screen and Z up.

      double scale = Config.InputScaleFactor;

      int n = result.Model.Vertices.Count;

      List<XYZ> vertices = new List<XYZ>( n );
      XYZ w;

      foreach( Vertex v in result.Model.Vertices )
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

      foreach( Face f in result.Model.UngroupedFaces )
      {
        n = f.Indices.Count;

        Debug.Assert( 3 == n || 4 == n,
          "expected triagles or quadrilaterals" );

        Debug.Print( string.Join( ", ",
          f.Indices.ToString() ) );
      }

      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Document doc = uidoc.Document;

      string appGuid 
        = uiapp.ActiveAddInId.GetGUID().ToString();

      string shapeName = Util.Capitalize(
        Path.GetFileNameWithoutExtension( _filename )
          .Replace( '_', ' ' ) );

      // Find GraphicsStyle

      FilteredElementCollector collector
        = new FilteredElementCollector( doc )
          .OfClass( typeof( GraphicsStyle ) );

      GraphicsStyle style = collector.Cast<GraphicsStyle>()
        .FirstOrDefault<GraphicsStyle>( gs => gs.Name.Equals( "<Sketch>" ) );

      ElementId graphicsStyleId = null;

      if( style != null )
      {
        graphicsStyleId = style.Id;
      }

      Result rc = Result.Failed;

      using( Transaction tx = new Transaction( doc ) )
      {
        tx.Start( "Create DirectShape from OBJ" );

        int nFaces = 0;

        if( 0 < result.Model.UngroupedFaces.Count )
        {
          nFaces += NewDirectShape( vertices, 
            result.Model.UngroupedFaces, doc, 
            graphicsStyleId, appGuid, shapeName );
        }

        foreach( Group g in result.Model.Groups )
        {
          string s = string.Join( ".", g.Names );

          if( 0 < s.Length ) { s = "." + s; }

          nFaces += NewDirectShape( vertices, g.Faces,
            doc, graphicsStyleId, appGuid, shapeName 
            + s );
        }

        if( 0 == nFaces )
        {
          message = "Zero faces";
        }
        else
        {
          tx.Commit();

          rc = Result.Succeeded;
        }
      }
      return rc;
    }
  }
}
