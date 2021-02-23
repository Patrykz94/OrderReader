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

        /// <summary>
        /// Removes all orders with the specified order id
        /// </summary>
        /// <param name="orderId">Order ID as a <see cref="string"/></param>
        public void RemoveAllOrdersWithID(string orderId)
        {
            for (int i = Orders.Count-1; i >= 0; i--)
            {
                if (Orders[i].OrderID == orderId)
                {
                    Orders.Remove(Orders[i]);
                }
            }
        }

        /// <summary>
        /// Removes the specified order object from the list of orders
        /// </summary>
        /// <param name="orderToRemove"><see cref="Order"/> object</param>
        public void RemoveOrder(Order orderToRemove)
        {
            for (int i = Orders.Count-1; i >= 0; i--)
            {
                if (Orders[i] == orderToRemove)
                {
                    Orders.Remove(Orders[i]);
                }
            }
        }

        /// <summary>
        /// Simple function that adds the order to the list of orders
        /// </summary>
        /// <param name="orderToAdd"><see cref="Order"/> object</param>
        public void AddOrder(Order orderToAdd)
        {
            Orders.Add(orderToAdd);
        }

        /// <summary>
        /// Checks of the order passed in matches any of the orders already in the library
        /// </summary>
        /// <param name="orderIn"><see cref="Order"/> object</param>
        /// <returns>Returns true or false</returns>
        public bool HasOrder(Order orderIn)
        {
            foreach (Order order in Orders)
            {
                if (order == orderIn)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if an order with the same reference number already exists
        /// </summary>
        /// <param name="orderIn">An <see cref="Order"/> object</param>
        /// <returns><see cref="bool"/> whether or not the order exists</returns>
        public bool HasOrderWithSameReference(Order orderIn)
        {
            foreach (Order order in Orders)
            {
                if (order.OrderReference == orderIn.OrderReference)
                    return true;
            }
            return false;
        }

        #endregion
    }
}
