using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling;

/// <summary>
/// Is responsible for exporting data into a CSV file
/// </summary>
public static class CSVExport
{
    public static INotificationService NotificationService { get; set; }
        
    // A function that will export the orders with provided order ID to a CSV file
    public static async Task ExportOrdersToCSV(ObservableCollection<Order> orders, Customer customer)
    {
        // Make sure that list is not empty
        if (orders.Count == 0) return;

        // Create a list of strings which will represent lines on csv files
        List<string> lines = new List<string>
        {
            // Add the row of headings
            "Customer,Branch,Required Date,Customer Reference,Product Code,Order Qty,Price,"
        };

        // Iterate over the list of orders that need to be printed
        foreach (Order order in orders)
        {
            // Get depot for each order
            Depot depot = customer.GetDepot(order.DepotID);

            foreach (OrderProduct product in order.Products)
            {
                // Get the price of the current product
                decimal price = customer.GetProduct(product.ProductID).Price;
                // Convert the price into the correct format for exporting
                string priceString = price == 0m ? "" : string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:F2}", price);
                // Add the line with all required information into the list of lines
                lines.Add($"{ customer.CSVName },{ depot.CSVName },{ order.Date.ToShortDateString() },{ order.OrderReference },{ customer.GetProduct(product.ProductID).CSVName },{ product.Quantity },{ priceString },");
            }
        }

        if (lines.Count > 0)
        {
            DateTime time = DateTime.Now;
            string pcName = Environment.MachineName;
            string customerName = customer.CSVName;
            string fileName = $"order_{pcName}_{time.Year}-{time.Month}-{time.Day}_{time.Hour}-{time.Minute}-{time.Second}_{orders[0].OrderID}_{customerName}.csv";

            UserSettings settings = Settings.LoadSettings();

            try
            {
                // Before saving the file, make sure that the export path exists, if not then create it
                if (!Directory.Exists(settings.UserCSVExportPath)) Directory.CreateDirectory(settings.UserCSVExportPath);

                // Create the file
                File.WriteAllLines($"{settings.UserCSVExportPath}\\{fileName}", lines);
            }
            catch (Exception ex)
            {
                // If an error occurs, we want to show the error message
                await NotificationService.ShowMessage(
                    "CSV Export Error",
                    $"An error occured while trying to export the CSV file.\n" +
                    $"Details of error:\n\n{ex.Message}" +
                    $"\n\nThis order was not processed.");
                throw;
            }
        }
    }
}