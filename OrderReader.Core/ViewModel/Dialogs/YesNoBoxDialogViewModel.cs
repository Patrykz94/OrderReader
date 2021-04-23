namespace OrderReader.Core
{
    /// <summary>
    /// Details for a message box dialog
    /// </summary>
    public class YesNoBoxDialogViewModel : BaseDialogViewModel
    {
        #region Public properties

        /// <summary>
        /// Question to be displayed
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Text that will appear on the Yes answer Button
        /// </summary>
        public string YesButtonText { get; set; } = "Yes";

        /// <summary>
        /// Text that will appear on the No answer Button
        /// </summary>
        public string NoButtonText { get; set; } = "No";

        #endregion
    }
}
