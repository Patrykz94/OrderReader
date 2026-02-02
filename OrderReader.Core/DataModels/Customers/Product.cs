namespace OrderReader.Core.DataModels.Customers;

/// <summary>
/// A class that stores information about product
/// </summary>
public class Product
{
    #region Public Properties

    /// <summary>
    /// Unique Product ID number
    /// </summary>
    public int Id { get; private set; } = -1;

    /// <summary>
    /// Id of the customer profile that this product belongs to
    /// </summary>
    public int CustomerProfileId { get; }

    /// <summary>
    /// This is the name of the product that will be displayed in the application
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The name that will appear on the CSV file
    /// </summary>
    public string CsvName { get; private set; }

    /// <summary>
    /// The name that will appear on the orders that we are reading from
    /// </summary>
    public string OrderName { get; private set; }

    /// <summary>
    /// The price of this product
    /// </summary>
    public decimal Price { get; private set; }

    #endregion

    #region Constructor

    public Product()
    {
        Name = string.Empty;
        CsvName = string.Empty;
        OrderName = string.Empty;
    }
    
    /// <summary>
    /// Constructor that creates a new product
    /// </summary>
    /// <param name="customerProfileId">The ID of customer profile that owns this product</param>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    /// <param name="price">Price of this product</param>
    public Product(int customerProfileId, string name, string csvName, string orderName, decimal price = 0.0m)
    {
        CustomerProfileId = customerProfileId;
        Name = name;
        CsvName = csvName;
        OrderName = orderName;
        Price = price;
    }

    /// <summary>
    /// Constructor that creates a new product. Used when loading products from databse
    /// </summary>
    /// <param name="id">The Id number of this product</param>
    /// <param name="customerProfileId">The Id number of customer profile that owns this product</param>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    /// <param name="price">Price of this product</param>
    public Product(int id, int customerProfileId, string name, string csvName, string orderName, decimal price = 0.0m)
    {
        Id = id;
        CustomerProfileId = customerProfileId;
        Name = name;
        CsvName = csvName;
        OrderName = orderName;
        Price = price;
    }

    #endregion

    #region Public Helpers

    /// <summary>
    /// Updates the product information
    /// </summary>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    /// <param name="price">Price of this product</param>
    public void Update(string name, string csvName, string orderName, decimal price)
    {
        Name = name;
        CsvName = csvName;
        OrderName = orderName;
        Price = price;
    }

    /// <summary>
    /// Update the ID of the product. This is usually used when craeting a new product and we don't know the ID yet
    /// </summary>
    /// <param name="id">The Id number of the product</param>
    public void UpdateId(int id)
    {
        Id = id;
    }

    #endregion
}