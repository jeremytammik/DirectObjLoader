#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;
#endregion

namespace DirectObjLoader
{
  class App : IExternalApplication
  {
    /// <summary>
    /// Add buttons for our command
    /// to the ribbon panel.
    /// </summary>
    void PopulatePanel( RibbonPanel p )
    {
      string path = Assembly.GetExecutingAssembly()
        .Location;

      RibbonItemData i1 = new PushButtonData(
          "DirectObjLoader_Command",
          "DirectShape\r\nOBJ Loader",
          path, "DirectObjLoader.Command" );

      i1.ToolTip = "Load a WaveFront OBJ file into a "
        + "DirectShape element";

      //p.AddStackedItems( i1, i2, i3 );

      p.AddItem( i1 );
    }

    public Result OnStartup( UIControlledApplication a )
    {
      PopulatePanel(
        a.CreateRibbonPanel(
          "DirectShape OBJ Loader" ) );

      return Result.Succeeded;
    }

    public Result OnShutdown( UIControlledApplication a )
    {
      return Result.Succeeded;
    }
  }
}
