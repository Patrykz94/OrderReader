namespace OrderReader.Core
{
    /// <summary>
    /// Details for a message box dialog
    /// </summary>
    public class MessageBoxDialogViewModel : BaseDialogViewModel
    {
        /// <summary>
        /// Message to be displayed
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Text that will appear on the OK Button
        /// </summary>
        public string ButtonText { get; set; }
    }
}
