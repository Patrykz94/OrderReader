﻿using OrderReaderUI.ViewModels.BaseViewModels;

namespace OrderReaderUI.ViewModels;

public class DialogMessageViewModel : DialogViewModelBase
{
    #region Constructors

    public DialogMessageViewModel()
    {}
    
    public DialogMessageViewModel(string message, string title = "Message", string buttonText = "Ok")
    {
        Message = message;
        Title = title;
        PrimaryButtonText = buttonText;
    }

    #endregion

    #region Public Methods

    public void PrimaryButton()
    {
        Close();
    }

    #endregion
}