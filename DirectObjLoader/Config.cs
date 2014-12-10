#region Namespaces
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
#endregion // Namespaces

namespace DirectObjLoader
{
  class Config
  {
    const string _defaultFolderObj = "defaultFolderObj";
    const string _inputScaleFactor = "inputScaleFactor";
    const string _tryToCreateSolids = "tryToCreateSolids";

    static Configuration _config = null;

    static Configuration GetConfig()
    {
      string path = Assembly.GetExecutingAssembly().Location;
      string configPath = path + ".config";

      Configuration config =
        ConfigurationManager.OpenMappedExeConfiguration(
          new ExeConfigurationFileMap { ExeConfigFilename = configPath },
          ConfigurationUserLevel.None );

      string[] keys = config.AppSettings.Settings.AllKeys;

      if( !keys.Contains<string>( _defaultFolderObj ) )
      {
        config.AppSettings.Settings.Add(
          _defaultFolderObj, Path.GetTempPath() );
      }
      if( !keys.Contains<string>( _inputScaleFactor ) )
      {
        config.AppSettings.Settings.Add(
          _inputScaleFactor, "1.0" );
      }
      if( !keys.Contains<string>( _tryToCreateSolids ) )
      {
        config.AppSettings.Settings.Add(
          _tryToCreateSolids, "true" );
      }
      return config;
    }

    static KeyValueConfigurationCollection Settings
    {
      get
      {
        if( null == _config )
        {
          _config = GetConfig();
        }
        return _config.AppSettings.Settings;
      }
    }

    public static string DefaultFolderObj
    {
      get
      {
        return Settings[_defaultFolderObj].Value;
      }
      set
      {
        string oldVal = DefaultFolderObj;
        if( !value.Equals( oldVal ) )
        {
          Settings[_defaultFolderObj].Value = value;
          _config.Save( ConfigurationSaveMode.Modified );
        }
      }
    }

    public static double InputScaleFactor
    {
      get
      {
        double f;
        try
        {
          f = double.Parse( Settings[_inputScaleFactor].Value );
        }
        catch( System.FormatException )
        {
          f = 1.0;
        }
        return f;
      }
      set
      {
        string oldVal = DefaultFolderObj;
        if( !value.Equals( oldVal ) )
        {
          Settings[_inputScaleFactor].Value = value.ToString();
          _config.Save( ConfigurationSaveMode.Modified );
        }
      }
    }

    static bool TryToCreateSolids
    {
      get
      {
        bool rc;

        Util.GetTrueOrFalse(
          Settings[_tryToCreateSolids].Value,
          out rc );

        return rc;
      }
      set
      {
        bool oldVal;

        Util.GetTrueOrFalse(
          Settings[_tryToCreateSolids].Value,
          out oldVal );

        if( !value.Equals( oldVal ) )
        {
          Settings[_tryToCreateSolids].Value = value
            ? Boolean.TrueString.ToLower()
            : Boolean.FalseString.ToLower();

          _config.Save( ConfigurationSaveMode.Modified );
        }
      }
    }
  }
}
