using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;

namespace OrderReader.Core.DataModels.FileHandling.OrderParsers;

public class KeelingsPDFParser(INotificationService notificationService) : IParseOrder
{
    #region Private Variables

    private readonly INotificationService _notificationService = notificationService;

    #endregion
        
    #region Public Properties

    /// <summary>
    /// The file extention that this parser works with
    /// </summary>
    public string FileExtension { get; } = ".pdf";

    #endregion

    #region Public Helpers

    public Customer GetCustomer(Dictionary<string, string[]> orderText, CustomersHandler customers)
    {
        foreach (KeyValuePair<string, string[]> pageText in orderText)
        {
            // Check if any of the lines contain a customer name
            foreach (string line in pageText.Value)
            {
                if (customers.HasCustomerOrderName(line)) return customers.GetCustomerByOrderName(line);
            }
        }

        return null;
    }

    public async Task ParseOrderAsync(Dictionary<string, string[]> orderText, string fileName, Customer customer, OrdersLibrary ordersLibrary)
    {
        // Iterate over the pages in extracted text
        foreach (KeyValuePair<string, string[]> pageText in orderText)
        {
            // A string array that will contain strings from the current page
            string[] lines = pageText.Value;

            // Variables that will be required
            bool isAllDataFound = true;
            double unknownProductQuantity = 0.0;
            List<OrderWarning> warnings = new List<OrderWarning>();

            // Required data to extract in string form
            string depotString = null;
            string dateString = null;
            string orderTotalString = null;
            string orderReference = null;
            Dictionary<string, string> productStrings = new Dictionary<string, string>();

            // Required data
            Depot depot = null;
            DateTime deliveryDate = DateTime.MinValue;
            double orderTotal = 0.0;
            List<OrderProduct> products = new List<OrderProduct>();

            // Product code specification
            int productCodeMinLength = 7;

            // Iterate over the lines to find required data
            for (int i = 0; i < lines.Length; i++)
            {
                // Look for depot name if not already found
                if (depotString == null)
                {
                    if (customer.HasDepotOrderName(lines[i]))
                        depotString = lines[i];
                }

                // Look for order reference if not already found
                if (orderReference == null)
                {
                    string orderReferenceTitle = "Supplier PO Number:";
                    if (lines[i] == orderReferenceTitle && lines.Length > i + 4)
                        orderReference = lines[i+4];  
                }

                // Look for delivery date if not already found
                if (dateString == null)
                {
                    string deliveryDateTitle = "Delivery Date: ";
                    if (lines[i].StartsWith(deliveryDateTitle))
                        dateString = lines[i].Substring(deliveryDateTitle.Length);
                }

                // Look for total boxes if not already found
                if (orderTotalString == null)
                {
                    if (lines[i] == "Total Qty:" && i > 1)
                        orderTotalString = lines[i - 2];
                }

                // Look for products
                if (lines[i].Length > productCodeMinLength)
                {
                    string[] lineElements = lines[i].Split(' ');
                    int elementCount = lineElements.Length;

                    // Last element should be the product number
                    if (elementCount > 3 && lineElements[elementCount - 1].Length >= productCodeMinLength && lineElements[elementCount - 1].All(char.IsDigit))
                    {
                        // The third to last element should be the quantity
                        if (double.TryParse(lineElements[elementCount - 3], NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double result))
                        {
                            string productNumber = lineElements[elementCount - 1];
                            string productQuantity = lineElements[elementCount - 3];

                            productStrings.Add(productNumber, productQuantity);
                        }
                    }
                }
            }

            // Make sure that we have found all necessary data
            if (depotString == null || orderReference == null || dateString == null || orderTotalString == null || productStrings.Count == 0)
            {
                string errorMessage = $"Following issues have been found in file {fileName}:\n";
                if (depotString == null) errorMessage += "* Depot name was not found (that likely means that it's a new depot that Order Reader doesn't know about yet, please manually check and add a new depot if required)\n";
                if (orderReference == null) errorMessage += "* PO reference number was not found\n";
                if (dateString == null) errorMessage += "* Delivery date was not found\n";
                if (orderTotalString == null) errorMessage += "* Total quantity was not found\n";
                if (productStrings.Count == 0) errorMessage += "* No products were found\n";
                errorMessage += "\nThis file was not processed.\n";
                errorMessage += "If you think this file has all the correct data, please contact Patryk Z.";
                isAllDataFound = false;

                // Display error message to the user
                await _notificationService.ShowMessage("File Processing Error", errorMessage);
            }
            else
            {
                // Process and validate delivery date
                if (!DateTime.TryParse(dateString.Trim(), out deliveryDate))
                {
                    string errorMessage = $"Could not read the date from file {fileName}.\n" +
                                          "Please check the date in PDF file. If the date format looks correct then please contact Patryk Z.\n" +
                                          "\nThis file was not processed.";
                    isAllDataFound = false;

                    // Display error message to the user
                    await _notificationService.ShowMessage("File Processing Error", errorMessage);
                }
                else if (deliveryDate != DateTime.Today.AddDays(1))
                {
                    string errorMessage = $"Delivery date ({deliveryDate.ToShortDateString()}) in file {fileName} on page {pageText.Key} is not tomorrow.";

                    // Create a new warning object for this order
                    warnings.Add(new OrderWarning(OrderWarning.WarningType.UnusualDate, errorMessage));

                    // Display error message to the user
                    await _notificationService.ShowMessage("Unusual Date Warning", errorMessage);
                }

                // Process and validate all products
                foreach (var product in productStrings)
                {
                    // Make sure that this product exists
                    if (!customer.HasProductOrderName(product.Key))
                    {
                        // If the quantity of unknown product can be parsed then we can add this product and just display a warning
                        // Otherwise, we can not process this order
                        if (double.TryParse(product.Value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double quantity))
                        {
                            string errorMessage = $"Unknown product ({product.Key}) in file {fileName}.\n" +
                                                  "You can process this order without that product if you intend to add it manually later or if you think it's been put on by mistake.";

                            // Add the quantity to unknown product quantity
                            unknownProductQuantity += quantity;

                            // Create a warning object
                            warnings.Add(new OrderWarning(OrderWarning.WarningType.UnknownProduct, errorMessage));

                            // Display error message to the user
                            await _notificationService.ShowMessage("Unknown Product Warning", errorMessage);
                        }
                        else
                        {
                            string errorMessage = $"Unknown product ({product.Key}) in file {fileName}.\n" +
                                                  "If this is a new product that Order Reader doesn't know about yet, please add it to Order Reader and try processing this file again. Otherwise, please contact Patryk Z." +
                                                  "This file was not processed.";

                            isAllDataFound = false;

                            // Display error message to the user
                            await _notificationService.ShowMessage("Unknown Product Error", errorMessage);
                        }
                    }
                    else
                    {
                        // Get the product quantity
                        if (double.TryParse(product.Value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double quantity))
                        {
                            OrderProduct orderProduct = new OrderProduct(customer.Id, customer.GetProduct(product.Key).Id, quantity, customer);
                            products.Add(orderProduct);
                        }
                        else
                        {
                            string errorMessage = $"Could not read product quantity ({product.Key}) from file {fileName}.\n" +
                                                  "Please check the product quantity in PDF file. If the quantity looks correct then please contact Patryk Z.\n" +
                                                  "This file was not processed.";

                            isAllDataFound = false;

                            // Display error message to the user
                            await _notificationService.ShowMessage("Order Processing Error", errorMessage);
                        }
                    }
                }

                // Validate the total quantity from PDF file
                if (!double.TryParse(orderTotalString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out orderTotal))
                {
                    string errorMessage = $"Could not read the total quantity from file {fileName}.\n" +
                                          "Please check the total quantity in PDF file. If the quantity looks correct then please contact Patryk Z.\n" +
                                          "This file was not processed.";

                    isAllDataFound = false;

                    // Display error message to the user
                    await _notificationService.ShowMessage("Order Processing Error", errorMessage);
                }

                // Make sure we have valid products to add
                if (products.Count == 0)
                {
                    string errorMessage = $"No valid products foud in file {fileName}.\n" +
                                          "This file was not processed.";

                    isAllDataFound = false;

                    // Display error message to the user
                    await _notificationService.ShowMessage("Order Processing Error", errorMessage);
                }

                // Create the depot object
                depot = customer.GetDepot(depotString);

                // If we found all required data, create the order
                if (isAllDataFound)
                {
                    // Create a  new order object
                    Order order = new Order(orderReference, deliveryDate, customer.Id, depot.Id, customer);

                    // Add products to this order
                    foreach (OrderProduct product in products)
                    {
                        order.AddProduct(product);
                    }

                    // Add warnings to this order
                    foreach (OrderWarning warning in warnings)
                    {
                        order.AddWarning(warning);
                    }

                    // Make sure that the total product quantity is correct
                    if (order.GetTotalProductQuantity() == orderTotal || order.GetTotalProductQuantity() == orderTotal - unknownProductQuantity)
                    {
                        // Check if the same order already exists
                        if (ordersLibrary.HasOrder(order) || ordersLibrary.HasOrderWithSameReference(order))
                        {
                            string errorMessage = $"The order in file {fileName} could not be processed. An order with the same reference number already exists.";

                            // Display error message to the user
                            await _notificationService.ShowMessage("Order Processing Error", errorMessage);
                        }
                        else
                        {
                            // Add this order to the list
                            ordersLibrary.AddOrder(order);
                        }
                    }
                    else
                    {
                        string errorMessage = $"Total field not equal to sum of all product quantities in file {fileName}.\n" +
                                              "Please check the PDF file to see if all product quantities add up to the total amount. " +
                                              "If they do, please contact Patryk Z.\n" +
                                              "This file was not processed.";

                        // Display error message to the user
                        await _notificationService.ShowMessage("Order Processing Error", errorMessage);
                    }
                }
            }
        }
    }

    #endregion
}