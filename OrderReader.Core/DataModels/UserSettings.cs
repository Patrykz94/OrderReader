using System;
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
    public bool ExportCsv { get; set; }

    /// <summary>
    /// The export location for CSV files that the user selects. It will override the default export location
    /// </summary>
    public string UserCsvExportPath { get; set; }

    /// <summary>
    /// Whether the PDF file should be exported
    /// </summary>
    public bool ExportPdf { get; set; }

    /// <summary>
    /// The export location for PDF files that the user selects. It will override the default export location
    /// </summary>
    public string UserPdfExportPath { get; set; }

    /// <summary>
    /// Whether the processed orders should be printed
    /// </summary>
    public bool PrintOrders { get; set; }

    /// <summary>
    /// The printer that user prefers to use. If not available, default printer will be chosen
    /// </summary>
    public string? PreferredPrinterName { get; set; }

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
        UserCsvExportPath = Settings.DefaultExportPath;
        UserPdfExportPath = Settings.DefaultExportPath;
        PreferredPrinterName = null;
        ExportCsv = true;
        ExportPdf = true;
        PrintOrders = true;
        PrintingCopies = 1;
        Theme = "Auto";
        Accent = "Red";

        if (!SqliteDataAccess.HasConnectionString() && !SqliteDataAccess.TestConnection()) return;
        
        var defaultSettings = SqliteDataAccess.LoadDefaultSettings();

        UserCsvExportPath = defaultSettings.TryGetValue("DefaultCSVExportPath", out var csvExportPath) && !string.IsNullOrEmpty(csvExportPath) ? csvExportPath : Settings.DefaultExportPath;
        UserPdfExportPath = defaultSettings.TryGetValue("DefaultPDFExportPath", out var pdfExportPath) && !string.IsNullOrEmpty(pdfExportPath) ? pdfExportPath : Settings.DefaultExportPath;
        PreferredPrinterName = defaultSettings.TryGetValue("DefaultPrinter", out var defaultSetting) ? defaultSetting : null;
    }

    /// <summary>
    /// A constructor that deserializes this object
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public UserSettings(SerializationInfo info, StreamingContext context)
    {
        // Load saved settings
        UserCsvExportPath = (string)info.GetValue("UserCSVExportPath", typeof(string))!;
        UserPdfExportPath = (string)info.GetValue("UserPDFExportPath", typeof(string))!;
        ExportCsv = (bool)info.GetValue("ExportCSV", typeof(bool))!;
        ExportPdf = (bool)info.GetValue("ExportPDF", typeof(bool))!;
        PrintOrders = (bool)info.GetValue("PrintOrders", typeof(bool))!;
        PreferredPrinterName = (string)info.GetValue("SelectedPrinterName", typeof(string))!;
        PrintingCopies = (int)info.GetValue("PrintingCopies", typeof(int))!;
        Theme = (string)info.GetValue("Theme", typeof(string))!;
        Accent = (string)info.GetValue("Accent", typeof(string))!;
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
        info.AddValue("UserCSVExportPath", UserCsvExportPath);
        info.AddValue("UserPDFExportPath", UserPdfExportPath);
        info.AddValue("ExportCSV", ExportCsv);
        info.AddValue("ExportPDF", ExportPdf);
        info.AddValue("PrintOrders", PrintOrders);
        info.AddValue("SelectedPrinterName", PreferredPrinterName);
        info.AddValue("PrintingCopies", PrintingCopies);
        info.AddValue("Theme", Theme);
        info.AddValue("Accent", Accent);
    }

    #endregion
}