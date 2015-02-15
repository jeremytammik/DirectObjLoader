#region Namespaces
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
#endregion // Namespaces

namespace DirectObjLoader
{
  class Util
  {
    /// <summary>
    /// Return an English plural suffix 's' or
    /// nothing for the given number of items.
    /// </summary>
    public static string PluralSuffix( int n )
    {
      return 1 == n ? "" : "s";
    }

    // JavaScript sample implementations:
    // capitalize:function(){return this.replace(/\b[a-z]/g,function(match){return match.toUpperCase();});}
    // capitalize: function() { return this.charAt(0).toUpperCase() + this.substring(1).toLowerCase(); }

    /// <summary>
    /// Ensure that each space delimited word in the 
    /// given string has an upper case first character.
    /// </summary>
    public static string Capitalize( string s )
    {
      return string.Join( " ", s.Split( null )
        .Select<string, string>( a
          => a.Substring( 0, 1 ).ToUpper() 
            + a.Substring( 1 ) ) );
    }

    public static string RealString( double a )
    {
      return a.ToString( "0.##" );
    }

    public static string PointString( XYZ p )
    {
      return string.Format( "({0},{1},{2})",
        RealString( p.X ), RealString( p.Y ),
        RealString( p.Z ) );
    }

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

    /// <summary>
    /// Return the size in bytes of the given file.
    /// </summary>
    static public long GetFileSize( string filename )
    {
      long fileSize = 0L;

      using( FileStream file = File.Open(
        filename, FileMode.Open ) )
      {
        fileSize = file.Seek( 0L, SeekOrigin.End );
        file.Close();
      }
      return fileSize;
    }
  }
}
