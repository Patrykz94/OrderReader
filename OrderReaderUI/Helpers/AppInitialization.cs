using OrderReader.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReaderUI.ViewModels;
using OrderReaderUI.ViewModels.Dialogs;

namespace OrderReaderUI.Helpers;

public class AppInitialization
{
    private readonly IWindowManager _windowManager;

    public AppInitialization(IWindowManager windowManager)
    {
        _windowManager = windowManager;
    }

    public async Task Initialize()
    {
        // Initialize the settings class which creates all required directories
        Settings.Initialize();

        // Check if there are any saved settings and if so, restore them
        RestoreSettings();

        // Check if app is configured
        if (!SqliteDataAccess.HasConnectionString())
        {
            // Config File Dialog
            var configDialog = new DialogConfigFileViewModel(secondaryButtonText: "Exit");
            var configResult = await _windowManager.ShowDialogAsync(configDialog);
            
            // If user did not provide the file, exit the application
            if (configResult != true) Environment.Exit(0);

            await UpdateConfigFile(configDialog.ConfigFileLocation);
        }

        // Test connection to the database
        if (!SqliteDataAccess.TestConnection())
        {
            // Config File Dialog
            var configDialog = new DialogConfigFileViewModel("Configuration Error", secondaryButtonText: "Exit")
            {
                Message = "Application could not access the database.\n\nThis could mean the database file was moved or renamed, or it could indicate a network issue.\n\nIf you have a new configuration file, please drop it into the box below."
            };
            var configResult = await _windowManager.ShowDialogAsync(configDialog);
            
            // If user did not provide the file, exit the application
            if (configResult != true) Environment.Exit(0);

            await UpdateConfigFile(configDialog.ConfigFileLocation);

            // Test the new configs file again
            if (!SqliteDataAccess.TestConnection())
            {
                await _windowManager.ShowDialogAsync(new DialogMessageViewModel(
                    "Could not connect to the database using the configuration file provided.\n\nApplication will now terminate.",
                    "Configuration Error",
                    "Exit"));

                Environment.Exit(0);
            }
        }
    }

    /// <summary>
    /// Update the config file if required
    /// </summary>
    /// <param name="filePath">The path to the new config file</param>
    /// <param name="exitOnError">Whether the application should exit if an error is encountered</param>
    private async Task UpdateConfigFile(string filePath, bool exitOnError = true)
    {
        if (filePath == string.Empty)
        {
            if (exitOnError) Environment.Exit(0);
        }
        else
        {
            // Attempt to load and update the configuration file
            try
            {
                var appConfig = Settings.LoadConfigs(filePath);
                if (appConfig.HasConfigs())
                {
                    appConfig.UpdateConfigs();
                }
                else
                {
                    await _windowManager.ShowDialogAsync(new DialogMessageViewModel(
                            $"Could not read configuration data form the provided file.{(exitOnError ? "\n\nApplication will now terminate." : "")}",
                            "Configuration Error",
                            "Exit"));

                    if (exitOnError) Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                await _windowManager.ShowDialogAsync(new DialogMessageViewModel(
                        $"Could not process the configuration file provided:\n\n{ex.Message}{(exitOnError ? "\n\nApplication will now terminate." : "")}",
                        "Configuration Error",
                        "Exit"));

                if (exitOnError) Environment.Exit(0);
            }
        }
    }

    /// <summary>
    /// Restore our settings backup if any.
    /// Used to persist settings across updates.
    /// </summary>
    private async void RestoreSettings()
    {
        // Check if we have settings that we need to restore
        if (!File.Exists(Settings.ConfigFile))
        {
            // Nothing to do
            return;
        }

        // Update the current config file with the backup
        await UpdateConfigFile(Settings.ConfigFile, false);

        // Delete backup file
        try
        {
            File.Delete(Settings.ConfigFile);
        }
        catch (Exception ex)
        {
            await _windowManager.ShowDialogAsync(new DialogMessageViewModel(
                    $"Failed to delete the backup config file:\n\n{ex.Message}",
                    "Config Restore Error"));
        }

    }
}
