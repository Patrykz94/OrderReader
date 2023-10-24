using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;

namespace OrderReader.Core
{
    /// <summary>
    /// A class used to access data in the database
    /// </summary>
    public class SqliteDataAccess
    {
        public static Dictionary<string, string> LoadDefaultSettings()
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                // Get all customers from database and convert them to a list
                Dictionary<string, string> output = cnn.Query("SELECT * FROM 'Settings'", new DynamicParameters()).ToDictionary(row => (string)row.Setting, row => (string)row.Value);

                return output;
            }
        }

        /// <summary>
        /// Load all customers from the database
        /// </summary>
        /// <returns>A list of customers</returns>
        public static ObservableCollection<Customer> LoadCustomers()
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                // Get all customers from database and convert them to a list
                var custOutput = cnn.Query<Customer>("SELECT * FROM 'Customers'", new DynamicParameters()).ToList();

                // Get all depots and products from the database
                var depotOutput = cnn.Query<Depot>("SELECT * FROM 'Depots'", new DynamicParameters()).ToList();
                var productOutput = cnn.Query<Product>("SELECT * FROM 'Products'", new DynamicParameters()).ToList();

                // Match depots and producst with their customers
                foreach (Customer c in custOutput)
                {
                    // Add the matching depots to their customers and remove from output list
                    depotOutput.Reverse();
                    for (int i = depotOutput.Count - 1; i >= 0; i--)
                    {
                        if (c.Id == depotOutput[i].CustomerId)
                        {
                            c.AddDepot(depotOutput[i]);
                            depotOutput.RemoveAt(i);
                        }
                    }

                    // Add the matching product to their customers and remove from output list
                    productOutput.Reverse();
                    for (int i = productOutput.Count - 1; i >= 0; i--)
                    {
                        if (c.Id == productOutput[i].CustomerId)
                        {
                            c.AddProduct(productOutput[i]);
                            productOutput.RemoveAt(i);
                        }
                    }
                }
                
                // Return the customers as an observable collection
                return new ObservableCollection<Customer>(custOutput);
            }
        }

        /// <summary>
        /// Update the customer in the database
        /// </summary>
        /// <param name="customer"></param>
        public static void UpdateCustomer(Customer customer)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE 'Customers' SET \"Name\" = @Name, \"CSVName\" = @CSVName, \"OrderName\" = @OrderName WHERE \"Id\" = @Id", customer);
            }
        }

        /// <summary>
        /// Remove a depot from the database
        /// </summary>
        /// <param name="depotId">An Id number of the depot</param>
        public static void RemoveDepot(int depotId)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM 'Depots' WHERE \"Id\" = {depotId}");
            }
        }

        /// <summary>
        /// Adds a new depot to the database
        /// </summary>
        /// <param name="depot">A <see cref="Depot"/> object to be added</param>
        /// <returns>A uniqie ID of the created depot</returns>
        public static int AddDepot(Depot depot)
        {
            using IDbConnection cnn = new SqliteConnection(LoadConnectionString());

            int result = cnn.Query<int>("INSERT INTO 'Depots' (CustomerId, Name, CSVName, OrderName) VALUES (@CustomerId, @Name, @CSVName, @OrderName); SELECT last_insert_rowid();", depot).ToList().FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Update the depot in the database
        /// </summary>
        /// <param name="depot"></param>
        public static void UpdateDepot(Depot depot)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE 'Depots' SET \"Name\" = @Name, \"CSVName\" = @CSVName, \"OrderName\" = @OrderName WHERE \"Id\" = @Id", depot);
            }
        }

        /// <summary>
        /// Remove a product from the database
        /// </summary>
        /// <param name="productId">An Id number of the product</param>
        public static void RemoveProduct(int productId)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM 'Products' WHERE \"Id\" = {productId}");
            }
        }

        /// <summary>
        /// Adds a new product to the database
        /// </summary>
        /// <param name="product">A <see cref="Product"/> object to be added</param>
        /// <returns>A unique ID of the created product</returns>
        public static int AddProduct(Product product)
        {
            using IDbConnection cnn = new SqliteConnection(LoadConnectionString());

            var result = cnn.Query<int>("INSERT INTO 'Products' (CustomerId, Name, CSVName, OrderName, Price) VALUES (@CustomerId, @Name, @CSVName, @OrderName, @Price); SELECT last_insert_rowid();", product).ToList().FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Update the product in the database
        /// </summary>
        /// <param name="product"></param>
        public static void UpdateProduct(Product product)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE 'Products' SET \"Name\" = @Name, \"CSVName\" = @CSVName, \"OrderName\" = @OrderName, \"Price\" = @Price WHERE \"Id\" = @Id", product);
            }
        }

        /// <summary>
        /// A function that check if a connection can be made with the database
        /// </summary>
        /// <returns></returns>
        public static bool TestConnection()
        {
            try
            {
                if (!HasConnectionString()) return false;

                using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
                {
                    // Get all customers from database and convert them to a list
                    var custOutput = cnn.Query<Customer>("SELECT * FROM Customers").ToList();
                    if (custOutput.Count > 0) return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Check if the connection string exists
        /// </summary>
        /// <param name="key">Connection string ID</param>
        /// <returns>True or false whether the connection string exists</returns>
        public static bool HasConnectionString(string id = "default")
        {
            var con = ConfigurationManager.ConnectionStrings[id];
            if (con == null) return false;
            return true;
        }

        /// <summary>
        /// Get the connection string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string LoadConnectionString(string id = "default")
        {
            if (HasConnectionString(id)) return ConfigurationManager.ConnectionStrings[id].ConnectionString;
            return "";
        }
    }
}
