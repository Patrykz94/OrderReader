using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OrderReader.Core
{
    public class PDFImport : IFileImport
    {
        #region Private Variables

        /// <summary>
        /// All text extracted from a PDF file, separated by lines and pages
        /// </summary>
        private readonly Dictionary<string, string[]> orderText;

        /// <summary>
        /// The extension of the file that we are working on
        /// </summary>
        private readonly string fileExtension;

        /// <summary>
        /// Name of the file that we are working on including extension
        /// </summary>
        private readonly string fileName;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that opens the PDF file and reads from it
        /// </summary>
        /// <param name="filePath"></param>
        public PDFImport(string filePath)
        {
            fileExtension = Path.GetExtension(filePath).ToLower();
            fileName = Path.GetFileName(filePath);
            orderText = ReadAllLines(filePath);
        }

        #endregion

        #region Public Helpers

        public async Task<bool> ProcessFileAsync()
        {
            // List of available processors - TODO: Populate this list automatically
            List<IParseOrder> orderParsers = new List<IParseOrder>();
            orderParsers.Add(new KeelingsPDFParser());

            CustomersHandler customers = IoC.Customers();
            customers.LoadCustomers();

            foreach (IParseOrder parser in orderParsers)
            {
                if (parser.FileExtension == fileExtension)
                {
                    Customer customer = parser.GetCustmer(orderText, customers);

                    if (customer != null)
                    {
                        await parser.ParseOrderAsync(orderText, fileName, customer);
                        return true;
                    }
                }
            }

            string errorMessage = $"Could not identify customer information in file {fileName}\n\n" +
                    "Please double check the PDF file to make sure it contains a valid order.\n";

            // Display error message to the user
            await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "File Processing Error",
                Message = errorMessage,
                ButtonText = "OK"
            });

            return false;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Extracts all text from PDF file and separates it by lines
        /// </summary>
        /// <param name="filePath">The location of PDF file to read from</param>
        /// <returns>Array of <see cref="string"/> each containing a single line</returns>
        private Dictionary<string, string[]> ReadAllLines(string filePath)
        {
            // Create the return array first
            Dictionary<string, string[]> text = new Dictionary<string, string[]>();

            // Make sure the file exists
            if (File.Exists(filePath))
            {
                List<string> allExtractedText = new List<string>();

                // Load the PDF file and extract all text
                using (PdfReader reader = new PdfReader(filePath))
                {
                    using (PdfDocument doc = new PdfDocument(reader))
                    {
                        for (int page = 1; page <= doc.GetNumberOfPages(); page++)
                        {
                            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                            string extractedText = "";
                            extractedText = string.Concat(extractedText, PdfTextExtractor.GetTextFromPage(doc.GetPage(page), strategy));
                            allExtractedText.Add(extractedText);
                        }
                    }
                }

                for (int i = 0; i < allExtractedText.Count; i++)
                {
                    text.Add((i + 1).ToString(), allExtractedText[i].Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    ));
                }
            }

            return text;
        }

        #endregion
    }
}
