using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace OrderReaderUI.Helpers;

public class ThemeManager
{
    #region Private Memebers

    private static readonly List<string> _themeList = new() { "Light", "Dark" };
    private static readonly List<string> _accentList = new() { "Grey", "Green", "Red", "Blue", "Purple", "Yellow" };

    #endregion

    #region Public Methods

    public static bool ChangeTheme(string themeName)
    {
        if (themeName == "Auto")
        {
            if (IsOSLightTheme())
                themeName = "Light";
            else
                themeName = "Dark";
        }

        if (_themeList.Contains(themeName))
        {
            if (themeName != GetCurrentTheme())
            {
                Uri uri = new($"Styles/Themes/{themeName}.xaml", UriKind.Relative);

                SetTheme(uri);
                return true;
            }
        }

        return false;
    }

    public static bool ChangeAccent(string accentName)
    {
        if (_accentList.Contains(accentName))
        {
            if (accentName != GetCurrentAccent())
            {
                Uri uri = new($"Styles/Accents/{accentName}.xaml", UriKind.Relative);

                SetAccent(uri);
                return true;
            }
        }

        return false;
    }

    public static bool IsOSLightTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var value = key?.GetValue("AppsUseLightTheme");
        return value is int i && i > 0;
    }

    #endregion

    #region Private Helpers

    private static string GetCurrentTheme()
    {
        var resources = Application.Current.Resources.MergedDictionaries;
        foreach (var resourceItem in resources)
        {
            string uri = resourceItem.Source?.OriginalString ?? "none";
            if (uri.StartsWith("Styles/Themes/"))
            {
                return Path.GetFileNameWithoutExtension(uri);
            }
        }
        return "none";
    }

    private static string GetCurrentAccent()
    {
        var resources = Application.Current.Resources.MergedDictionaries;
        foreach (var resourceItem in resources)
        {
            string uri = resourceItem.Source?.OriginalString ?? "none";
            if (uri.StartsWith("Styles/Accents/"))
            {
                return Path.GetFileNameWithoutExtension(uri);
            }
        }
        return "none";
    }

    private static void SetTheme(Uri theme)
    {
        var resources = Application.Current.Resources.MergedDictionaries;

        foreach ( var resourceItem in resources )
        {
            string uri = resourceItem.Source?.OriginalString ?? "none";
            if (uri.StartsWith("Styles/Themes/"))
            {
                resourceItem.Source = theme;
                return;
            }
        }
    }
    
    private static void SetAccent(Uri accent)
    {
        var resources = Application.Current.Resources.MergedDictionaries;

        foreach ( var resourceItem in resources )
        {
            string uri = resourceItem.Source?.OriginalString ?? "none";
            if (uri.StartsWith("Styles/Accents/"))
            {
                resourceItem.Source = accent;
                return;
            }
        }
    }

    #endregion
}
