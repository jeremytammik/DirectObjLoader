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
using Autodesk.Revit.UI.Selection;
using Autodesk.Windows;
using FileFormatWavefront;
using FileFormatWavefront.Model;
using Face = FileFormatWavefront.Model.Face;
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
    /// Define initial OBJ file folder.
    /// </summary>
    static string _obj_folder_name
      = Path.GetTempPath();

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      IWin32Window revit_window
        = new JtWindowHandle(
          ComponentManager.ApplicationWindow );

      if( !Util.FileSelectObj( _obj_folder_name,
        ref _filename ) )
      {
        return Result.Cancelled;
      }

      _obj_folder_name = Path.GetDirectoryName(
        _filename );

      bool loadTextureImages = true;

      var result = FileFormatObj.Load( _filename, loadTextureImages );

      foreach( var m in result.Messages )
      {
        Debug.Print( "{0}: {1}", m.MessageType, m.Details );
        Debug.Print( "{0}: {1}", m.FileName, m.LineNumber );
      }

      // Convert OBJ vertices to Revit XYZ.
      // OBJ assumes X to the right, Y up and Z out of the screen.
      // Revit 3D view assumes X right, Y away from the screen and Z up.

      int n = result.Model.Vertices.Count;

      List<XYZ> vertices = new List<XYZ>( n );

      foreach( Vertex v in result.Model.Vertices )
      {
        Debug.Print( "v {0} {1} {2}", v.x, v.y, v.z );
        vertices.Add( new XYZ( v.x, -v.z, v.y ) );
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

      using( Transaction tx = new Transaction( doc ) )
      {
        tx.Start( "Create DirectShape from OBJ" );

        TessellatedShapeBuilder builder = new TessellatedShapeBuilder();

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
        }

        builder.CloseConnectedFaceSet();

        TessellatedShapeBuilderResult r
          = builder.Build(
            TessellatedShapeBuilderTarget.Solid,
            TessellatedShapeBuilderFallback.Abort,
            graphicsStyleId );

        ElementId categoryId = new ElementId(
          BuiltInCategory.OST_GenericModel );

        DirectShape ds = DirectShape.CreateElement(
          doc, categoryId, "A", "B" );

        ds.SetShape( r.GetGeometricalObjects() );

        ds.Name = "Test";
        tx.Commit();
      }
      return Result.Succeeded;
    }
  }
}
