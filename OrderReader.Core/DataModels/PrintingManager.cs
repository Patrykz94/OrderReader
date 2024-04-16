using Spire.Pdf;
using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core
{
    public static class PrintingManager
    {
        #region Public Properties

        /// <summary>
        /// Returns a list of all printers
        /// </summary>
        public static ObservableCollection<string> InstalledPrinters
        {
            get
            {
                ObservableCollection<string> printers = new ObservableCollection<string>();
                foreach (var printer in PrinterSettings.InstalledPrinters)
                {
                    printers.Add(printer.ToString());
                }
                return printers;
            }
        }

        /// <summary>
        /// Name of the default system printer
        /// </summary>
        public static string DefaultPrinter = new PrinterSettings().PrinterName;
        
        public static INotificationService NotificationService { get; set; }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Prints a PDF document
        /// </summary>
        /// <param name="filePath">Path to the PDF file to be printed</param>
        /// <returns>True or false whether printing was successful</returns>
        public static bool PrintPDF(string filePath)
        {
            // Load user settings
            UserSettings settings = Settings.LoadSettings();

            // Print the file
            try
            {
                // Load the PDF file to print
                using (var document = new PdfDocument(filePath))
                {
                    // Adjust the print settings
                    document.PrintSettings.PrinterName = PrinterAvailable(settings.PreferredPrinterName) ? settings.PreferredPrinterName : DefaultPrinter;
                    document.PrintSettings.Copies = (short)settings.PrintingCopies;
                    document.PrintSettings.PrintController = new StandardPrintController();
                    document.PrintSettings.SetPaperMargins(0,0,0,0);

                    // Print the document
                    document.Print();
                }

                return true;
            }
            catch (Exception ex)
            {
                NotificationService.ShowMessage("Printing Error", $"An error occured while trying to print the order:\n{ex.Message}\nOperation was aborted.");
                return false;
            }
        }

        /// <summary>
        /// Check whether the printer provided exists
        /// </summary>
        /// <param name="printerName">Name of the printer</param>
        /// <returns>True or false</returns>
        public static bool PrinterAvailable(string printerName)
        {
            foreach (string printer in InstalledPrinters)
            {
                if (printer == printerName) return true;
            }

            return false;
        }

        #endregion
    }
}
