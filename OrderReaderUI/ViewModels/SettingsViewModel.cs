using Caliburn.Micro;
using OrderReader.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using WinForms = System.Windows.Forms;

namespace OrderReaderUI.ViewModels;

public class SettingsViewModel : Screen
{
    #region Properties

    private bool _exportCSV;
    public bool ExportCSV
    {
        get => _exportCSV;
        set {
            _exportCSV = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => CanPathCSV);
            NotifyOfPropertyChange(() => CanBrowseCSV);
        }
    }

    private bool _exportPDF;
    public bool ExportPDF
    { 
        get => _exportPDF;
        set
        {
            _exportPDF = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => CanPathPDF);
            NotifyOfPropertyChange(() => CanBrowsePDF);
        }
    }

    private bool _printOrders;
    public bool PrintOrders
    {
        get => _printOrders;
        set
        {
            _printOrders = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => CanPrinters);
            NotifyOfPropertyChange(() => CanCopies);
        }
    }

    private string _pathCSV = string.Empty;
    public string PathCSV
    {
        get => _pathCSV;
        set
        {
            _pathCSV = value;
            NotifyOfPropertyChange();
        }
    }
    
    private string _pathPDF = string.Empty;
    public string PathPDF
    {
        get => _pathPDF;
        set
        {
            _pathPDF = value;
            NotifyOfPropertyChange();
        }
    }

    public ObservableCollection<string> Printers => PrintingManager.InstalledPrinters;
    public string SelectedPrinter { get; set; } = string.Empty;

    private int _copies;
    public int Copies
    {
        get => _copies;
        set
        {
            _copies = value;
            NotifyOfPropertyChange();
        }
    }

    public bool CanPathCSV => ExportCSV;
    public bool CanBrowseCSV => ExportCSV;
    public bool CanPathPDF => ExportPDF;
    public bool CanBrowsePDF => ExportPDF;
    public bool CanPrinters => PrintOrders;
    public bool CanCopies => PrintOrders;

    #endregion

    #region Constructor

    public SettingsViewModel()
    {
        LoadSettings();
    }

    #endregion

    #region Public Functions

    public void BrowseCSV()
    {
        WinForms.FolderBrowserDialog folderBrowserDialog = new()
        {
            InitialDirectory = PathCSV
        };
        WinForms.DialogResult result = folderBrowserDialog.ShowDialog();
        if (result == WinForms.DialogResult.OK)
        {
            PathCSV = folderBrowserDialog.SelectedPath;
        }
    }

    public void BrowsePDF()
    {
        WinForms.FolderBrowserDialog folderBrowserDialog = new()
        {
            InitialDirectory = PathPDF
        };
        WinForms.DialogResult result = folderBrowserDialog.ShowDialog();
        if (result == WinForms.DialogResult.OK)
        {
            PathPDF = folderBrowserDialog.SelectedPath;
        }
    }

    #endregion

    #region Private Helpers

    private void SaveSettings()
    {
        Dictionary<string, string> defaultSettings = SqliteDataAccess.LoadDefaultSettings();

        if (PathCSV == "") PathCSV = defaultSettings.ContainsKey("DefaultCSVExportPath") ? defaultSettings["DefaultCSVExportPath"] : Settings.DefaultExportPath;
        if (PathCSV == "" || PathCSV == null) PathCSV = Settings.DefaultExportPath;
        if (PathPDF == "") PathPDF = defaultSettings.ContainsKey("DefaultPDFExportPath") ? defaultSettings["DefaultPDFExportPath"] : Settings.DefaultExportPath;
        if (PathPDF == "" || PathPDF == null) PathPDF = Settings.DefaultExportPath;
        if (!Directory.Exists(PathCSV)) Directory.CreateDirectory(PathCSV);
        if (!Directory.Exists(PathPDF)) Directory.CreateDirectory(PathPDF);

        UserSettings settings = new()
        {
            ExportCSV = ExportCSV,
            ExportPDF = ExportPDF,
            PrintOrders = PrintOrders,

            UserCSVExportPath = PathCSV,
            UserPDFExportPath = PathPDF,
            PreferredPrinterName = SelectedPrinter,
            PrintingCopies = Copies
        };

        Settings.SaveSettings(settings);
    }

    private void LoadSettings()
    {
        UserSettings settings = Settings.LoadSettings();

        ExportCSV = settings.ExportCSV;
        ExportPDF = settings.ExportPDF;
        PrintOrders = settings.PrintOrders;

        PathCSV = settings.UserCSVExportPath;
        PathPDF = settings.UserPDFExportPath;
        SelectedPrinter = settings.PreferredPrinterName;
        Copies = settings.PrintingCopies;

        if (SelectedPrinter == null || !Printers.Contains(SelectedPrinter))
            SelectedPrinter = PrintingManager.DefaultPrinter;
    }

    #endregion
}
