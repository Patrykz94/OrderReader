using System;
using System.IO;
using System.Threading.Tasks;
using OrderReader.Core.DataAccess;
using OrderReader.Core.DataModels;
using OrderReader.Core.Interfaces;

namespace OrderReaderUI.Helpers;

public class AppInitialization(INotificationService notificationService)
{
    public async Task Initialize()
    {
        // Initialize the settings class which creates all required directories
        Settings.Initialize();
        
        // Attempt to load user settings in order to set the desired theme and accent colour
        if (Settings.SettingsFileExists())
        {
            try
            {
                var userSettings = Settings.LoadSettings();
                ThemeManager.ChangeTheme(userSettings.Theme);
                ThemeManager.ChangeAccent(userSettings.Accent);
            }
            catch (Exception ex)
            {
                await notificationService.ShowMessage("Loading Theme Settings", $"Could not load theme settings: {ex}");
                throw;
            }
        }

        // Check if there are any saved settings and if so, restore them
        RestoreSettings();

        // Check if app is configured
        if (!SqliteDataAccess.HasConnectionString())
        {
            // Show Config File Dialog
            var configResult = await notificationService.ShowConfigMessage();
            
            // If user did not provide the file, exit the application
            if (configResult == string.Empty) Environment.Exit(0);

            await UpdateConfigFile(configResult);
        }

        // Test connection to the database
        if (!SqliteDataAccess.TestConnection())
        {
            // Show Config File Dialog
            const string message = "Application could not access the database.\n\n" +
                                   "This could mean the database file was moved or renamed, or it could indicate a network issue.\n\n" +
                                   "If you have a new configuration file, please drop it into the box below.";
            var configResult = await notificationService.ShowConfigMessage("Configuration Error", message);
            
            // If user did not provide the file, exit the application
            if (configResult == string.Empty) Environment.Exit(0);

            await UpdateConfigFile(configResult);

            // Test the new configs file again
            if (!SqliteDataAccess.TestConnection())
            {
                await notificationService.ShowMessage(
                    "Configuration Error",
                    "Could not connect to the database using the configuration file provided.\n\nApplication will now terminate.",
                    "Exit");

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
                    appConfig.UpdateConfigs(notificationService);
                }
                else
                {
                    await notificationService.ShowMessage(
                        "Configuration Error",
                        $"Could not read configuration data form the provided file.{(exitOnError ? "\n\nApplication will now terminate." : "")}",
                        "Exit");

                    if (exitOnError) Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                await notificationService.ShowMessage(
                    "Configuration Error",
                    $"Could not process the configuration file provided:\n\n{ex.Message}{(exitOnError ? "\n\nApplication will now terminate." : "")}",
                    "Exit");

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
            await notificationService.ShowMessage(
                "Config Restore Error",
                $"Failed to delete the backup config file:\n\n{ex.Message}");
        }

    }
}
