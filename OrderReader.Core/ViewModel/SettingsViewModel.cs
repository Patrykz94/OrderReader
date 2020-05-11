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

        public ICommand LoadSettings { get; set; }
        public ICommand SaveSettings { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public SettingsViewModel()
        {

            UserSettings = Settings.LoadSettings();

            LoadSettings = new RelayCommand(() => UserSettings = Settings.LoadSettings());
            SaveSettings = new RelayCommand(() => Settings.SaveSettings(UserSettings));

        }

        #endregion
    }
}
