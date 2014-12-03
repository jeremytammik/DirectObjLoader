#region Namespaces
using System;
using System.Windows.Forms;
#endregion // Namespaces

namespace DirectObjLoader
{
  class Util
  {
    /// <summary>
    /// Extract a true or false value from the given
    /// string, accepting yes/no, Y/N, true/false, T/F
    /// and 1/0. We are extremely tolerant, i.e., any
    /// value starting with one of the characters y, n,
    /// t or f is also accepted. Return false if no 
    /// valid Boolean value can be extracted.
    /// </summary>
    public static bool GetTrueOrFalse(
      string s,
      out bool val )
    {
      val = false;

      if( s.Equals( Boolean.TrueString,
        StringComparison.OrdinalIgnoreCase ) )
      {
        val = true;
        return true;
      }
      if( s.Equals( Boolean.FalseString,
        StringComparison.OrdinalIgnoreCase ) )
      {
        return true;
      }
      if( s.Equals( "1" ) )
      {
        val = true;
        return true;
      }
      if( s.Equals( "0" ) )
      {
        return true;
      }
      s = s.ToLower();

      if( 't' == s[0] || 'y' == s[0] )
      {
        val = true;
        return true;
      }
      if( 'f' == s[0] || 'n' == s[0] )
      {
        return true;
      }
      return false;
    }

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
