﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace OrderReader.Core
{
    /// <summary>
    /// Is responsible for exporting data into a CSV file
    /// </summary>
    public static class CSVExport
    {
        /// <summary>
        /// A function that will export the orders with provided order ID to a CSV file
        /// </summary>
        /// <param name="orderId">The orderID that is used to group orders together</param>
        public static async Task ExportOrdersToCSV(string orderId)
        {
            // Create a list of orders that we want to process and populate it
            ObservableCollection<Order> orders = IoC.Orders().GetAllOrdersWithID(orderId);

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
                // Get customer and depot for each
                Customer cust = IoC.Customers().GetCustomerByID(order.CustomerID);
                Depot depot = cust.GetDepot(order.DepotID);

                foreach (OrderProduct product in order.Products)
                {
                    // Get the price of the current product
                    decimal price = cust.GetProduct(product.ProductID).Price;
                    // Convert the price into the correct format for exporting
                    string priceString = price == 0m ? "" : string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:F2}", price);
                    // Add the line with all required information into the list of lines
                    lines.Add($"{ cust.CSVName },{ depot.CSVName },{ order.Date.ToShortDateString() },{ order.OrderReference },{ cust.GetProduct(product.ProductID).CSVName },{ product.Quantity },{ priceString },");
                }
            }

            if (lines.Count > 0)
            {
                DateTime time = DateTime.Now;
                string pcName = Environment.MachineName;
                string customerName = IoC.Customers().GetCustomerByID(orders[0].CustomerID).CSVName;
                string fileName = $"order_{pcName}_{time.Year}-{time.Month}-{time.Day}_{time.Hour}-{time.Minute}-{time.Second}_{orderId}_{customerName}.csv";

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
                    await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        Title = "CSV Export Error",
                        Message = $"An error occured while trying to export the CSV file.\n" +
                        $"Details of error:\n\n{ex.Message}" +
                        $"\n\nThis order was not processed.",
                        ButtonText = "OK"
                    });
                    throw;
                }
            }
        }
    }
}
