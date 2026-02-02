using OrderReader.Core.DataModels.Customers;

namespace OrderReader.Core.DataModels.Orders;

/// <summary>
/// A class that holds a single product in an order
/// </summary>
public class OrderProduct
{
    #region Private Variables

    private readonly CustomerProfile _customerProfile;

    #endregion

    #region Public Properties

    /// <summary>
    /// The ID number of customer that ordered this product
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The ID number of the product that is ordered
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The name of the product that is ordered
    /// </summary>
    public string ProductName => GetProductName();

    /// <summary>
    /// The quantity of the product that is ordered
    /// </summary>
    public double Quantity { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// A default constructor for the OrderLine class
    /// </summary>
    public OrderProduct(int customerId, int productId, double quantity, CustomerProfile customerProfile)
    {
        _customerProfile = customerProfile;
        CustomerId = customerId;
        ProductId = productId;
        Quantity = quantity;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Get the name of this product
    /// </summary>
    /// <returns>Product name</returns>
    private string GetProductName()
    {
        return _customerProfile.GetProduct(ProductId)?.Name ?? $"Product not found [ID - {ProductId}]";
    }

    #endregion
}