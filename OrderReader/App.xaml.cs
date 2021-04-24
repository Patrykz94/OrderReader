using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using OrderReader.Core;

namespace OrderReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Custom startup so we load our IoC immediately before anything else
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Let the base aplication do what it needs
            base.OnStartup(e);

            // Start setup of the main appication
            StartApplicationSetup();

            // Create the main window
            Current.MainWindow = new MainWindow();

            // Finish setting up the application
            FinishApplicationSetup();

            // Show main window to user
            Current.MainWindow.Show();
        }

        /// <summary>
        /// First part of configuring our application
        /// </summary>
        private void StartApplicationSetup()
        {
            // Setup IoC
            IoC.SetupInitial();

            // Bind a UI Manager
            IoC.Kernel.Bind<IUIManager>().ToConstant(new UIManager());

            // Initialize the settings class which creates all required directories
            Settings.Initialize();

            // Check if there are any saved settings and if so, restore them
            RestoreSettings();
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        /// <summary>
        /// Final part of configuring our application ready for use
        /// </summary>
        private async void FinishApplicationSetup()
        {
            // Check if app is configured
            if (!SqliteDataAccess.HasConnectionString())
            {
                // Ask the user to drag and drop a file
                var result = await IoC.UI.ShowMessage(new ConfigFileBoxDialogViewModel
                {
                    Title = "App Configuration",
                    Heading = "Welcome to Order Reader!",
                    Message = "To begin using this application, please drag and drop the configuration file provided with the download into the box below.",
                    CancelButtonText = "Exit"
                });

                await UpdateConfigFile(result);
            }

            // Test connection to the database
            if (!SqliteDataAccess.TestConnection())
            {
                var result = await IoC.UI.ShowMessage(new ConfigFileBoxDialogViewModel
                {
                    Title = "Configuration Error",
                    Message = "Application could not access the database.\n\nThis could mean the database file was moved or renamed, or it could indicate a network issue.\n\nIf you have a new configuration file, please drop it into the box below.",
                    CancelButtonText = "Exit"
                });

                await UpdateConfigFile(result);

                // Test the new configs file again
                if (!SqliteDataAccess.TestConnection())
                {
                    await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        Title = "Configuration Error",
                        Message = $"Could not connect to the database using the configuration file provided.\n\nApplication will now terminate.",
                        ButtonText = "Exit"
                    });

                    Environment.Exit(0);
                }
            }

            // Finish setup
            IoC.SetupFull();
        }

        /// <summary>
        /// Update the config file if required
        /// </summary>
        /// <param name="filePath">The path to the new config file</param>
        /// <returns></returns>
        private async Task UpdateConfigFile(string filePath)
        {
            if (filePath == default)
            {
                Environment.Exit(0);
            }
            else
            {
                // Attempt to load and update the configuration file
                try
                {
                    AppConfiguration appConfig = Settings.LoadConfigs(filePath);
                    if (appConfig.HasConfigs())
                    {
                        appConfig.UpdateConfigs();
                    }
                    else
                    {
                        await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                        {
                            Title = "Configuration Error",
                            Message = "Could not read configuration data form the provided file.\n\nApplication will now terminate.",
                            ButtonText = "Exit"
                        });

                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        Title = "Configuration Error",
                        Message = $"Could not process the configuration file provided:\n\n{ex.Message}\n\nApplication will now terminate.",
                        ButtonText = "Exit"
                    });

                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Restore our settings backup if any.
        /// Used to persist settings across updates.
        /// </summary>
        private static async void RestoreSettings()
        {
            //Restore settings after application update
            string destFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            string sourceFile = Settings.ConfigFile;
            // Check if we have settings that we need to restore
            if (!File.Exists(sourceFile))
            {
                // Nothing we need to do
                return;
            }
            // Create directory as needed
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            }
            catch (Exception ex)
            {
                await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Config Restore Error",
                    Message = $"Failed to create a config directory:\n\n{ex.Message}"
                });
            }

            // Copy our backup file in place 
            try
            {
                File.Copy(sourceFile, destFile, true);
            }
            catch (Exception ex)
            {
                await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Config Restore Error",
                    Message = $"Failed to copy the backup config file:\n\n{ex.Message}"
                });
            }

            // Delete backup file
            try
            {
                File.Delete(sourceFile);
            }
            catch (Exception ex)
            {
                await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Config Restore Error",
                    Message = $"Failed to deleted the backup config file:\n\n{ex.Message}"
                });
            }

        }
    }
}
