using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrderReader.Core.DataModels.Customers;

/// <summary>
/// The customer class that will hold information about the customer, as well as it's depots
/// </summary>
public class Customer
{
    #region Public Properties

    /// <summary>
    /// Unique Customer ID number
    /// </summary>
    public int Id { get; private set; } = -1;
    
    public int CustomerProfileId { get; }

    /// <summary>
    /// Name of the customer, this is what's displayed in the application UI
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
    /// List of depots for this customer
    /// </summary>
    public List<Depot> Depots { get; } = [];

    #endregion

    #region Constructors

    public Customer()
    {
        Name = string.Empty;
        CsvName = string.Empty;
        OrderName = string.Empty;
    }
    
    /// <summary>
    /// A constructor that creates the customer and optionally depots, if provided
    /// </summary>
    /// <param name="customerProfileId">The ID of the customer profile this customer belongs to</param>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    /// <param name="depots">A list of depots that belong to this customer</param>
    public Customer(int customerProfileId, string name, string csvName, string orderName, List<Depot>? depots = null)
    {
        CustomerProfileId = customerProfileId;
        Name = name;
        CsvName = csvName;
        OrderName = orderName;

        // If a populated list of depots is provided, add it
        if (depots != null) Depots = depots;
    }

    /// <summary>
    /// A constructor that creates the customer and optionally depots, if provided
    /// </summary>
    /// <param name="id">The ID of this customer</param>
    /// <param name="customerProfileId">The ID of the customer profile this customer belongs to</param>
    /// <param name="name">Name as will appear in the UI</param>
    /// <param name="csvName">Name on CSV files</param>
    /// <param name="orderName">Name on Orders</param>
    /// <param name="depots">A list of depots that belong to this customer</param>
    public Customer(int id, int customerProfileId, string name, string csvName, string orderName, List<Depot>? depots = null)
    {
        Id = id;
        CustomerProfileId = customerProfileId;
        Name = name;
        CsvName = csvName;
        OrderName = orderName;

        // If a populated list of depots is provided, add it
        if (depots != null) Depots = depots;
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
        CsvName = csvName;
        OrderName = orderName;
    }

    public void UpdateId(int id)
    {
        Id = id;
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
            if (ourDepot.CsvName == depot.CsvName && ourDepot.OrderName == depot.OrderName) return true;
        }

        return false;
    }

    /// <summary>
    /// Add a new depot to the list of depots if it doesn't already exist
    /// </summary>
    /// <param name="id">Id number of the depot</param>
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
    /// <param name="depots">A list of <see cref="Depot"/></param>
    public void AddDepots(List<Depot> depots)
    {
        foreach (var depot in depots)
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
    public Depot? GetDepot(int id)
    {
        return Depots.FirstOrDefault(depot => depot.Id == id);
    }

    /// <summary>
    /// Gets the depot by it's Order Name
    /// </summary>
    /// <param name="orderName">Order name of this depot</param>
    /// <returns><see cref="Depot"/> object</returns>
    public Depot? GetDepot(string orderName)
    {
        return Depots.FirstOrDefault(depot => depot.OrderName == orderName);
    }

    #endregion
}