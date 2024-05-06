using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OrderReader.Core.DataAccess;

namespace OrderReader.Core.DataModels;

/// <summary>
/// A class that will store the user specific settings of the application
/// </summary>
[Serializable]
public class UserSettings : ISerializable
{
    #region Public Properties

    /// <summary>
    /// Whether the CSV files should be exported
    /// </summary>
    public bool ExportCSV { get; set; }

    /// <summary>
    /// The export location for CSV files that the user selects. It will override the default export location
    /// </summary>
    public string UserCSVExportPath { get; set; }

    /// <summary>
    /// Whether the PDF file should be exported
    /// </summary>
    public bool ExportPDF { get; set; }

    /// <summary>
    /// The export location for PDF files that the user selects. It will override the default export location
    /// </summary>
    public string UserPDFExportPath { get; set; }

    /// <summary>
    /// Whether or not the processed orders should be printed
    /// </summary>
    public bool PrintOrders { get; set; }

    /// <summary>
    /// The printer that user prefers to use. If not available, default printer will be chosen
    /// </summary>
    public string PreferredPrinterName { get; set; }

    /// <summary>
    /// Number of copies to print
    /// </summary>
    public int PrintingCopies { get; set; }

    /// <summary>
    /// Name of the theme to use
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// Name of the accent colour to use
    /// </summary>
    public string Accent { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor
    /// </summary>
    public UserSettings()
    {
        Dictionary<string, string> defaultSettings = SqliteDataAccess.LoadDefaultSettings();

        UserCSVExportPath = defaultSettings.ContainsKey("DefaultCSVExportPath") ? defaultSettings["DefaultCSVExportPath"] : Settings.DefaultExportPath;
        if (UserCSVExportPath == null || UserCSVExportPath == "") UserCSVExportPath = Settings.DefaultExportPath;
        UserPDFExportPath = defaultSettings.ContainsKey("DefaultPDFExportPath") ? defaultSettings["DefaultPDFExportPath"] : Settings.DefaultExportPath;
        if (UserPDFExportPath == null || UserPDFExportPath == "") UserPDFExportPath = Settings.DefaultExportPath;
        PreferredPrinterName = defaultSettings.ContainsKey("DefaultPrinter") ? defaultSettings["DefaultPrinter"] : null;
        ExportCSV = true;
        ExportPDF = true;
        PrintOrders = true;
        PrintingCopies = 1;
        Theme = "Auto";
        Accent = "Red";
    }

    /// <summary>
    /// A constructor that deserializes this object
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public UserSettings(SerializationInfo info, StreamingContext context)
    {
        // Load saved settings
        UserCSVExportPath = (string)info.GetValue("UserCSVExportPath", typeof(string));
        UserPDFExportPath = (string)info.GetValue("UserPDFExportPath", typeof(string));
        ExportCSV = (bool)info.GetValue("ExportCSV", typeof(bool));
        ExportPDF = (bool)info.GetValue("ExportPDF", typeof(bool));
        PrintOrders = (bool)info.GetValue("PrintOrders", typeof(bool));
        PreferredPrinterName = (string)info.GetValue("SelectedPrinterName", typeof(string));
        PrintingCopies = (int)info.GetValue("PrintingCopies", typeof(int));
        Theme = (string)info.GetValue("Theme", typeof(string));
        Accent = (string)info.GetValue("Accent", typeof(string));
    }

    #endregion

    #region Public Helpers

    /// <summary>
    /// Serializes this object
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("UserCSVExportPath", UserCSVExportPath);
        info.AddValue("UserPDFExportPath", UserPDFExportPath);
        info.AddValue("ExportCSV", ExportCSV);
        info.AddValue("ExportPDF", ExportPDF);
        info.AddValue("PrintOrders", PrintOrders);
        info.AddValue("SelectedPrinterName", PreferredPrinterName);
        info.AddValue("PrintingCopies", PrintingCopies);
        info.AddValue("Theme", Theme);
        info.AddValue("Accent", Accent);
    }

    #endregion
}