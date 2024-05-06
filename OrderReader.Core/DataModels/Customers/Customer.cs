using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OrderReader.Core.DataModels.Customers;

/// <summary>
/// The customer class that will hold information about the customer, as well as it's depots and products
/// </summary>
public class Customer
{
    #region Public Properties

    /// <summary>
    /// Unique Customer ID number
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Name of the customer, this is what's displayed in the application UI
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The name that will appear on the CSV file
    /// </summary>
    public string CSVName { get; private set; }

    /// <summary>
    /// The name that will appear on the orders that we are reading from
    /// </summary>
    public string OrderName { get; private set; }

    /// <summary>
    /// List of depots for this customer
    /// </summary>
    public ObservableCollection<Depot> Depots { get; } = new ObservableCollection<Depot>();

    /// <summary>
    /// List of products for this customer
    /// </summary>
    public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    public Customer() { }

    /// <summary>
    /// A constructor that creates the customer and optionally depots and products (if they are provided)
    /// </summary>
    /// <param name="id">The Id numer of this customer</param>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    /// <param name="depots">A list of depots that belong to this customer</param>
    /// <param name="products">A list of products that belong to this customer</param>
    public Customer(int id, string name, string csvName, string orderName, ObservableCollection<Depot> depots = null, ObservableCollection<Product> products = null)
    {
        Id = id;
        Name = name;
        CSVName = csvName;
        OrderName = orderName;

        // If populated list of depots is provided, add it
        if (depots != null) Depots = depots;

        // If populated list of products is provided, add it
        if (products != null) Products = products;
    }

    #endregion

    #region Public Helpers

    /// <summary>
    /// Updates the details of this customer
    /// </summary>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    public void Update(string name, string csvName, string orderName)
    {
        Name = name;
        CSVName = csvName;
        OrderName = orderName;
    }

    /// <summary>
    /// A function that checks if a product with this id already exists for this customer
    /// </summary>
    /// <param name="id">Id number of the product</param>
    /// <returns><see cref="bool"/> whether the products already exists or not</returns>
    public bool HasProduct(int id)
    {
        foreach (Product product in Products)
        {
            if (product.Id == id) return true;
        }

        return false;
    }

    /// <summary>
    /// A function that checks if a product with this name already exists for this customer
    /// </summary>
    /// <param name="name">Name of the product</param>
    /// <returns><see cref="bool"/> whether the products already exists or not</returns>
    public bool HasProductName(string name)
    {
        foreach (Product product in Products)
        {
            if (product.Name == name) return true;
        }

        return false;
    }

    /// <summary>
    /// A function that checks if a product with this order name exists for this customer
    /// </summary>
    /// <param name="orderName">Order Name of the product</param>
    /// <returns><see cref="bool"/> whether the products exists or not</returns>
    public bool HasProductOrderName(string orderName)
    {
        foreach (Product product in Products)
        {
            if (product.OrderName == orderName) return true;
        }

        return false;
    }

    /// <summary>
    /// A function that checks if a product with this CSVName and OrderName already exists for this customer
    /// </summary>
    /// <param name="product">A <see cref="Product"/> object to compare</param>
    /// <returns><see cref="bool"/> whether the product already exists or not</returns>
    public bool SameProductExists(Product product)
    {
        foreach (Product ourProduct in Products)
        {
            if (ourProduct.CSVName == product.CSVName && ourProduct.OrderName == product.OrderName) return true;
        }

        return false;
    }

    /// <summary>
    /// Add a new product to the list of products if it doesn't already exist
    /// </summary>
    /// <param name="id">Id number of the product</param>
    /// <param name="customerId">Id number of this customer</param>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    /// <param name="price">Price of this product</param>
    /// <returns><see cref="bool"/> false if the product already exists</returns>
    public bool AddProduct(int id, string name, string csvName, string orderName, decimal price = 0.0m)
    {
        // Make sure to check whether this product exists first
        // We don't want duplicate products
        if (!HasProduct(id) && !HasProductName(name) && !HasProductOrderName(orderName))
        {
            Products.Add(new Product(id, Id, name, csvName, orderName, price));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Add a new product to the list of products if it doesn't already exist
    /// </summary>
    /// <param name="product">A <see cref="Product"/> object</param>
    /// <returns><see cref="bool"/> false if the product already exists</returns>
    public bool AddProduct(Product product)
    {
        // Make sure to check whether this product exists first
        // We don't want duplicate products
        if (!HasProduct(product.Id) && !HasProductName(product.Name) && !HasProductOrderName(product.OrderName))
        {
            Products.Add(product);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Add a list of products if it doesn't already exist
    /// </summary>
    /// <param name="Products">A list of <see cref="Product"/></param>
    public void AddProducts(List<Product> Products)
    {
        foreach (Product product in Products)
        {
            // Make sure to check whether this product exists first
            // We don't want duplicate products
            if (!HasProduct(product.Id) && !HasProductName(product.Name) && !HasProductOrderName(product.OrderName))
            {
                Products.Add(product);
            }
        }
    }

    /// <summary>
    /// Delets a product from this customer
    /// </summary>
    /// <param name="id">Id number of this product</param>
    public void DeleteProduct(int id)
    {
        foreach (Product product in Products)
        {
            if (product.Id == id)
            {
                Products.Remove(product);
                return;
            }
        }
    }

    /// <summary>
    /// Gets the product by it's id
    /// </summary>
    /// <param name="id">Id of this product</param>
    /// <returns><see cref="Product"/> object</returns>
    public Product GetProduct(int id)
    {
        foreach (Product product in Products)
        {
            if (product.Id == id) return product;
        }

        // If the product was not found, return null
        return null;
    }

    /// <summary>
    /// Gets the product by it's order name
    /// </summary>
    /// <param name="orderName">Order Name of this product</param>
    /// <returns><see cref="Product"/> object</returns>
    public Product GetProduct(string orderName)
    {
        foreach (Product product in Products)
        {
            if (product.OrderName == orderName) return product;
        }

        // If the product was not found, return null
        return null;
    }

    /// <summary>
    /// A function that checks if a depot with this id already exists for this customer
    /// </summary>
    /// <param name="id">Id of the depot</param>
    /// <returns><see cref="bool"/> whether the depot already exists or not</returns>
    public bool HasDepot(int id)
    {
        foreach (Depot depot in Depots)
        {
            if (depot.Id == id) return true;
        }

        return false;
    }

    /// <summary>
    /// A function that checks if a depot with this name already exists for this customer
    /// </summary>
    /// <param name="name">Name of the depot</param>
    /// <returns><see cref="bool"/> whether the depot already exists or not</returns>
    public bool HasDepotName(string name)
    {
        foreach (Depot depot in Depots)
        {
            if (depot.Name == name) return true;
        }

        return false;
    }

    /// <summary>
    /// A function that checks if a depot with this order name already exists for this customer
    /// </summary>
    /// <param name="orderName">Order name for the depot</param>
    /// <returns><see cref="bool"/> whether the depot exists or not</returns>
    public bool HasDepotOrderName(string orderName)
    {
        foreach (Depot depot in Depots)
        {
            if (depot.OrderName == orderName) return true;
        }

        return false;
    }

    /// <summary>
    /// A function that checks if a depot with this CSVName and OrderName already exists for this customer
    /// </summary>
    /// <param name="depot">A <see cref="Depot"/> object to compare</param>
    /// <returns><see cref="bool"/> whether the depot already exists or not</returns>
    public bool SameDepotExists(Depot depot)
    {
        foreach (Depot ourDepot in Depots)
        {
            if (ourDepot.CSVName == depot.CSVName && ourDepot.OrderName == depot.OrderName) return true;
        }

        return false;
    }

    /// <summary>
    /// Add a new depot to the list of depots if it doesn't already exist
    /// </summary>
    /// <param name="id">Id number of the depot</param>
    /// <param name="customerId">Id number of this customer</param>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    /// <returns><see cref="bool"/> false if the depot already exists</returns>
    public bool AddDepot(int id, string name, string csvName, string orderName)
    {
        // Make sure to check whether this depot exists first
        // We don't want duplicate depots
        if (!HasDepot(id) && !HasDepotName(name) && !HasDepotOrderName(orderName))
        {
            Depots.Add(new Depot(id, Id, name, csvName, orderName));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Add a new depot to the list of depots if it doesn't already exist
    /// </summary>
    /// <param name="depot">A <see cref="Depot"/> object</param>
    /// <returns><see cref="bool"/> false if the depot already exists</returns>
    public bool AddDepot(Depot depot)
    {
        // Make sure to check whether this depot exists first
        // We don't want duplicate depots
        if (!HasDepot(depot.Id) && !HasDepotName(depot.Name) && !HasDepotOrderName(depot.OrderName))
        {
            Depots.Add(depot);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Add a list of depots if it doesn't already exist
    /// </summary>
    /// <param name="Depots">A list of <see cref="Depot"/></param>
    public void AddDepots(List<Depot> Depots)
    {
        foreach (Depot depot in Depots)
        {
            // Make sure to check whether this depot exists first
            // We don't want duplicate depots
            if (!HasDepot(depot.Id) && !HasDepotName(depot.Name) && !HasDepotOrderName(depot.OrderName))
            {
                Depots.Add(depot);
            }
        }
    }

    /// <summary>
    /// Delets a depot from this customer
    /// </summary>
    /// <param name="id">Id number of this depot</param>
    public void DeleteDepot(int id)
    {
        foreach (Depot depot in Depots)
        {
            if (depot.Id == id)
            {
                Depots.Remove(depot);
                return;
            }
        }
    }

    /// <summary>
    /// Gets the depot by it's Id
    /// </summary>
    /// <param name="id">Id number of this depot</param>
    /// <returns><see cref="Depot"/> object</returns>
    public Depot GetDepot(int id)
    {
        foreach (Depot depot in Depots)
        {
            if (depot.Id == id) return depot;
        }

        // If the depot was not found, return null
        return null;
    }

    /// <summary>
    /// Gets the depot by it's Order Name
    /// </summary>
    /// <param name="orderName">Order name of this depot</param>
    /// <returns><see cref="Depot"/> object</returns>
    public Depot GetDepot(string orderName)
    {
        foreach (Depot depot in Depots)
        {
            if (depot.OrderName == orderName) return depot;
        }

        // If the depot was not found, return null
        return null;
    }

    #endregion
}