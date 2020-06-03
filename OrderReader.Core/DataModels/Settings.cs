using System;
using System.IO;
using System.Xml.Serialization;

namespace OrderReader.Core
{
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
        /// The path where the application setting/config files should be stored
        /// </summary>
        public static string SettingsPath { get; } = Path.GetFullPath(Path.Combine(AppDataPath, @"Settings"));

        /// <summary>
        /// The full path of the settings file
        /// </summary>
        public static string SettingsFile { get; } = Path.GetFullPath(Path.Combine(SettingsPath, @"settings.xml"));

        /// <summary>
        /// The default export location for CSV files. This will be created and used if the user doesn't specify a custom location
        /// </summary>
        public static string DefaultExportPath { get; } = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"CSVExport"));

        #endregion

        #region Public Helpers

        /// <summary>
        /// Load user settings from file
        /// </summary>
        /// <returns><see cref="UserSettings"/> object</returns>
        public static UserSettings LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(UserSettings));

                using (TextReader reader = new StreamReader(SettingsFile))
                {
                    object obj = deserializer.Deserialize(reader);
                    return (UserSettings)obj;
                }
            }

            return new UserSettings();
        }

        /// <summary>
        /// Saves user settings to a file
        /// </summary>
        public static void SaveSettings(UserSettings settings)
        {
            if (!Directory.Exists(SettingsPath)) Directory.CreateDirectory(SettingsPath);

            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));

            using (TextWriter writer = new StreamWriter(SettingsFile))
            {
                serializer.Serialize(writer, settings);
            }
        }

        #endregion
    }
}
