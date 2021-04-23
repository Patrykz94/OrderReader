namespace OrderReader.Core
{
    /// <summary>
    /// Details for a config file box dialog
    /// </summary>
    public class ConfigFileBoxDialogDesignModel : ConfigFileBoxDialogViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static ConfigFileBoxDialogDesignModel Instance => new ConfigFileBoxDialogDesignModel();

        #endregion

        #region Constructor

        public ConfigFileBoxDialogDesignModel()
        {
            Heading = "Welcome!";
            Message = "Please drag and drop your company's config file into\nthe box below in order to configure the application.";
            OKButtonText = "OK";
            CancelButtonText = "Cancel";
        }

        #endregion
    }
}
