namespace OrderReader.Core
{
    /// <summary>
    /// Details for a yes/no box dialog
    /// </summary>
    public class YesNoBoxDialogDesignModel : YesNoBoxDialogViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static YesNoBoxDialogDesignModel Instance => new YesNoBoxDialogDesignModel();

        #endregion

        #region Constructor

        public YesNoBoxDialogDesignModel()
        {
            YesButtonText = "Yes";
            NoButtonText = "No";
            Question = "Design time Question :)";
        }

        #endregion
    }
}
