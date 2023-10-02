using OrderReader.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OrderReaderUI.Helpers;

public static class AppInitialization
{
    public static async void Initialize()
    {
        // Initialize the settings class which creates all required directories
        Settings.Initialize();

        // Check if there are any saved settings and if so, restore them
        RestoreSettings();

        // Check if app is configured
        if (!SqliteDataAccess.HasConnectionString())
        {
            // Ask the user to drag and drop a file
            //var result = await IoC.UI.ShowMessage(new ConfigFileBoxDialogViewModel
            //{
            //    Title = "App Configuration",
            //    Heading = "Welcome to Order Reader!",
            //    Message = "To begin using this application, please drag and drop the configuration file provided with the download into the box below.",
            //    CancelButtonText = "Exit"
            //});

            string result = @"E:\Documents\Programming\OrderReader Resources\config.xml";

            await UpdateConfigFile(result);
        }

        // Test connection to the database
        if (!SqliteDataAccess.TestConnection())
        {
            //var result = await IoC.UI.ShowMessage(new ConfigFileBoxDialogViewModel
            //{
            //    Title = "Configuration Error",
            //    Message = "Application could not access the database.\n\nThis could mean the database file was moved or renamed, or it could indicate a network issue.\n\nIf you have a new configuration file, please drop it into the box below.",
            //    CancelButtonText = "Exit"
            //});

            //await UpdateConfigFile(result);

            // Test the new configs file again
            if (!SqliteDataAccess.TestConnection())
            {
                //await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                //{
                //    Title = "Configuration Error",
                //    Message = $"Could not connect to the database using the configuration file provided.\n\nApplication will now terminate.",
                //    ButtonText = "Exit"
                //});

                Environment.Exit(0);
            }
        }
    }

    /// <summary>
    /// Update the config file if required
    /// </summary>
    /// <param name="filePath">The path to the new config file</param>
    /// <returns></returns>
    private static async Task UpdateConfigFile(string filePath, bool exitOnError = true)
    {
        if (filePath == default)
        {
            if (exitOnError) Environment.Exit(0);
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
                    //await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    //{
                    //    Title = "Configuration Error",
                    //    Message = $"Could not read configuration data form the provided file.{(exitOnError ? "\n\nApplication will now terminate." : "")}",
                    //    ButtonText = "Exit"
                    //});

                    if (exitOnError) Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                //await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                //{
                //    Title = "Configuration Error",
                //    Message = $"Could not process the configuration file provided:\n\n{ex.Message}{(exitOnError ? "\n\nApplication will now terminate." : "")}",
                //    ButtonText = "Exit"
                //});

                if (exitOnError) Environment.Exit(0);
            }
        }
    }

    /// <summary>
    /// Restore our settings backup if any.
    /// Used to persist settings across updates.
    /// </summary>
    private static async void RestoreSettings()
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
            //await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            //{
            //    Title = "Config Restore Error",
            //    Message = $"Failed to delete the backup config file:\n\n{ex.Message}",
            //    ButtonText = "OK"
            //});
        }

    }
}
