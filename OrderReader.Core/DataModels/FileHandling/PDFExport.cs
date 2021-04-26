using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Data;
using System.IO;

namespace OrderReader.Core
{
    public static class PDFExport
    {
        public static void ExportOrderToPDF(OrderListItemViewModel orders)
        {
            DataTable ordersTable = orders.OrdersTable;
            ordersTable.Columns.Add(new DataColumn("Sales Order"));
            ordersTable.Columns.Add(new DataColumn("Pal"));
            ordersTable.Columns.Add(new DataColumn("Lorry Number"));

            // Create the file name
            DateTime time = DateTime.Now;
            string pcName = Environment.MachineName;
            string fileName = $"OrderReaderExport_{pcName}_{time.Year}_{time.Month}_{time.Day}_{time.Hour}_{time.Minute}_{time.Second}_{orders.OrderID}.pdf";
            // Load the user settings
            UserSettings settings = Settings.LoadSettings();
            string tempFilePath = System.IO.Path.Combine(Settings.TempFilesPath, fileName);

            // Try to save the file in the temp directory
            try
            {
                // Create the PDF document and the document objects
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(tempFilePath));
                // Set the page size to A4 and rotate it to Landscape mode
                pdfDoc.SetDefaultPageSize(PageSize.A4.Rotate());
                Document doc = new Document(pdfDoc);
                // Set up our fonts
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                doc.SetFont(font);
                doc.SetFontSize(12.0f);

                // Create a header
                Paragraph header = new Paragraph().Add("Order for ").Add(new Text(orders.CustomerName).SetFont(boldFont).SetFontSize(16.0f))
                    .Add(" to be delivered on ").Add(new Text(orders.Date.ToShortDateString()).SetFont(boldFont).SetFontSize(16.0f))
                    .SetFontColor(ColorConstants.DARK_GRAY).SetMarginBottom(20.0f).SetFontSize(14.0f);
                doc.Add(header);

                // Create the table
                Table table = new Table(ordersTable.Columns.Count);
                table.SetTextAlignment(TextAlignment.CENTER);

                // Add column headers
                foreach (DataColumn c in ordersTable.Columns)
                {
                    Cell cell = new Cell().Add(new Paragraph(c.ColumnName).SetPadding(5.0f)
                        .SetFont(boldFont)).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    table.AddHeaderCell(cell);
                }

                // Add all data rows
                for (int r = 0; r < ordersTable.Rows.Count; r++)
                {
                    DataRow row = ordersTable.Rows[r];
                    for (int c = 0; c < row.ItemArray.Length; c++)
                    {
                        Cell cell = new Cell().Add(new Paragraph(row.ItemArray[c].ToString()).SetPaddingLeft(10.0f).SetPaddingRight(10.0f));

                        // If we're in the last row (which contains totals), set the font to be bold
                        if (r == ordersTable.Rows.Count - 1)
                        {
                            cell.SetFont(boldFont);
                        }

                        table.AddCell(cell);
                    }
                }

                // Add our table to the document
                doc.Add(table);

                // Add a footer
                Paragraph footerTitle = new Paragraph().Add(new Text("Created using Order Reader").SetFont(boldFont).SetFontSize(18.0f)).Add(" ")
                    .Add(new Text("by Patryk Zawierucha").SetItalic()).SetFontColor(ColorConstants.GRAY);
                Paragraph footerInfo = new Paragraph().Add("Processed on ").Add(time.ToShortDateString()).Add(" at ").Add(time.ToShortTimeString())
                    .SetFontColor(ColorConstants.GRAY);

                // Set position of the footer
                float footerXPos = pdfDoc.GetPage(1).GetPageSize().GetWidth() / 2;
                float footerYPos = 40;
                // Show footer on the document
                doc.ShowTextAligned(footerTitle, footerXPos, footerYPos, TextAlignment.CENTER);
                doc.ShowTextAligned(footerInfo, footerXPos, footerYPos - 20, TextAlignment.CENTER);

                // Close the document
                doc.Close();
            }
            catch (IOException ex)
            {
                IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Error creating a file",
                    Message = $"An error occured while trying to create a PDF file:\n{ex.Message}\nOperation was aborted.",
                    ButtonText = "OK"
                });

                try
                {
                    // Delete the file if it exists
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                }
                catch (IOException IOex)
                {
                    IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        Title = "Error deleting a file",
                        Message = $"An error occured while trying to delete a PDF file:\n{IOex.Message}\nOperation was aborted.",
                        ButtonText = "OK"
                    });
                }

                return;
            }

            // Before saving the file, make sure that the export directory exists, if not then create it
            if (!Directory.Exists(settings.UserPDFExportPath)) Directory.CreateDirectory(settings.UserPDFExportPath);
            // Create the export path
            string exportPath = System.IO.Path.Combine(settings.UserPDFExportPath, fileName);
            
            // If file needs to be printed
            if (settings.PrintOrders)
            {
                PrintingManager.PrintPDF(tempFilePath);
            }

            // If PDF file should be saved, move it to export folder. Otherwise, delete it
            if (settings.ExportPDF)
            {
                // Copy the file over to the export directory
                try
                {
                    File.Move(tempFilePath, exportPath);
                }
                catch (Exception ex)
                {
                    IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        Title = "Error moving a file",
                        Message = $"An error occured while trying to move a PDF file:\n{ex.Message}\nFile was not moved.",
                        ButtonText = "OK"
                    });
                }
            }
            else
            {
                try
                {
                    File.Delete(tempFilePath);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                        {
                            Title = "Error deleting a file",
                            Message = $"An error occured while trying to delete a PDF file:\n{ex.Message}\nOperation was aborted.",
                            ButtonText = "OK"
                        });
                    }
                    else if (!(ex is DirectoryNotFoundException))
                    {
                        throw;
                    }
                }
            }
        }
    }
}
