using System.IO;

namespace OrderReader.Core
{
    public class FileImport
    {
        #region Private Enums

        private enum CustomerNames
        {
            Keelings_Coop = 1,
            Keelings = 2,
            Lidl = 3
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Lines of text that have been read from a PDF file
        /// </summary>
        private string[] LinesOfText = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// The path to the file we are working on
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// The name of the file we are working on
        /// </summary>
        public string FileName { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filePath"></param>
        public FileImport(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(FilePath);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Process the file
        /// </summary>
        /// <returns>True or False whether the file was processed successfully</returns>
        public bool ProcessFile()
        {
            // TODO: Every time we return false, we should display an error message stating what went wrong

            // Make sure the file exists
            if (File.Exists(FilePath))
            {
                // Determine what file extension we are dealing with and call appropriate import class
                string ext = Path.GetExtension(FilePath);
                switch (ext.ToLower())
                {
                    case ".xlsx":
                        // Display error message to the user
                        IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                        {
                            Title = "File Processing Error",
                            Message = "Processing of Excel files has not been implemented yet.",
                            ButtonText = "OK"
                        });
                        return false;
                    case ".pdf":
                        PDFImport importedFile = new PDFImport(FilePath);

                        if (importedFile.GetCustomerId() == -1)
                        {
                            string errorMessage = $"Could not identify customer information in file {FileName}.\n" +
                                "Please double check the PDF file to make sure it contains a valid order.\n";

                            // Display error message to the user
                            IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                            {
                                Title = "File Processing Error",
                                Message = errorMessage,
                                ButtonText = "OK"
                            });

                            return false;
                        }

                        Customer customer = IoC.Customers().GetCustomerByID(importedFile.GetCustomerId());

                        LinesOfText = importedFile.Lines;

                        switch ((CustomerNames)customer.Id)
                        {
                            case CustomerNames.Keelings_Coop:
                                KeelingsPDFParser.ParseOrder(LinesOfText, FileName, customer);
                                break;
                            default:
                                // Display error message to the user
                                IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                                {
                                    Title = "File Processing Error",
                                    Message = "Could not process order for this customer.",
                                    ButtonText = "OK"
                                });
                                return false;
                        }

                        return true;
                    default:
                        // Display error message to the user
                        IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                        {
                            Title = "File Processing Error",
                            Message = "Unsupported file type.",
                            ButtonText = "OK"
                        });
                        return false;
                }
            }

            // Display error message to the user
            IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "File Processing Error",
                Message = "Could not process this file.",
                ButtonText = "OK"
            });

            return false;
        }

        #endregion
    }
}
