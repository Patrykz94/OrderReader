using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader.Core;
using OrderReaderUI.Helpers;
using WinForms = System.Windows.Forms;

namespace OrderReaderUI.Pages.Settings;

public class SettingsViewModel : Screen
{
    #region Properties

    private bool _exportCsv;
    public bool ExportCsv
    {
        get => _exportCsv;
        set {
            _exportCsv = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => CanPathCsv);
            NotifyOfPropertyChange(() => CanBrowseCsv);
        }
    }

    private bool _exportPdf;
    public bool ExportPdf
    { 
        get => _exportPdf;
        set
        {
            _exportPdf = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => CanPathPdf);
            NotifyOfPropertyChange(() => CanBrowsePdf);
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

    private string _pathCsv = string.Empty;
    public string PathCsv
    {
        get => _pathCsv;
        set
        {
            _pathCsv = value;
            NotifyOfPropertyChange();
        }
    }
    
    private string _pathPdf = string.Empty;
    public string PathPdf
    {
        get => _pathPdf;
        set
        {
            _pathPdf = value;
            NotifyOfPropertyChange();
        }
    }

    private string _selectedTheme = "Light";
    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (value == _selectedTheme) return;
            ThemeManager.ChangeTheme(value);
            _selectedTheme = value;
            NotifyOfPropertyChange();
        }
    }

    private string _selectedAccent = "Red";
    public string SelectedAccent
    {
        get => _selectedAccent;
        set
        {
            if (value == _selectedAccent) return;
            ThemeManager.ChangeAccent(value);
            _selectedAccent = value;
            NotifyOfPropertyChange();
        }
    }

    public ObservableCollection<string> Printers => PrintingManager.InstalledPrinters;

    private string _selectedPrinter = string.Empty;
    public string SelectedPrinter
    {
        get => _selectedPrinter;
        set
        {
            _selectedPrinter = value;
            NotifyOfPropertyChange();
        }
    }

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

    public bool CanPathCsv => ExportCsv;
    public bool CanBrowseCsv => ExportCsv;
    public bool CanPathPdf => ExportPdf;
    public bool CanBrowsePdf => ExportPdf;
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

    public void BrowseCsv()
    {
        WinForms.FolderBrowserDialog folderBrowserDialog = new()
        {
            InitialDirectory = PathCsv
        };
        var result = folderBrowserDialog.ShowDialog();
        if (result == WinForms.DialogResult.OK)
        {
            PathCsv = folderBrowserDialog.SelectedPath;
        }
    }

    public void BrowsePdf()
    {
        WinForms.FolderBrowserDialog folderBrowserDialog = new()
        {
            InitialDirectory = PathPdf
        };
        var result = folderBrowserDialog.ShowDialog();
        if (result == WinForms.DialogResult.OK)
        {
            PathPdf = folderBrowserDialog.SelectedPath;
        }
    }

    public void ReloadCsvSettings()
    {
        PathCsv = LoadDefaultSetting("DefaultCSVExportPath") ?? OrderReader.Core.Settings.DefaultExportPath;
    }

    public void ReloadPdfSettings()
    {
        PathPdf = LoadDefaultSetting("DefaultPDFExportPath") ?? OrderReader.Core.Settings.DefaultExportPath;
    }

    public void ReloadPrintingSettings()
    {
        var defaultPrinter = LoadDefaultSetting("DefaultPrinter");
        if (defaultPrinter == null || !Printers.Contains(defaultPrinter))
            SelectedPrinter = PrintingManager.DefaultPrinter;
        else
            SelectedPrinter = defaultPrinter;
    }

    #endregion

    #region Private Helpers

    private void SaveSettings()
    {
        Dictionary<string, string> defaultSettings = SqliteDataAccess.LoadDefaultSettings();

        if (PathCsv == string.Empty) PathCsv = defaultSettings.TryGetValue("DefaultCSVExportPath", out var value) ? value : OrderReader.Core.Settings.DefaultExportPath;
        if (string.IsNullOrEmpty(PathCsv)) PathCsv = OrderReader.Core.Settings.DefaultExportPath;
        if (PathPdf == string.Empty) PathPdf = defaultSettings.TryGetValue("DefaultPDFExportPath", out var value) ? value : OrderReader.Core.Settings.DefaultExportPath;
        if (string.IsNullOrEmpty(PathPdf)) PathPdf = OrderReader.Core.Settings.DefaultExportPath;
        if (!Directory.Exists(PathCsv)) Directory.CreateDirectory(PathCsv);
        if (!Directory.Exists(PathPdf)) Directory.CreateDirectory(PathPdf);

        UserSettings settings = new()
        {
            ExportCSV = ExportCsv,
            ExportPDF = ExportPdf,
            PrintOrders = PrintOrders,

            UserCSVExportPath = PathCsv,
            UserPDFExportPath = PathPdf,
            PreferredPrinterName = SelectedPrinter,
            PrintingCopies = Copies,

            Theme = SelectedTheme,
            Accent = SelectedAccent
        };

        OrderReader.Core.Settings.SaveSettings(settings);
    }

    private void LoadSettings()
    {
        var settings = OrderReader.Core.Settings.LoadSettings();

        ExportCsv = settings.ExportCSV;
        ExportPdf = settings.ExportPDF;
        PrintOrders = settings.PrintOrders;

        PathCsv = settings.UserCSVExportPath;
        PathPdf = settings.UserPDFExportPath;
        SelectedPrinter = settings.PreferredPrinterName;
        Copies = settings.PrintingCopies;

        SelectedTheme = settings.Theme;
        SelectedAccent = settings.Accent;

        if (SelectedPrinter == null || !Printers.Contains(SelectedPrinter))
            SelectedPrinter = PrintingManager.DefaultPrinter;
    }

    private string? LoadDefaultSetting(string settingName)
    {
        Dictionary<string, string> defaultSettings = SqliteDataAccess.LoadDefaultSettings();

        return defaultSettings.TryGetValue(settingName, out var value) ? value : null;
    }

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        if (close) SaveSettings();
        
        return base.OnDeactivateAsync(close, cancellationToken);
    }

    #endregion
}
