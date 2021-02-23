﻿using System;
using System.Collections.ObjectModel;

namespace OrderReader.Core
{
    /// <summary>
    /// A class that represents a single customer order for one depot
    /// </summary>
    public class Order
    {
        #region Public Properties

        /// <summary>
        /// The ID of this order
        /// This is mainly used to group orders together by customer and date
        /// </summary>
        public string OrderID { get; private set; }

        /// <summary>
        /// The reference number for the order
        /// </summary>
        public string OrderReference { get; private set; }

        /// <summary>
        /// A delivery date for this order
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// An ID of the customer
        /// </summary>
        public int CustomerID { get; private set; }

        /// <summary>
        /// Gets the name of the Customer
        /// </summary>
        public string CustomerName => GetCustomerName();

        /// <summary>
        /// An ID of the depot
        /// </summary>
        public int DepotID { get; private set; }

        /// <summary>
        /// Gets the name of the Depot
        /// </summary>
        public string DepotName => GetDepotName();

        /// <summary>
        /// A list of all products ordered
        /// </summary>
        public ObservableCollection<OrderProduct> Products { get; private set; } = new ObservableCollection<OrderProduct>();

        /// <summary>
        /// A list of warnings for this order
        /// </summary>
        public ObservableCollection<OrderWarning> Warnings { get; private set; } = new ObservableCollection<OrderWarning>();

        /// <summary>
        /// Whether there are any warnings for this order
        /// </summary>
        public bool HasWarnings => Warnings.Count > 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="orderReference">The customer reference number</param>
        /// <param name="date">The required delivery date</param>
        /// <param name="customerId">Id of the customer</param>
        /// <param name="depotId">Id of the customer depot</param>
        public Order(string orderReference, DateTime date, int customerId, int depotId)
        {
            OrderReference = orderReference;
            Date = date;
            CustomerID = customerId;
            DepotID = depotId;

            // OrderID is constructed from CustomerID and date
            OrderID = $"{CustomerID}-{Date.Year}-{Date.Month}-{Date.Day}";
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Add a product to the order
        /// </summary>
        /// <param name="productId">Id number of the product</param>
        /// <param name="quantity">Quantity to add</param>
        public void AddProduct(int productId, double quantity)
        {
            // First iterate through the list of products already on the order
            foreach (OrderProduct line in Products)
            {
                // Look for the same product id
                if (line.ProductID == productId)
                {
                    // If same product is already on the list, just add the new quantity to the existing one
                    line.Quantity += quantity;
                    return;
                }
            }

            // If same product is not on this order yet, add it to the list
            Products.Add(new OrderProduct(CustomerID, productId, quantity));
        }

        /// <summary>
        /// Add a product to the order
        /// </summary>
        /// <param name="product">An <see cref="OrderProduct"/> object</param>
        public void AddProduct(OrderProduct product)
        {
            if (product.CustomerID == CustomerID)
            {
                // First iterate through the list of products already on the order
                foreach (OrderProduct line in Products)
                {
                    // Look for the same product id
                    if (line.ProductID == product.ProductID)
                    {
                        // If same product is already on the list, just add the new quantity to the existing one
                        line.Quantity += product.Quantity;
                        return;
                    }
                }

                // If same product is not on this order yet, add it to the list
                Products.Add(product);
            }
        }

        /// <summary>
        /// Add a warning to the list
        /// </summary>
        /// <param name="warning">A <see cref="OrderWarning"/> object</param>
        public void AddWarning(OrderWarning warning)
        {
            Warnings.Add(warning);
        }

        /// <summary>
        /// Get ordered quantity of specified product
        /// </summary>
        /// <param name="productId">The ID number of the product</param>
        /// <returns>Ordered quantity</returns>
        public double GetQuantityOfProduct(int productId)
        {
            foreach (OrderProduct product in Products)
            {
                if (product.ProductID == productId) return product.Quantity;
            }

            return 0.0;
        }

        /// <summary>
        /// Gets the total quantity of all products on this order
        /// </summary>
        /// <returns>Total ordered quantity</returns>
        public double GetTotalProductQuantity()
        {
            double total = 0.0;

            foreach (OrderProduct product in Products)
            {
                total += product.Quantity;
            }

            return total;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the name of customer on this order
        /// </summary>
        /// <returns>Name of the customer</returns>
        private string GetCustomerName()
        {
            return IoC.Customers().HasCustomer(CustomerID) ? IoC.Customers().GetCustomerByID(CustomerID).Name : $"Customer not found [ID - {CustomerID}]";
        }

        /// <summary>
        /// Gets the name of depot on this order
        /// </summary>
        /// <returns>Name of the depot</returns>
        private string GetDepotName()
        {
            if (IoC.Customers().HasCustomer(CustomerID))
                return IoC.Customers().GetCustomerByID(CustomerID).HasDepot(DepotID) ? IoC.Customers().GetCustomerByID(CustomerID).GetDepot(DepotID).Name : $"Depot not found [ID - {DepotID}]";
            return $"Customer not found [ID - {DepotID}]";
        }

        #endregion
    }
}
