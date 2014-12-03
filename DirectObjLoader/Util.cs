#region Namespaces
using System;
using System.Windows.Forms;
#endregion // Namespaces

namespace DirectObjLoader
{
  class Util
  {
    /// <summary>
    /// Select a specified file in the given folder.
    /// </summary>
    /// <param name="folder">Initial folder.</param>
    /// <param name="filename">Selected filename on 
    /// success.</param>
    /// <returns>Return true if a file was successfully 
    /// selected.</returns>
    static bool FileSelect(
      string folder,
      string title,
      string filter,
      ref string filename )
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Title = title;
      dlg.CheckFileExists = true;
      dlg.CheckPathExists = true;
      dlg.InitialDirectory = folder;
      dlg.FileName = filename;
      dlg.Filter = filter;
      bool rc = ( DialogResult.OK == dlg.ShowDialog() );
      filename = dlg.FileName;
      return rc;
    }

    /// <summary>
    /// Select a WaveFront OBJ file in the given folder.
    /// </summary>
    /// <param name="folder">Initial folder.</param>
    /// <param name="filename">Selected filename on 
    /// success.</param>
    /// <returns>Return true if a file was successfully 
    /// selected.</returns>
    static public bool FileSelectObj(
      string folder,
      ref string filename )
    {
      return FileSelect( folder,
        "Select WaveFront OBJ file or Cancel to Exit",
        "WaveFront OBJ Files (*.obj)|*.obj",
        ref filename );
    }
  }
}
