using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.IO;

namespace OrderReader.Core
{
    public class PDFImport
    {
        #region Public Properties

        /// <summary>
        /// All text extracted from a PDF file, separated by lines
        /// </summary>
        public string[] Lines { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that opens the PDF file and reads from it
        /// </summary>
        /// <param name="filePath"></param>
        public PDFImport(string filePath)
        {
            Lines = ReadAllLines(filePath);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Get the name of customer from an order file
        /// </summary>
        /// <returns>Name of customer or <see cref="null"/></returns>
        public int GetCustomerId()
        {
            // Load the customers
            CustomersHandler customers = IoC.Customers();
            customers.LoadCustomers();

            // Check if the any of the lines contain a customer name
            foreach (string line in Lines)
            {
                if (customers.HasCustomerOrderName(line)) return customers.GetCustomerByOrderName(line).Id;
            }

            // If nothing was found, return -1
            return -1;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Extracts all text from PDF file and separates it by lines
        /// </summary>
        /// <param name="filePath">The location of PDF file to read from</param>
        /// <returns>Array of <see cref="string"/> each containing a single line</returns>
        private string[] ReadAllLines(string filePath)
        {
            // Create the return array first
            string[] lines = {""};

            // Make sure the file exists
            if (File.Exists(filePath))
            {
                // Make sure we are looking at a PDF file
                string ext = Path.GetExtension(filePath);
                if (ext.ToLower() == ".pdf")
                {
                    string allExtractedText = "";

                    // Load the PDF file and extract all text
                    using (PdfReader reader = new PdfReader(filePath))
                    {
                        using (PdfDocument doc = new PdfDocument(reader))
                        {
                            for (int page = 1; page <= doc.GetNumberOfPages(); page++)
                            {
                                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                                allExtractedText = string.Concat(allExtractedText, PdfTextExtractor.GetTextFromPage(doc.GetPage(page), strategy));
                            }
                        }
                    }

                    lines = allExtractedText.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );
                }
            }

            return lines;
        }

        #endregion
    }
}
