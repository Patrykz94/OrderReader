using System;
using System.Runtime.Serialization;

namespace OrderReader.Core
{
    /// <summary>
    /// A class that will store the user specific settings of the application
    /// </summary>
    [Serializable]
    public class UserSettings : ISerializable
    {
        #region Public Properties

        /// <summary>
        /// The export location for CSV files that the user selects. It will override the default export location
        /// </summary>
        public string UserExportPath { get; set; }

        /// <summary>
        /// Whether or not the CSV files should be exported
        /// </summary>
        public bool ExportCSV { get; set; }

        /// <summary>
        /// Whether or not the processed orders should be printed
        /// </summary>
        public bool PrintOrders { get; set; }

        #endregion

        #region Constructor

        public UserSettings()
        {
            UserExportPath = Settings.DefaultExportPath;
            ExportCSV = true;
            PrintOrders = true;
        }

        /// <summary>
        /// A constructor that deserializes this object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public UserSettings(SerializationInfo info, StreamingContext context)
        {
            UserExportPath = (string)info.GetValue("UserExportPath", typeof(string));
            ExportCSV = (bool)info.GetValue("ExportCSV", typeof(bool));
            PrintOrders = (bool)info.GetValue("PrintOrders", typeof(bool));
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
            info.AddValue("UserExportPath", UserExportPath);
            info.AddValue("ExportCSV", ExportCSV);
            info.AddValue("PrintOrders", PrintOrders);
        }

        #endregion
    }
}
