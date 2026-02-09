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
public static class CsvExport
{
    public static INotificationService NotificationService { get; set; }
        
    // A function that will export the orders with provided order ID to a CSV file
    public static async Task ExportOrdersToCsv(ObservableCollection<Order> orders, CustomerProfile customerProfile, Customer customer)
    {
        // Make sure that list is not empty
        if (orders.Count == 0) return;

        // Create a list of strings which will represent lines on csv files
        List<string> lines = ["Customer,Branch,Required Date,Customer Reference,Product Code,Order Qty,Price,"];

        // Iterate over the list of orders that need to be printed
        foreach (var order in orders)
        {
            // Get a depot for each order
            var depot = customer.GetDepot(order.DepotId);

            foreach (var orderProduct in order.Products)
            {
                var product = customerProfile.GetProduct(orderProduct.ProductId);
                
                if (product == null)
                    throw new Exception($"Could not find product with ID {orderProduct.ProductId}");
                
                // Try to get the price of the product from the order, otherwise from the product itself
                var price = (decimal)double.Round(orderProduct.Price, 2);
                if (price == 0m) price = product.Price;
                
                // Convert the price into the correct format for exporting
                var priceString = price == 0m ? "" : string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:F2}", price);
                // Add the line with all required information into the list of lines
                lines.Add($"{ customer.CsvName },{ depot?.CsvName },{ order.Date.ToShortDateString() },{ order.OrderReference },{ product.CsvName },{ orderProduct.Quantity },{ priceString },");
            }
        }

        if (lines.Count > 0)
        {
            var time = DateTime.Now;
            var pcName = Environment.MachineName;
            var customerName = customer.CsvName;
            var fileName = $"order_{pcName}_{time.Year}-{time.Month}-{time.Day}_{time.Hour}-{time.Minute}-{time.Second}_{orders[0].OrderId}_{customerName}.csv";

            var settings = Settings.LoadSettings();

            try
            {
                // Before saving the file, make sure that the export path exists, if not then create it
                if (!Directory.Exists(settings.UserCsvExportPath)) Directory.CreateDirectory(settings.UserCsvExportPath);

                // Create the file
                await File.WriteAllLinesAsync($"{settings.UserCsvExportPath}\\{fileName}", lines);
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