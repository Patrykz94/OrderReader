using System;
using System.Configuration;
using System.Runtime.Serialization;

namespace OrderReader.Core
{
    /// <summary>
    /// A class that will store settings specific to the company that uses Order Reader
    /// </summary>
    [Serializable]
    public class AppConfiguration : ISerializable
    {
        #region Public Properties

        /// <summary>
        /// The connection string for database that will be used by Order Reader
        /// </summary>
        public string DataBaseConnectionString { get; set; }

        /// <summary>
        /// The provider name for our database
        /// </summary>
        public string DataBaseProviderName { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AppConfiguration()
        {
            DataBaseConnectionString = default;
            DataBaseProviderName = default;
        }

        /// <summary>
        /// A constructor that deserializes this object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AppConfiguration(SerializationInfo info, StreamingContext context)
        {
            DataBaseConnectionString = (string)info.GetValue("DataBaseConnectionString", typeof(string));
            DataBaseProviderName = (string)info.GetValue("DataBaseProviderName", typeof(string));
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Serializes this object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DataBaseConnectionString", DataBaseConnectionString);
            info.AddValue("DataBaseProviderName", DataBaseProviderName);
        }

        /// <summary>
        /// Checks if we were able to read data off of the configs file
        /// </summary>
        /// <returns></returns>
        public bool HasConfigs()
        {
            if (DataBaseConnectionString != default && DataBaseProviderName != default)
                return true;

            return false;
        }

        /// <summary>
        /// Updates the configuration file when a new file is provided
        /// </summary>
        /// <returns>Whether or not the config was successfully updated</returns>
        public bool UpdateConfigs()
        {
            try
            {
                ValidateConnectionString();

                // Open the exe configuration file
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                // Remove all existing connection strings (if any exist)
                config.ConnectionStrings.ConnectionStrings.Clear();
                // Add our new connection string
                config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("default", DataBaseConnectionString, DataBaseProviderName));
                // Save the changes we just made to the exe configuration file so that the settings persist when we reload the application
                config.Save(ConfigurationSaveMode.Modified);
                // Refresh the connection strings section of ConfigurationManager so that new settings are immediately usable without having to reload the application
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch(Exception ex)
            {
                // If an error occurs, we just want to show the error message and return false, instead of crashing the application
                IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Error",
                    Message = $"An error occured while trying to update the configuration file:\n{ex.Message}",
                    ButtonText = "OK"
                });
                return false;
            }
            return true;
        }

        #endregion

        #region Private Helpers

        private void ValidateConnectionString()
        {
            // Remove the version number
            if (DataBaseConnectionString.EndsWith("Version=3;"))
            {
                DataBaseConnectionString = DataBaseConnectionString[..^"Version=3;".Length];
            }
        }

        #endregion
    }
}
