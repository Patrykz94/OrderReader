using Caliburn.Micro;

namespace OrderReader.Dialogs.BaseViewModels;

public class DialogViewModelBase : Screen
{
    #region Public Properties

    public double WindowMaxWidth { get; private set; }
    public double WindowMaxHeight { get; private set; }
    public double WindowMinWidth { get; private set; }
    public double WindowMinHeight { get; private set; }

    public string Title { get; set; } = "Message";
    public string Message { get; set; } = "Default Message";
    public string PrimaryButtonText { get; set; } = "Ok";

    public bool? DialogResult { get; private set; }

    #endregion

    #region Constructors

    protected DialogViewModelBase()
    {
        Initialize();
    }

    #endregion

    #region Public Methods

    protected void Close(bool? dialogResult = null)
    {
        DialogResult = dialogResult;
        TryCloseAsync(dialogResult);
    }

    #endregion

    #region Protected Methods

    private void Initialize()
    {
        WindowMaxWidth = 600;
        WindowMaxHeight = 500;
        WindowMinWidth = 400;
        WindowMinHeight = 150;
    }

    #endregion
}