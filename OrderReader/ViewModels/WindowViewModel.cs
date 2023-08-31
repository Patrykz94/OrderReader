using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using OrderReader.Core;
using Squirrel;

namespace OrderReader
{
    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 0;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        #endregion

        #region Public Properties

        /// <summary>
        /// The minimum width of the window
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 1200;

        /// <summary>
        /// The minimum height of the window
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 700;

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get { return Borderless ? 0 : 1; } }

        /// <summary>
        /// The size of the resize border around the window, taking nto account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mOuterMarginSize;
            }

            set
            {
                mOuterMarginSize = value;
            }
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mWindowRadius;
            }

            set
            {
                mWindowRadius = value;
            }
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight { get; set; } = 42;

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        /// <summary>
        /// True if we should have a dimmend overlay on the window
        /// such as when the popup is visible or the window is not focused
        /// </summary>
        public bool DimmableOverlayVisible { get; set; }

        /// <summary>
        /// The current version of the application
        /// </summary>
        public string CurrentVersion { get; set; }

        /// <summary>
        /// If a new update has been installed, this field will be updated
        /// </summary>
        public string UpdatedVersion { get; set; } = default;

        #endregion

        #region Commands

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }


        /// <summary>
        /// The command to show the system menu of the window
        /// </summary>
        public ICommand CloseCommand { get; set; }


        /// <summary>
        /// The command to minimize the window
        /// </summary>
        public ICommand MenuCommand { get; set; }

        /// <summary>
        /// Go to orders panel page
        /// </summary>
        public ICommand OrdersPanelCommand { get; set; }

        /// <summary>
        /// Go to customers page
        /// </summary>
        public ICommand CustomersCommand { get; set; }

        /// <summary>
        /// Go to settings page
        /// </summary>
        public ICommand SettingsCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public WindowViewModel(Window window)
        {
            mWindow = window;

            // Listen out for the window resizing
            mWindow.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                WindowResized();
            };

            // Create commands
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));
            // Got to orders page
            OrdersPanelCommand = new RelayCommand(() => IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Orders));
            // Got to customers page
            CustomersCommand = new RelayCommand(() => IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Customers));
            // Got to settings page
            SettingsCommand = new RelayCommand(() => IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Settings));

            // Fix window resize issue
            var resizer = new WindowResizer(mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                mDockPosition = dock;

                // Fire off resize events
                WindowResized();
            };

            // Get the current version of our app
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            CurrentVersion = $" v{ versionInfo.FileVersion }";

#if !DEBUG
            // Check for updates
            CheckForUpdates();
#endif
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Get the screen position of the mouse
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            return mWindow.PointToScreen(Mouse.GetPosition(mWindow));
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
        }

        /// <summary>
        /// Function that will check for updates and update the aplication
        /// </summary>
        private async void CheckForUpdates()
        {
            // TODO: Do this in a better way
            while(!SqliteDataAccess.TestConnection())
            {
                await Task.Delay(1000);
            }

            var defaultSettings = SqliteDataAccess.LoadDefaultSettings();

            if (defaultSettings.ContainsKey("UpdateLocation"))
            {
                string updateLocation = defaultSettings["UpdateLocation"];
                if (Directory.Exists(updateLocation))
                {
                    using (var manager = new UpdateManager(updateLocation))
                    {
                        var updateInfo = await manager.CheckForUpdate();

                        if (updateInfo.ReleasesToApply.Count > 0)
                        {
                            var installedUpdates = await manager.UpdateApp();

                            await BackupSettings();

                            UpdatedVersion = installedUpdates.Version.ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Make a backup of our configs.
        /// Used to persist configuration settings, like connection strings, across updates.
        /// </summary>
        private static async Task BackupSettings()
        {
            try
            {
                var con = ConfigurationManager.ConnectionStrings["default"];
                if (con != null)
                {
                    AppConfiguration appConfig = new AppConfiguration
                    {
                        DataBaseConnectionString = con.ConnectionString,
                        DataBaseProviderName = con.ProviderName
                    };

                    XmlSerializer serializer = new XmlSerializer(typeof(AppConfiguration));

                    using (TextWriter writer = new StreamWriter(Core.Settings.ConfigFile))
                    {
                        serializer.Serialize(writer, appConfig);
                    }
                }
            }
            catch (Exception ex)
            {
                // If an error occurs, we just want to show the error message, instead of crashing the application
                await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Config Backup Error",
                    Message = $"Failed to backup the config file:\n\n{ex.Message}"
                });
            }
        }

        #endregion
    }
}
