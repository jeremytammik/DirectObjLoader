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

        TessellatedShapeBuilder builder = new TessellatedShapeBuilder();

        int nFaces = 0;

        builder.OpenConnectedFaceSet( false );

        List<XYZ> corners = new List<XYZ>( 4 );

        foreach( Face f in result.Model.UngroupedFaces )
        {
          corners.Clear();

          foreach( Index i in f.Indices )
          {
            corners.Add( vertices[i.vertex] );
          }

          builder.AddFace( new TessellatedFace( corners,
            ElementId.InvalidElementId ) );

          ++nFaces;
        }

        foreach( Group g in result.Model.Groups )
        {
          foreach( Face f in g.Faces )
          {
            corners.Clear();

            foreach( Index i in f.Indices )
            {
              corners.Add( vertices[i.vertex] );
            }

            builder.AddFace( new TessellatedFace( corners,
              ElementId.InvalidElementId ) );

            ++nFaces;
          }
        }

        if( 0 == nFaces )
        {
          message = "Zero faces";
        }
        else
        {
          builder.CloseConnectedFaceSet();

          // Refer to StlImport sample for more clever 
          // handling of target and fallback.

          TessellatedShapeBuilderResult r
            = builder.Build(
              //TessellatedShapeBuilderTarget.Solid,
              TessellatedShapeBuilderTarget.Mesh,
              //TessellatedShapeBuilderFallback.Abort,
              TessellatedShapeBuilderFallback.Salvage,
              graphicsStyleId );

          ElementId categoryId = new ElementId(
            BuiltInCategory.OST_GenericModel );

          DirectShape ds = DirectShape.CreateElement(
            doc, categoryId, "A", "B" );

          ds.SetShape( r.GetGeometricalObjects() );

          ds.Name = "Test";
          tx.Commit();

          rc = Result.Succeeded;
        }
      }
      return rc;
    }
  }
}
