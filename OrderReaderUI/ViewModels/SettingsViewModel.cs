using Caliburn.Micro;
using OrderReader.Core;
using OrderReaderUI.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

    private string _selectedTheme;
    public string SelectedTheme
    {
        get { return _selectedTheme; }
        set
        {
            if (value != _selectedTheme)
            {
                ThemeManager.ChangeTheme(value);
                _selectedTheme = value;
                NotifyOfPropertyChange();
            }
        }
    }

    private string _selectedAccent;
    public string SelectedAccent
    {
        get { return _selectedAccent; }
        set
        {
            if (value != _selectedAccent)
            {
                ThemeManager.ChangeAccent(value);
                _selectedAccent = value;
                NotifyOfPropertyChange();
            }
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

    public void ReloadCSVSettings()
    {
        PathCSV = LoadDefaultSetting("DefaultCSVExportPath") ?? Settings.DefaultExportPath;
    }

    public void ReloadPDFSettings()
    {
        PathPDF = LoadDefaultSetting("DefaultPDFExportPath") ?? Settings.DefaultExportPath;
    }

    public void ReloadPrintingSettings()
    {
        string? defaultPrinter = LoadDefaultSetting("DefaultPrinter");
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
            PrintingCopies = Copies,

            Theme = SelectedTheme,
            Accent = SelectedAccent
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

        SelectedTheme = settings.Theme;
        SelectedAccent = settings.Accent;

        if (SelectedPrinter == null || !Printers.Contains(SelectedPrinter))
            SelectedPrinter = PrintingManager.DefaultPrinter;
    }

    private string? LoadDefaultSetting(string settingName)
    {
        Dictionary<string, string> defaultSettings = SqliteDataAccess.LoadDefaultSettings();

        if (defaultSettings.ContainsKey(settingName)) { return defaultSettings[settingName]; }

        return null;
    }

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        if (close)
        {
            SaveSettings();
        }
        return base.OnDeactivateAsync(close, cancellationToken);
    }

    #endregion
}
