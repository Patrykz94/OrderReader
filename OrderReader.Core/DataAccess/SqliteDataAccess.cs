using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using OrderReader.Core.DataModels.Customers;

namespace OrderReader.Core.DataAccess;

/// <summary>
/// A class used to access data in the database
/// </summary>
public static class SqliteDataAccess
{
    public static Dictionary<string, string> LoadDefaultSettings()
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        
        Dictionary<string, string> settings = cnn.Query("SELECT * FROM 'Settings'").ToDictionary(row => (string)row.Setting, row => (string)row.Value);

        return settings;
    }

    /// <summary>
    /// Load all customers from the database
    /// </summary>
    /// <returns>A list of customers</returns>
    public static List<CustomerProfile> LoadCustomers()
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        // Get all customer profiles from the database
        var customerProfiles = cnn.Query<CustomerProfile>("SELECT * FROM 'CustomerProfiles'").ToList();
            
        // Get all customers from the database and convert them to a list
        var customers = cnn.Query<Customer>("SELECT * FROM 'Customers'").ToList();

        // Get all depots and products from the database
        var depots = cnn.Query<Depot>("SELECT * FROM 'Depots'").ToList();
        var products = cnn.Query<Product>("SELECT * FROM 'Products'").ToList();

        // Match customers and products with their profiles
        foreach (var customerProfile in customerProfiles)
        {
            var profileCustomers = customers.Where(c => c.CustomerProfileId == customerProfile.Id).ToList();
            profileCustomers.ForEach(c => customerProfile.AddCustomer(c));
                
            var profileProducts = products.Where(p => p.CustomerProfileId == customerProfile.Id).ToList();
            profileProducts.ForEach(p => customerProfile.AddProduct(p));
        }
            
        // Match depots and product with their customers
        foreach (var customer in customers)
        {
            var customerDepots = depots.Where(d => d.CustomerId == customer.Id).ToList();
            customer.AddDepots(customerDepots);
        }
                
        // Return the customers
        return customerProfiles;
    }

    /// <summary>
    /// Update the customer profile in the database
    /// </summary>
    /// <param name="customerProfile"></param>
    public static void UpdateCustomerProfile(CustomerProfile customerProfile)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        
        cnn.Execute("UPDATE 'CustomerProfiles' SET \"Name\" = @Name, \"Identifier\" = @Identifier WHERE \"Id\" = @Id", customerProfile);
    }
    
    /// <summary>
    /// Adds a new customer to the database
    /// </summary>
    /// <param name="customer">A <see cref="Customer"/> object to be added</param>
    /// <returns>A unique ID of the created customer</returns>
    public static int AddCustomer(Customer customer)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());

        var result = cnn.Query<int>("INSERT INTO 'Customers' (CustomerProfileId, Name, CSVName, OrderName) VALUES (@CustomerProfileId, @Name, @CsvName, @OrderName); SELECT last_insert_rowid();", customer).ToList().FirstOrDefault();

        return result;
    }
    
    /// <summary>
    /// Update the customer in the database
    /// </summary>
    /// <param name="customer"></param>
    public static void UpdateCustomer(Customer customer)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        
        cnn.Execute("UPDATE 'Customers' SET \"Name\" = @Name, \"CSVName\" = @CSVName, \"OrderName\" = @OrderName WHERE \"Id\" = @Id", customer);
    }

    /// <summary>
    /// Remove a customer from the database
    /// </summary>
    /// <param name="customerId">An ID of the customer</param>
    public static void RemoveCustomer(int customerId)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        cnn.Execute($"DELETE FROM 'Customers' WHERE \"Id\" = {customerId}");
    }
    
    /// <summary>
    /// Remove a depot from the database
    /// </summary>
    /// <param name="depotId">An Id number of the depot</param>
    public static void RemoveDepot(int depotId)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        cnn.Execute($"DELETE FROM 'Depots' WHERE \"Id\" = {depotId}");
    }

    /// <summary>
    /// Adds a new depot to the database
    /// </summary>
    /// <param name="depot">A <see cref="Depot"/> object to be added</param>
    /// <returns>A unique ID of the created depot</returns>
    public static int AddDepot(Depot depot)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());

        var result = cnn.Query<int>("INSERT INTO 'Depots' (CustomerId, Name, CSVName, OrderName) VALUES (@CustomerId, @Name, @CSVName, @OrderName); SELECT last_insert_rowid();", depot).ToList().FirstOrDefault();

        return result;
    }

    /// <summary>
    /// Update the depot in the database
    /// </summary>
    /// <param name="depot"></param>
    public static void UpdateDepot(Depot depot)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        cnn.Execute("UPDATE 'Depots' SET \"Name\" = @Name, \"CSVName\" = @CSVName, \"OrderName\" = @OrderName WHERE \"Id\" = @Id", depot);
    }

    /// <summary>
    /// Remove a product from the database
    /// </summary>
    /// <param name="productId">An ID number of the product</param>
    public static void RemoveProduct(int productId)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        cnn.Execute($"DELETE FROM 'Products' WHERE \"Id\" = {productId}");
    }

    /// <summary>
    /// Adds a new product to the database
    /// </summary>
    /// <param name="product">A <see cref="Product"/> object to be added</param>
    /// <returns>A unique ID of the created product</returns>
    public static int AddProduct(Product product)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());

        var result = cnn.Query<int>("INSERT INTO 'Products' (CustomerProfileId, Name, CSVName, OrderName, Price) VALUES (@CustomerProfileId, @Name, @CSVName, @OrderName, @Price); SELECT last_insert_rowid();", product).ToList().FirstOrDefault();

        return result;
    }

    /// <summary>
    /// Update the product in the database
    /// </summary>
    /// <param name="product"></param>
    public static void UpdateProduct(Product product)
    {
        using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
        cnn.Execute("UPDATE 'Products' SET \"Name\" = @Name, \"CSVName\" = @CsvName, \"OrderName\" = @OrderName, \"Price\" = @Price WHERE \"Id\" = @Id", product);
    }

    /// <summary>
    /// A function that checks if a connection can be made with the database
    /// </summary>
    /// <returns></returns>
    public static bool TestConnection()
    {
        try
        {
            if (!HasConnectionString()) return false;

            using IDbConnection cnn = new SqliteConnection(LoadConnectionString());
            // Get all customers from the database and convert them to a list
            var output = cnn.Query<CustomerProfile>("SELECT * FROM CustomerProfiles").ToList();
            return output.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if the connection string exists
    /// </summary>
    /// <param name="id">Connection string ID</param>
    /// <returns>True or false whether the connection string exists</returns>
    public static bool HasConnectionString(string id = "default")
    {
        var con = ConfigurationManager.ConnectionStrings[id];
        return con != null;
    }

    /// <summary>
    /// Get the connection string
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private static string LoadConnectionString(string id = "default")
    {
        return HasConnectionString(id) ? ConfigurationManager.ConnectionStrings[id].ConnectionString : "";
    }
}