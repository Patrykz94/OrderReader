using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OrderReader.Core
{
    /// <summary>
    /// A class that stores all orders
    /// </summary>
    public class OrdersLibrary
    {
        #region Public Properties

        public ObservableCollection<Order> Orders { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrdersLibrary()
        {
            Orders = new ObservableCollection<Order>();

            // TODO: Remove the test code below
            Order order = new Order("PO123456", DateTime.Today.AddDays(1.0), 1, 4);
            order.AddProduct(1, 20.0);
            order.AddProduct(2, 15.0);
            order.AddProduct(3, 45.0);
            order.AddProduct(2, 50.0);

            Order order2 = new Order("PO654321", DateTime.Today.AddDays(1.0), 1, 6);
            order2.AddProduct(1, 25.0);
            order2.AddProduct(2, 15.0);

            Order order3 = new Order("PO24680", DateTime.Today.AddDays(1.0), 2, 38);
            order3.AddProduct(4, 25.0);

            Order order4 = new Order("PO08642", DateTime.Today.AddDays(2.0), 1, 8);
            order4.AddProduct(1, 25.0);
            order4.AddProduct(2, 15.0);

            Orders.Add(order);
            Orders.Add(order2);
            Orders.Add(order3);
            Orders.Add(order4);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Get a list of unique order IDs
        /// </summary>
        /// <returns>A list of strings</returns>
        public List<string> GetUniqueOrderIDs()
        {
            List<string> OrderIDs = new List<string>();

            foreach (Order order in Orders)
            {
                if (!OrderIDs.Contains(order.OrderID)) OrderIDs.Add(order.OrderID);
            }

            return OrderIDs;
        }

        /// <summary>
        /// Check if an order with the specified order ID exists
        /// </summary>
        /// <param name="orderId">The ID of an order</param>
        /// <returns>Returns true or false</returns>
        public bool HasOrdersWithID(string orderId)
        {
            foreach (Order order in Orders)
            {
                if (order.OrderID == orderId) return true;
            }

            return false;
        }

        /// <summary>
        /// Get a list of orders with the specified order ID
        /// </summary>
        /// <param name="orderId">The ID of this order</param>
        /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="Order"/> objects</returns>
        public ObservableCollection<Order> GetAllOrdersWithID(string orderId)
        {
            ObservableCollection<Order> AllOrders = new ObservableCollection<Order>();

            foreach (Order order in Orders)
            {
                if (order.OrderID == orderId) AllOrders.Add(order);
            }

            return AllOrders;
        }

        #endregion
    }
}
