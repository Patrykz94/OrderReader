using Dapper;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace OrderReader.Core
{
    /// <summary>
    /// A class used to access data in the database
    /// </summary>
    public class SqliteDataAccess
    {
        /// <summary>
        /// Load all customers from the database
        /// </summary>
        /// <returns>A list of customers</returns>
        public static ObservableCollection<Customer> LoadCustomers()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
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
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
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
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM 'Depots' WHERE \"Id\" = {depotId}");
            }
        }

        /// <summary>
        /// Add a new depot to the database
        /// </summary>
        /// <param name="depot">A <see cref="Depot"/> object to be added</param>
        public static Depot AddDepot(Depot depot)
        {
            Depot newDepot;

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var result = cnn.Query<int>("INSERT INTO 'Depots' (CustomerId, Name, CSVName, OrderName) VALUES (@CustomerId, @Name, @CSVName, @OrderName); SELECT last_insert_rowid();", depot).ToList();
                newDepot = cnn.QuerySingle<Depot>($"SELECT * FROM 'Depots' WHERE \"Id\" = {result[0]}", new DynamicParameters());
            }

            return newDepot;
        }

        /// <summary>
        /// Update the depot in the database
        /// </summary>
        /// <param name="depot"></param>
        public static void UpdateDepot(Depot depot)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
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
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM 'Products' WHERE \"Id\" = {productId}");
            }
        }

        /// <summary>
        /// Add a new product to the database
        /// </summary>
        /// <param name="product">A <see cref="Product"/> object to be added</param>
        public static Product AddProduct(Product product)
        {
            Product newProduct;

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var result = cnn.Query<int>("INSERT INTO 'Products' (CustomerId, Name, CSVName, OrderName, Price) VALUES (@CustomerId, @Name, @CSVName, @OrderName, @Price); SELECT last_insert_rowid();", product).ToList();
                newProduct = cnn.QuerySingle<Product>($"SELECT * FROM 'Products' WHERE \"Id\" = {result[0]}", new DynamicParameters());
            }

            return newProduct;
        }

        /// <summary>
        /// Update the product in the database
        /// </summary>
        /// <param name="product"></param>
        public static void UpdateProduct(Product product)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE 'Products' SET \"Name\" = @Name, \"CSVName\" = @CSVName, \"OrderName\" = @OrderName, \"Price\" = @Price WHERE \"Id\" = @Id", product);
            }
        }

        private static string LoadConnectionString(string id = "Development2")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
