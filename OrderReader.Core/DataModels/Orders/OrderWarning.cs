namespace OrderReader.Core.DataModels.Orders;

/// <summary>
/// Represents a warning that there's something in an order that the user should be aware of
/// </summary>
public class OrderWarning
{
    #region Enums

    /// <summary>
    /// A list of possible warning types
    /// </summary>
    public enum WarningType
    {
        UnusualDate,
        UnknownProduct
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Type of this warning
    /// </summary>
    public WarningType Type { get; private set; }

    /// <summary>
    /// Message to be displayed to user
    /// </summary>
    public string Message { get; private set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="type">Type of this warning</param>
    /// <param name="message">Message to be displayed to user</param>
    public OrderWarning(WarningType type, string message)
    {
        Type = type;
        Message = message;
    }

    #endregion
}