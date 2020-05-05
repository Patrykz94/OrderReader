namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for a login screen
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Public Properties

        public string CSVExportDir { get; set; }

        #endregion

        #region Commands



        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public SettingsViewModel()
        {

            CSVExportDir = "Test";

        }

        #endregion
    }
}
