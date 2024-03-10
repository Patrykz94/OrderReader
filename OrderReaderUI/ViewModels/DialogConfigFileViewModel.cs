using Microsoft.Win32;
using OrderReaderUI.ViewModels.BaseViewModels;

namespace OrderReaderUI.ViewModels;

public class DialogConfigFileViewModel : DialogViewModelBase
{
    #region Private Variables

    private const string DefaultMessage = "To configure the application for your company, please either drop your configuration file here or browse for it:";

    #endregion
    
    #region Public Properties

    public string SecondaryButtonText { get; set; }

    public bool FileError { get; set; } = false;
    public string ConfigFileLocation { get; set; } = string.Empty;

    #endregion

    #region Constructors

    public DialogConfigFileViewModel()
    {
        Title = "Welcome";
        Message = DefaultMessage;
        PrimaryButtonText = "Configure";
        SecondaryButtonText = "Cancel";
    }
    
    public DialogConfigFileViewModel(string title = "Welcome", string okButtonText = "Configure", string cancelButtonText = "Cancel")
    {
        Title = title;
        Message = DefaultMessage;
        PrimaryButtonText = okButtonText;
        SecondaryButtonText = cancelButtonText;
    }

    #endregion

    #region Public Methods

    public void BrowseButton()
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

    public void PrimaryButton()
    {
        Close(true);
    }

    public void SecondaryButton()
    {
        Close(false);
    }

    #endregion
}