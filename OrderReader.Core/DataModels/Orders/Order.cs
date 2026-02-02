using System;
using System.Collections.ObjectModel;
using System.Linq;
using OrderReader.Core.DataModels.Customers;

namespace OrderReader.Core.DataModels.Orders;

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
    public string OrderId { get; private set; }

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
    public int CustomerId { get; private set; }

    /// <summary>
    /// Gets the name of the Customer
    /// </summary>
    public string CustomerName => Customer.Name;

    /// <summary>
    /// An ID of the depot
    /// </summary>
    public int DepotId { get; private set; }

    public CustomerProfile CustomerProfile { get; }
    
    public Customer Customer { get; }

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
    public Order(string orderReference, DateTime date, int customerId, int depotId, CustomerProfile customerProfile, Customer customer)
    {
        OrderReference = orderReference;
        Date = date;
        CustomerId = customerId;
        DepotId = depotId;
        CustomerProfile = customerProfile;
        Customer = customer;

        // OrderID is constructed from CustomerID and date
        OrderId = $"{CustomerId}-{Date.Year}-{Date.Month}-{Date.Day}";
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
        foreach (var line in Products)
        {
            // Look for the same product id
            if (line.ProductId == productId)
            {
                // If the same product is already on the list, just add the new quantity to the existing one
                line.Quantity += quantity;
                return;
            }
        }

        // If same product is not on this order yet, add it to the list
        Products.Add(new OrderProduct(CustomerId, productId, quantity, CustomerProfile));
        SortProducts();
    }

    /// <summary>
    /// Add a product to the order
    /// </summary>
    /// <param name="product">An <see cref="OrderProduct"/> object</param>
    public void AddProduct(OrderProduct product)
    {
        if (product.CustomerId == CustomerId)
        {
            // First iterate through the list of products already on the order
            foreach (OrderProduct line in Products)
            {
                // Look for the same product id
                if (line.ProductId == product.ProductId)
                {
                    // If same product is already on the list, just add the new quantity to the existing one
                    line.Quantity += product.Quantity;
                    return;
                }
            }

            // If same product is not on this order yet, add it to the list
            Products.Add(product);
            SortProducts();
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
            if (product.ProductId == productId) return product.Quantity;
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
    /// Gets the name of depot on this order
    /// </summary>
    /// <returns>Name of the depot</returns>
    private string GetDepotName()
    {
        return Customer.GetDepot(DepotId)?.Name ?? $"Depot not found [ID - {DepotId}]";
    }

    /// <summary>
    /// Sort the products alphabetically by name
    /// </summary>
    private void SortProducts()
    {
        Products = new ObservableCollection<OrderProduct>(Products.OrderBy(p => p.ProductName));
    }

    #endregion
}