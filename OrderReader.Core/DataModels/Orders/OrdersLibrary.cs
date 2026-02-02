using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrderReader.Core.DataModels.Orders;

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
            if (!OrderIDs.Contains(order.OrderId)) OrderIDs.Add(order.OrderId);
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
            if (order.OrderId == orderId) return true;
        }

        return false;
    }

    /// <summary>
    /// Get a list of orders with the specified order ID, sorted by depot name
    /// </summary>
    /// <param name="orderId">The ID of this order</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="Order"/> objects</returns>
    public ObservableCollection<Order> GetAllOrdersWithID(string orderId)
    {
        ObservableCollection<Order> AllOrders = new ObservableCollection<Order>();

        foreach (Order order in Orders)
        {
            if (order.OrderId == orderId) AllOrders.Add(order);
        }

        // Before returning the orders, sort them by the depot name
        return new ObservableCollection<Order>(AllOrders.OrderBy(o => o.DepotName));
    }

    /// <summary>
    /// Removes all orders with the specified order id
    /// </summary>
    /// <param name="orderId">Order ID as a <see cref="string"/></param>
    public void RemoveAllOrdersWithID(string orderId)
    {
        for (int i = Orders.Count-1; i >= 0; i--)
        {
            if (Orders[i].OrderId == orderId)
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
    /// Checks if the order passed in matches any of the orders already in the library
    /// </summary>
    /// <param name="orderIn"><see cref="Order"/> object</param>
    /// <returns>Returns true or false</returns>
    public bool HasOrder(Order orderIn)
    {
        foreach (Order order in Orders)
        {
            if (order.OrderId == orderIn.OrderId &&
                order.OrderReference == orderIn.OrderReference &&
                order.DepotId == orderIn.DepotId &&
                order.Products.Count == orderIn.Products.Count)
            {
                bool allProductsMatch = true;
                foreach (OrderProduct product1 in order.Products)
                {
                    bool productMatches = false;
                    foreach (OrderProduct product2 in orderIn.Products)
                    {
                        if (product1.ProductId == product2.ProductId && product1.Quantity == product2.Quantity)
                        {
                            productMatches = true;
                            break;
                        }
                    }
                    if (!productMatches) allProductsMatch = false;
                }
                if (allProductsMatch) return true;
            }
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