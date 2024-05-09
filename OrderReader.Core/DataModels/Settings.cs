using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OrderReader.Core.DataModels;

/// <summary>
/// A class that will store and manage settings of the application
/// </summary>
public static class Settings
{
    #region Public Properties

    /// <summary>
    /// The base directory of the application. This is where the exe file is located
    /// </summary>
    public static string ApplicationPath { get; } = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// The path where all application data should be stored. This is because the "ApplicationPath" above is version specific
    /// </summary>
    public static string AppDataPath { get; } = Path.GetFullPath(Path.Combine(ApplicationPath, @"..\", @"AppData"));

    /// <summary>
    /// The path where we can store temporary files
    /// </summary>
    public static string TempFilesPath { get; } = Path.GetFullPath(Path.Combine(AppDataPath, @"TempFiles"));

    /// <summary>
    /// The path where the application setting/config files should be stored
    /// </summary>
    public static string SettingsPath { get; } = Path.GetFullPath(Path.Combine(AppDataPath, @"Settings"));

    /// <summary>
    /// The full path of the settings file
    /// </summary>
    public static string SettingsFile { get; } = Path.GetFullPath(Path.Combine(SettingsPath, @"settings.xml"));

    /// <summary>
    /// The full path of the backup config file
    /// </summary>
    public static string ConfigFile { get; } = Path.GetFullPath(Path.Combine(SettingsPath, @"backup.Config"));

    /// <summary>
    /// The default export location for CSV files. This will be created and used if the user doesn't specify a custom location
    /// </summary>
    public static string DefaultExportPath { get; } = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"FileExport"));

    #endregion

    #region Public Helpers

    /// <summary>
    /// Create the required directories
    /// </summary>
    public static void Initialize()
    {
        if (!Directory.Exists(AppDataPath)) Directory.CreateDirectory(AppDataPath);
        if (!Directory.Exists(TempFilesPath)) Directory.CreateDirectory(TempFilesPath);
        if (!Directory.Exists(SettingsPath)) Directory.CreateDirectory(SettingsPath);
    }

    /// <summary>
    /// Load user settings from file
    /// </summary>
    /// <returns><see cref="UserSettings"/> object</returns>
    public static UserSettings LoadSettings()
    {
        if (!File.Exists(SettingsFile)) return new UserSettings();
        
        var deserializer = new XmlSerializer(typeof(UserSettings));

        using TextReader reader = new StreamReader(SettingsFile);
        var obj = deserializer.Deserialize(reader);
        var settingsObject = obj as UserSettings;
        return settingsObject ?? new UserSettings();
    }

    /// <summary>
    /// Saves user settings to a file
    /// </summary>
    public static void SaveSettings(UserSettings settings)
    {
        var serializer = new XmlSerializer(typeof(UserSettings));

        using TextWriter writer = new StreamWriter(SettingsFile);
        serializer.Serialize(writer, settings);
    }

    /// <summary>
    /// Load configuration from file
    /// </summary>
    /// <returns><see cref="AppConfiguration"/> object</returns>
    public static AppConfiguration LoadConfigs(string filePath = "")
    {
        if (!File.Exists(filePath)) return new AppConfiguration();
        
        // Try to load the backup config
        try
        {
            // Try loading the config in new format first
            var deserializer = new XmlSerializer(typeof(AppConfiguration));

            using TextReader reader = new StreamReader(filePath);
            var obj = deserializer.Deserialize(reader);
            if (obj is AppConfiguration configObject) return configObject;
            
            // If that didn't work, attempt to extract the information from the old file format instead
            // Load the XML file
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Select the connection string node
            var connectionStringNode = xmlDoc.SelectSingleNode("/configuration/connectionStrings/add[@name='default']");

            if (connectionStringNode != null)
            {
                // Extract the connection string value
                var connectionString = connectionStringNode.Attributes?["connectionString"]?.Value;
                var providerName = connectionStringNode.Attributes?["providerName"]?.Value;

                if (connectionString is not null && providerName is not null)
                {
                    // Create a new AppConfiguration object and initialize it with the extracted data
                    var appConfig = new AppConfiguration
                    {
                        DataBaseConnectionString = connectionString,
                        DataBaseProviderName = providerName
                    };

                    return appConfig;
                }
            }
        }
        catch
        {
            // Don't throw an exception here.
            // Return default value instead.
        }

        return new AppConfiguration();
    }
    
    /// <summary>
    /// Used to persist configuration settings, like connection strings, across updates.
    /// </summary>
    public static bool BackupConfigs()
    {
        try
        {
            var con = ConfigurationManager.ConnectionStrings["default"];
            
            if (con == null) return false;
            AppConfiguration appConfig = new AppConfiguration
            {
                DataBaseConnectionString = con.ConnectionString,
                DataBaseProviderName = con.ProviderName
            };

            XmlSerializer serializer = new XmlSerializer(typeof(AppConfiguration));

            using TextWriter writer = new StreamWriter(ConfigFile);
            serializer.Serialize(writer, appConfig);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}