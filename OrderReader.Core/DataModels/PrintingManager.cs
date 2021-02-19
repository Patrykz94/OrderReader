using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;

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
                // Create printer settings for our printers
                PrinterSettings printerSettings = new PrinterSettings
                {
                    PrinterName = PrinterAvailable(settings.PreferredPrinterName) ? settings.PreferredPrinterName : DefaultPrinter,
                    Copies = (short)settings.PrintingCopies
                };

                // Create page settings
                PageSettings pageSettings = new PageSettings
                {
                    Margins = new Margins(0, 0, 0, 0)
                };

                // Now print the PDF document
                using (var document = PdfiumViewer.PdfDocument.Load(filePath))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Printing Error",
                    Message = $"An error occured while trying to print the order:\n{ex.Message}\nOperation was aborted.",
                    ButtonText = "OK"
                });
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
