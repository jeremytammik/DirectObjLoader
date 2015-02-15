#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace DirectObjLoader
{
  class App : IExternalApplication
  {
    /// <summary>
    /// Caption used in messages etc.
    /// </summary>
    public const string Caption = "Direct OBJ Loader";

    #region Load bitmap from embedded resources
    static string _namespace_prefix
      = typeof( App ).Namespace + ".";

    /// <summary>
    /// Load a new icon bitmap from embedded resources.
    /// For the BitmapImage, make sure you reference 
    /// WindowsBase and PresentationCore, and import 
    /// the System.Windows.Media.Imaging namespace. 
    /// </summary>
    BitmapImage NewBitmapImage(
      Assembly a,
      string imageName )
    {
      Stream s = a.GetManifestResourceStream(
         _namespace_prefix + imageName );

      BitmapImage img = new BitmapImage();

      img.BeginInit();
      img.StreamSource = s;
      img.EndInit();

      return img;
    }
    #endregion // Load bitmap from embedded resources

    void CreateRibbonPanel( 
      UIControlledApplication a )
    {
      Assembly exe = Assembly.GetExecutingAssembly();
      string path = exe.Location;

      string className = GetType().FullName.Replace(
        "App", "Command" );

      RibbonPanel p = a.CreateRibbonPanel(
        "DirectShape OBJ Loader" );

      PushButtonData d = new PushButtonData(
          "DirectObjLoader_Command",
          "DirectShape\r\nOBJ Loader",
          path, "DirectObjLoader.Command" );

      d.ToolTip = "Load a WaveFront OBJ model mesh "
        + "into a DirectShape Revit element";

      d.Image = NewBitmapImage( exe, 
        "ImgDirectObjLoader16.png" );

      d.LargeImage = NewBitmapImage( exe, 
        "ImgDirectObjLoader32.png" );

      d.LongDescription = d.ToolTip;

      d.SetContextualHelp( new ContextualHelp( 
        ContextualHelpType.Url, 
        Command.TroubleshootingUrl ) );

      p.AddItem( d );
    }

    public Result OnStartup( 
      UIControlledApplication a )
    {
      CreateRibbonPanel( a );

      return Result.Succeeded;
    }

    public Result OnShutdown( 
      UIControlledApplication a )
    {
      return Result.Succeeded;
    }
  }
}
