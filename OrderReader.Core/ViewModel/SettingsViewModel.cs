using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for a settings page
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// Field that stores the current printer index
        /// </summary>
        private int _printerIndex = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// An instance of the user settings class which holds settings that user can change
        /// </summary>
        public UserSettings UserSettings { get; set; }

        /// <summary>
        /// A list of printers detected by the system
        /// </summary>
        public ObservableCollection<string> PrintersList => PrintingManager.InstalledPrinters;

        /// <summary>
        /// The currently selected index of a printer from the list of printers
        /// </summary>
        public int SelectedPrinterIndex
        {
            get => _printerIndex;
            set
            {
                _printerIndex = value;
                UserSettings.PreferredPrinterName = PrintersList[SelectedPrinterIndex];
            }
        }

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
            LoadSettings();

            // Command definitions
            LoadSettingsCommand = new RelayCommand(LoadSettings);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// A function that validates and saves the user settings
        /// </summary>
        private void SaveSettings()
        {
            if (UserSettings.UserCSVExportPath == "") UserSettings.UserCSVExportPath = Settings.DefaultExportPath;
            if (UserSettings.UserPDFExportPath == "") UserSettings.UserPDFExportPath = Settings.DefaultExportPath;
            if (!Directory.Exists(UserSettings.UserCSVExportPath)) Directory.CreateDirectory(UserSettings.UserCSVExportPath);
            if (!Directory.Exists(UserSettings.UserPDFExportPath)) Directory.CreateDirectory(UserSettings.UserPDFExportPath);
            
            Settings.SaveSettings(UserSettings);
            LoadSettings();
        }

        /// <summary>
        /// A function that loads saved settings from the file
        /// </summary>
        private void LoadSettings()
        {
            UserSettings = Settings.LoadSettings();

            if (UserSettings.PreferredPrinterName == null) UserSettings.PreferredPrinterName = PrintingManager.DefaultPrinter;

            for (int i = 0; i < PrintersList.Count; i++)
            {
                if (PrintersList[i] == UserSettings.PreferredPrinterName)
                {
                    _printerIndex = i;
                    break;
                }
            }
        }

        #endregion
    }
}
