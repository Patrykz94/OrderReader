using System.IO;
using System.Windows.Input;

namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for a settings page
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// An instance of the user settings class which holds settings that user can change
        /// </summary>
        public UserSettings UserSettings { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Command for loading setting
        /// </summary>
        public ICommand LoadSettingsCommand { get; set; }

        /// <summary>
        /// Command for saving settings
        /// </summary>
        public ICommand SaveSettingsCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// A default constructor
        /// </summary>
        public SettingsViewModel()
        {
            // Load the user settings initially
            UserSettings = Settings.LoadSettings();

            // Command definitions
            LoadSettingsCommand = new RelayCommand(() => UserSettings = Settings.LoadSettings());
            SaveSettingsCommand = new RelayCommand(() => SaveSettings());

        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// A function that validates and saves the user settings
        /// </summary>
        private void SaveSettings()
        {
            if (UserSettings.UserExportPath == "") UserSettings.UserExportPath = Settings.DefaultExportPath;
            if (!Directory.Exists(UserSettings.UserExportPath)) Directory.CreateDirectory(UserSettings.UserExportPath);
            
            Settings.SaveSettings(UserSettings);
            UserSettings = Settings.LoadSettings();
        }

        #endregion
    }
}
