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

      if( !keys.Contains<string>( "DefaultFolderObj" ) )
      {
        config.AppSettings.Settings.Add(
          "DefaultFolderObj", "C:/" );
      }
      if( !keys.Contains<string>( "TryToCreateSolids" ) )
      {
        config.AppSettings.Settings.Add(
          "TryToCreateSolids", "true" );
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

    static string DefaultFolderObj 
    { 
      get
      {
        return Settings["defaultFolderObj"].Value;
      } 
      set
      {
        Settings["defaultFolderObj"].Value = value;

        _config.Save( ConfigurationSaveMode.Modified );
      }
    }

    static bool TryToCreateSolids
    {
      get
      {
        bool rc;

        Util.GetTrueOrFalse( 
          Settings["tryToCreateSolids"].Value, 
          out rc );

        return rc;
      }
      set
      {
        Settings["tryToCreateSolids"].Value = value
          ? Boolean.TrueString.ToLower()
          : Boolean.FalseString.ToLower();

        _config.Save( ConfigurationSaveMode.Modified );
      }
    }
  }
}
