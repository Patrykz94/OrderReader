using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace OrderReaderUI.Helpers;

public class ThemeManager
{
    #region Private Memebers

    private static readonly List<string> ThemeList = ["Light", "Dark"];
    private static readonly List<string> AccentList = ["Grey", "Green", "Red", "Blue", "Purple", "Yellow"];

    #endregion

    #region Public Methods

    public static bool ChangeTheme(string themeName)
    {
        if (themeName == "Auto")
            themeName = IsOsLightTheme() ? "Light" : "Dark";

        if (!ThemeList.Contains(themeName)) return false;

        if (themeName == GetCurrentTheme()) return false;
        
        Uri uri = new($"Styles/Themes/{themeName}.xaml", UriKind.Relative);

        SetTheme(uri);
        return true;
    }

    public static bool ChangeAccent(string accentName)
    {
        if (!AccentList.Contains(accentName)) return false;

        if (accentName == GetCurrentAccent()) return false;
        
        Uri uri = new($"Styles/Accents/{accentName}.xaml", UriKind.Relative);

        SetAccent(uri);
        return true;

    }

    public static bool IsOsLightTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var value = key?.GetValue("AppsUseLightTheme");
        return value is > 0;
    }

    #endregion

    #region Private Helpers

    private static string GetCurrentTheme()
    {
        var resources = Application.Current.Resources.MergedDictionaries;
        foreach (var resourceItem in resources)
        {
            var uri = resourceItem.Source?.OriginalString ?? "none";
            if (uri.StartsWith("Styles/Themes/"))
                return Path.GetFileNameWithoutExtension(uri);
        }
        return "none";
    }

    private static string GetCurrentAccent()
    {
        var resources = Application.Current.Resources.MergedDictionaries;
        foreach (var resourceItem in resources)
        {
            var uri = resourceItem.Source?.OriginalString ?? "none";
            if (uri.StartsWith("Styles/Accents/"))
                return Path.GetFileNameWithoutExtension(uri);
        }
        return "none";
    }

    private static void SetTheme(Uri theme)
    {
        var resources = Application.Current.Resources.MergedDictionaries;

        foreach ( var resourceItem in resources )
        {
            var uri = resourceItem.Source?.OriginalString ?? "none";
            if (!uri.StartsWith("Styles/Themes/")) continue;
            resourceItem.Source = theme;
            return;
        }
    }
    
    private static void SetAccent(Uri accent)
    {
        var resources = Application.Current.Resources.MergedDictionaries;

        foreach ( var resourceItem in resources )
        {
            var uri = resourceItem.Source?.OriginalString ?? "none";
            if (!uri.StartsWith("Styles/Accents/")) continue;
            resourceItem.Source = accent;
            return;
        }
    }

    #endregion
}
