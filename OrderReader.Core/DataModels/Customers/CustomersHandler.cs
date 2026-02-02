using System.Collections.Generic;
using System.Linq;
using OrderReader.Core.DataAccess;

namespace OrderReader.Core.DataModels.Customers;

/// <summary>
/// A class that manages the customers
/// </summary>
public class CustomersHandler
{
    #region Public Properties

    /// <summary>
    /// A list of all known customers
    /// </summary>
    public List<CustomerProfile> CustomerProfiles { get; private set; } = [];

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor that loads the customers from database
    /// </summary>
    public CustomersHandler()
    {
        LoadCustomerData();
    }

    #endregion

    #region Public Helpers

    /// <summary>
    /// Loads the customers from database into the Customers property
    /// </summary>
    public void LoadCustomerData()
    {
        CustomerProfiles = SqliteDataAccess.LoadCustomers();
    }

    /// <summary>
    /// Checks if a customer with that ID exists
    /// </summary>
    /// <param name="id">ID number of the customer</param>
    /// <returns>True or false</returns>
    public bool HasCustomerProfile(int id)
    {
        return CustomerProfiles.Any(customerProfile => customerProfile.Id == id);
    }

    /// <summary>
    /// Check if a customer with that name already exists
    /// </summary>
    /// <param name="name">Name as it appears in the UI</param>
    /// <returns>true or false</returns>
    public bool HasCustomerProfileName(string name)
    {
        return CustomerProfiles.Any(customerProfile => customerProfile.Name == name);
    }

    /// <summary>
    /// Check if a customer with that order name already exists
    /// </summary>
    /// <param name="identifier">The name that will appear on the orders that we are reading from</param>
    /// <returns>True or false</returns>
    public bool HasCustomerProfileIdentifier(string identifier)
    {
        return CustomerProfiles.Any(customerProfile => customerProfile.Identifier == identifier);
    }

    /// <summary>
    /// Gets the customer object by ID number
    /// </summary>
    /// <param name="id">The ID number of required customer</param>
    /// <returns><see cref="Customer"/> object or Null</returns>
    public CustomerProfile? GetCustomerProfile(int id)
    {
        return CustomerProfiles.FirstOrDefault(customerProfile => customerProfile.Id == id);
    }

    /// <summary>
    /// Gets the customer object by the order name
    /// </summary>
    /// <param name="identifier">The name that will appear on the orders that we are reading from</param>
    /// <returns>A <see cref="Customer"/> object</returns>
    public CustomerProfile? GetCustomerProfile(string identifier)
    {
        return CustomerProfiles.FirstOrDefault(customerProfile => customerProfile.Identifier == identifier);
    }

    #endregion
}