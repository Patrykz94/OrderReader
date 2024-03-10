using Caliburn.Micro;
using Microsoft.Win32;

namespace OrderReaderUI.ViewModels;

public class DialogConfigFileViewModel : Screen
{
    #region Public Properties

    public double WindowMaxWidth { get; private set; }
    public double WindowMaxHeight { get; private set; }
    public double WindowMinWidth { get; private set; }
    public double WindowMinHeight { get; private set; }

    public string Title { get; set; }
    public string Message { get; set; } = "To configure the application for your company, please either drop your configuration file here or browse for it:";
    public string OkButtonText { get; set; }
    public string CancelButtonText { get; set; }

    public bool FileError { get; set; } = false;
    public string ConfigFileLocation { get; set; } = string.Empty;

    #endregion

    #region Constructor

    public DialogConfigFileViewModel(string title = "Welcome", string okButtonText = "OK", string cancelButtonText = "Cancel")
    {
        Title = title;
        OkButtonText = okButtonText;
        CancelButtonText = cancelButtonText;

        Initialize();
    }

    #endregion

    #region Public Methods

    public void BrowseConfig()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "XML files (*.xml)|*.xml"
        };

        if (dialog.ShowDialog() == true && dialog.CheckFileExists)
        {
            ConfigFileLocation = dialog.FileName;
        }
    }

    public void OkButton()
    {
        TryCloseAsync(true);
    }

    public void CancelButton()
    {
        TryCloseAsync(false);
    }

    #endregion
    
    #region Private Methods

    private void Initialize()
    {
        WindowMaxWidth = 600;
        WindowMaxHeight = 500;
        WindowMinWidth = 400;
        WindowMinHeight = 240;
    }

    #endregion
}