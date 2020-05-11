using System.IO;
using System.Windows.Input;

namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for a login screen
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Public Properties

        public UserSettings UserSettings { get; set; }

        #endregion

        #region Commands

        public ICommand LoadSettingsCommand { get; set; }
        public ICommand SaveSettingsCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public SettingsViewModel()
        {

            UserSettings = Settings.LoadSettings();

            LoadSettingsCommand = new RelayCommand(() => UserSettings = Settings.LoadSettings());
            SaveSettingsCommand = new RelayCommand(() => SaveSettings());

        }

        #endregion

        #region Private Helpers

        private void SaveSettings()
        {
            if (UserSettings.UserExportPath == "") UserSettings.UserExportPath = Settings.DefaultExportPath;

            if (Directory.Exists(UserSettings.UserExportPath))
            {
                Settings.SaveSettings(UserSettings);
                UserSettings = Settings.LoadSettings();
            }

            // TODO: Let user know if something went wrong
        }

        #endregion
    }
}
