using Caliburn.Micro;

namespace OrderReaderUI.ViewModels;

public class DialogMessageViewModel : Screen
{
    #region Public Properties

    public double WindowMaxWidth { get; private set; }
    public double WindowMaxHeight { get; private set; }
    public double WindowMinWidth { get; private set; }
    public double WindowMinHeight { get; private set; }

    public string Title { get; init; } = "Message";
    public string Message { get; set; } = string.Empty;
    public string ButtonText { get; set; } = "OK";

    #endregion

    #region Constructor

    public DialogMessageViewModel()
    {
        Initialize();
    }
    
    public DialogMessageViewModel(string message, string title = "Message", string buttonText = "OK")
    {
        Message = message;
        Title = title;
        ButtonText = buttonText;

        Initialize();
    }

    #endregion

    #region Public Methods

    public void OkButton()
    {
        TryCloseAsync();
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
        WindowMaxWidth = 600;
        WindowMaxHeight = 500;
        WindowMinWidth = 400;
        WindowMinHeight = 150;
    }

    #endregion
}