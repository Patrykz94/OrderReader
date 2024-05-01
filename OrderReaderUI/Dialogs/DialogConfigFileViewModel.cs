using System;
using System.IO;
using OrderReader;
using OrderReaderUI.Dialogs.BaseViewModels;

namespace OrderReaderUI.Dialogs;

public class DialogConfigFileViewModel : DialogViewModelBase, IFilesDropped
{
    #region Private Variables

    private const string DefaultMessage = "To configure the application for your company, please drop your configuration .xml file here:";

    #endregion
    
    #region Public Properties

    public string SecondaryButtonText { get; set; }

    private string _configFileLocation;
    public string ConfigFileLocation
    {
        get => _configFileLocation;
        set
        {
            _configFileLocation = value;
            NotifyOfPropertyChange();
        }
    }

    private string _fileError;
    public string FileError
    {
        get => _fileError;
        set
        {
            _fileError = value; 
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => HasError);
            NotifyOfPropertyChange(() => CanPrimaryButton);
        }
    }

    public bool HasError => FileError != string.Empty;

    public bool CanPrimaryButton => !HasError;
    
    #endregion

    #region Constructors

    public DialogConfigFileViewModel()
    {
        Title = "Welcome";
        Message = DefaultMessage;
        PrimaryButtonText = "Configure";
        SecondaryButtonText = "Exit";
        _configFileLocation = string.Empty;
        _fileError = "No file selected";
    }
    
    public DialogConfigFileViewModel(string title = "Welcome", string primaryButtonText = "Configure", string secondaryButtonText = "Exit")
    {
        Title = title;
        Message = DefaultMessage;
        PrimaryButtonText = primaryButtonText;
        SecondaryButtonText = secondaryButtonText;
        _configFileLocation = string.Empty;
        _fileError = "No file selected";
    }

    #endregion

    #region Public Methods

    public void PrimaryButton()
    {
        Close(true);
    }

    public void SecondaryButton()
    {
        Close(false);
    }
    
    public void OnFilesDropped(string[] files)
    {
        var filePath = files[0];
        if (File.Exists(filePath))
        {
            // Determine if we are dealing with a correct file extension
            var ext = Path.GetExtension(filePath);

            if (ext.Equals(".xml", StringComparison.CurrentCultureIgnoreCase))
            {
                ConfigFileLocation = filePath;
                FileError = string.Empty;

                return;
            }

            FileError = "Incorrect file type";
        }
        else
        {
            FileError = "Selected file doesn't exist";
        }

        ConfigFileLocation = string.Empty;
    }

    #endregion
}