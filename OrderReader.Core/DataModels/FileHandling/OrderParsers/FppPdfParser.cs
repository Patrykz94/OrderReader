using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling.OrderParsers;

public class FppPdfParser(INotificationService notificationService) : IParseOrder
{
    #region Public Properties

    /// <summary>
    /// The file extension that this parser works with
    /// </summary>
    public string FileExtension { get; } = ".pdf";

    #endregion

    #region Public Helpers

    public CustomerProfile? GetCustomerProfile(Dictionary<string, string[]> orderText, CustomersHandler customers)
    {
        foreach (var pageText in orderText)
        {
            // Check if any of the lines contain a customer name
            foreach (var line in pageText.Value)
            {
                var lineElements = line.Split(' ');
                if (lineElements.Length > 1)
                {
                    if (customers.HasCustomerProfileIdentifier(lineElements[^1])) return customers.GetCustomerProfile(lineElements[^1]);
                }
            }
        }

        return null;
    }

    public async Task ParseOrderAsync(Dictionary<string, string[]> orderText, string fileName, CustomerProfile customerProfile, OrdersLibrary ordersLibrary)
    {
        Customer? customer = null;
        
        foreach (var pageText in orderText)
        {
            // Check if any of the lines contain a customer name
            foreach (var line in pageText.Value)
            {
                var lineElements = line.Split(' ');
                if (lineElements.Length > 1)
                {
                    if (customerProfile.HasCustomerOrderName(lineElements[^1]))
                    {
                        customer = customerProfile.GetCustomer(lineElements[^1]);
                    }
                }
            }
        }
        
        if (customer is null)
        {
            var errorMessage = $"Could not read the customer information in file {fileName}.\n\n" +
                               "Please check the file and verify if it contains correct customer information. If it looks correct then please contact Patryk Z.\n" +
                               "\nThis file was not processed.";

            // Display an error message to the user
            await notificationService.ShowMessage("File Processing Error", errorMessage);
            return;
        }
        
        var depotNames = customer.Depots.Select(d => d.OrderName).ToList();
        
        // Iterate over the pages in extracted text
        foreach (var (page, lines) in orderText)
        {
            // Variables that will be required
            var isAllDataFound = true;
            OrderWarning? dateWarning = null;

            // Required data to extract in string form
            List<string> depotStrings = [];
            List<string> productStrings = [];
            List<List<double>> linesWithProductQuantities = [];
            
            List<Depot> depots = [];
            List<Product> products = [];

            // Product code specification
            const int productCodeMinLength = 8;
            
            // Get depot date
            var dateString = string.Empty;
            if (lines.Length > 2 && lines[2].Contains("Delivery Date"))
            {
                var dateLineElements = lines[2].Split(' ');
                if (dateLineElements.Length > 5)
                {
                    dateString = dateLineElements[5];
                }
            }
            var dateFound = DateTime.TryParse(dateString, out var deliveryDate);

            var combinedProductsAndQuantities = false;
            
            // Iterate over the lines to find required data
            foreach (var line in lines)
            {
                // Check if the line contains any of the depot names
                var depotName = depotNames.FirstOrDefault(d => line.Contains(d));
                if (depotName is not null)
                {
                    depotStrings.Add(depotName);
                    continue;
                }
                
                // Split this line into strings separated by spaces
                var lineElements = line.Split(' ');

                var productFound = false;
                // Check if the line contains any of the product codes
                for (var i = 0; i < lineElements.Length; i++)
                {
                    if (lineElements[i].Length >= productCodeMinLength && lineElements[i].All(char.IsDigit))
                    {
                        productFound = true;
                        productStrings.Add(lineElements[i]);

                        if (i < lineElements.Length - 2)
                        {
                            var subList = lineElements.Skip(i + 1).Take(lineElements.Length - i - 1).ToList();
                            
                            // Check if the line contains product quantities
                            if (subList.Count > 1 && subList.All(x => x.All(char.IsDigit)))
                            {
                                combinedProductsAndQuantities = true;
                                var productQuantities = subList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
                                linesWithProductQuantities.Add(productQuantities);
                            }
                        }
                        break;
                    }
                }
                
                if (productFound) continue;

                if (lineElements.Length > 2 && lineElements[0] == "Total")
                {
                    var subList = lineElements.Skip(1).Take(lineElements.Length - 1).ToList();
                    
                    // Check if the line contains product quantities
                    if (subList.Count > 1 && subList.All(x => x.All(char.IsDigit)))
                    {
                        productFound = true;
                        var productQuantities = subList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
                        linesWithProductQuantities.Add(productQuantities);
                    }
                }
                
                if (productFound) continue;
                
                // Check if the line contains product quantities
                if (lineElements.Length > 1 && lineElements.All(x => x.All(char.IsDigit)))
                {
                    var productQuantities = lineElements.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
                    linesWithProductQuantities.Add(productQuantities);
                }
            }

            if (!combinedProductsAndQuantities)
            {
                // Reverse the order of depots and products as for some reason they both appear in the opposite order when reading text from the file
                depotStrings.Reverse();
                productStrings.Reverse();
            }

            // Make sure that we have found all necessary data
            if (depotStrings.Count == 0 || productStrings.Count == 0 || linesWithProductQuantities.Count == 0 ||
                linesWithProductQuantities.Count != productStrings.Count + 1 ||
                linesWithProductQuantities.Any(x => x.Count != depotStrings.Count + 1))
            {
                var errorMessage = $"Following issues have been found in file {fileName}:\n";
                if (!dateFound) errorMessage += "* Delivery date was not found\n";
                if (depotStrings.Count == 0) errorMessage += "* No depot name was found\n";
                if (productStrings.Count == 0) errorMessage += "* No products were found\n";
                if (linesWithProductQuantities.Count == 0) errorMessage += "* Product quantity was not found\n";
                if (productStrings.Count > 0 && linesWithProductQuantities.Count > 0 &&
                    linesWithProductQuantities.Count != productStrings.Count + 1) errorMessage += "* Failed to match products with their quantities\n";
                if (depotStrings.Count > 0 && linesWithProductQuantities.Count > 0)
                {
                    foreach (var line in linesWithProductQuantities)
                    {
                        if (line.Count != depotStrings.Count + 1) errorMessage += "* Failed to match depots with their quantities\n";
                        break;
                    }
                }
                errorMessage += "\nThis file was not processed.\n";
                errorMessage += "If you think this file has all the correct data, please contact Patryk Z.";
                isAllDataFound = false;
                
                // Display an error message to the user
                await notificationService.ShowMessage("File Processing Error", errorMessage);
            }
            else
            {
                // Validate the delivery date
                if (deliveryDate != DateTime.Today.AddDays(1))
                {
                    var errorMessage = $"Delivery date ({deliveryDate.ToShortDateString()}) in file {fileName} on page {page} is not tomorrow.";

                    // Create a new warning object for this order
                    dateWarning = new OrderWarning(OrderWarning.WarningType.UnusualDate, errorMessage);

                    // Display an error message to the user
                    await notificationService.ShowMessage("Unusual Date Warning", errorMessage);
                }

                // Get all depot objects from strings
                depots.AddRange(depotStrings.Select(depotString => customer.GetDepot(depotString)));

                // Get all product objects from strings
                foreach (var product in productStrings)
                {
                    // Make sure that this product exists
                    if (!customerProfile.HasProductOrderName(product))
                    {
                        var errorMessage = $"Unknown product ({product}) in file {fileName}.\n" +
                                           "If this is a new product that Order Reader doesn't know about yet, please add it to Order Reader and try processing this file again. Otherwise, please contact Patryk Z." +
                                           "This file was not processed.";

                        isAllDataFound = false;

                        // Display an error message to the user
                        await notificationService.ShowMessage("Unknown Product Error", errorMessage);
                        break;
                    }

                    // Get the product object
                    products.Add(customerProfile.GetProduct(product));
                }
                
                // Make sure the total product quantities are correct
                for (var pLine = 0; pLine < linesWithProductQuantities.Count - 1; pLine++)
                {
                    var productTotal = linesWithProductQuantities[pLine][^1];
                    if (Math.Abs(linesWithProductQuantities[pLine].Sum() - productTotal * 2) > 0.01)
                    {
                        var errorMessage = $"Total product quantity is not equal to sum of all quantities in file {fileName}.\n" +
                                           "Please check the PDF file to see if all product quantities add up to the total amount. " +
                                           "If they do, please contact Patryk Z.\n" +
                                           "This file was not processed.";
                    
                        isAllDataFound = false;
                    
                        // Display an error message to the user
                        await notificationService.ShowMessage("Order Processing Error", errorMessage);
                        break;
                    }
                }
                
                // Make sure the total depot quantities are correct
                for (var dLine = 0; dLine < linesWithProductQuantities[0].Count - 1; dLine++)
                {
                    var depotTotal = linesWithProductQuantities[^1][dLine];
                    var depotSum = 0.0;
                    for (var pLine = 0; pLine < linesWithProductQuantities.Count - 1; pLine++)
                    {
                        depotSum += linesWithProductQuantities[pLine][dLine];
                    }
                    if (Math.Abs(depotSum - depotTotal) > 0.01)
                    {
                        var errorMessage = $"Total depot quantity is not equal to sum of all quantities in file {fileName}.\n" +
                                           "Please check the PDF file to see if all depot quantities add up to the total amount. " +
                                           "If they do, please contact Patryk Z.\n" +
                                           "This file was not processed.";
                    
                        isAllDataFound = false;
                    
                        // Display an error message to the user
                        await notificationService.ShowMessage("Order Processing Error", errorMessage);
                        break;
                    }
                }
            }
            
            if (!isAllDataFound) continue;
            for (var d = 0; d < depots.Count; d++)
            {
                // Get the depot object
                var depot = depots[d];
                
                // Create a new order
                var order = new Order("N/A", deliveryDate, customer.Id, depot.Id, customerProfile, customer);

                for (var p = 0; p < products.Count; p++)
                {
                    // Get the product object
                    var product = products[p];
                    
                    // Get the product quantity
                    var productQuantity = linesWithProductQuantities[p][d];
                    if (productQuantity == 0.0) continue;
                    
                    // Create a new order product
                    var orderProduct = new OrderProduct(customer.Id, product.Id, productQuantity, customerProfile);
                    order.AddProduct(orderProduct);
                }
                
                if (dateWarning is not null) order.AddWarning(dateWarning);
                
                // Check if the same order already exists
                if (ordersLibrary.HasOrder(order))
                {
                    var errorMessage = $"The order in file {fileName} could not be processed. The same order already exists.\n.";

                    // Display an error message to the user
                    await notificationService.ShowMessage("Order Processing Error", errorMessage);
                }
                else
                {
                    // Add this order to the list
                    ordersLibrary.AddOrder(order);
                }
            }
        }
    }

    #endregion
}