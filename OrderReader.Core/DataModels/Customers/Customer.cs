using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OrderReader.Core
{
    /// <summary>
    /// The customer class that will hold information about the customer, as well as it's depots and products
    /// </summary>
    [Serializable]
    public class Customer : ISerializable
    {
        #region Public Properties

        /// <summary>
        /// Name of the customer, this is what's displayed in the application UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name that will appear on the CSV file
        /// </summary>
        public string CSVName { get; set; }

        /// <summary>
        /// The name that will appear on the orders that we are reading from
        /// </summary>
        public string OrderName { get; set; }

        /// <summary>
        /// List of depots for this customer
        /// </summary>
        public List<Depot> Depots { get; } = new List<Depot>();

        /// <summary>
        /// List of products for this customer
        /// </summary>
        public List<Product> Products { get; } = new List<Product>();

        #endregion

        #region Constructors

        public Customer() { }

        /// <summary>
        /// A constructor that creates the customer and optionally depots and products (if they are provided)
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <param name="CSVName">Name on CSV files</param>
        /// <param name="OrderName">Name on Orders</param>
        /// <param name="Depots">A list of depots that belong to this customer</param>
        /// <param name="Products">A list of products that belong to this customer</param>
        public Customer(string Name, string CSVName, string OrderName, List<Depot> Depots = null, List<Product> Products = null)
        {
            this.Name = Name;
            this.CSVName = CSVName;
            this.OrderName = OrderName;

            // If populated list of depots is provided, add it
            if (Depots != null) this.Depots = Depots;

            // If populated list of products is provided, add it
            if (Products != null) this.Products = Products;
        }

        /// <summary>
        /// A constructor for deserializing this class
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public Customer(SerializationInfo info, StreamingContext context)
        {
            Name = (string)info.GetValue("Name", typeof(string));
            CSVName = (string)info.GetValue("CSVName", typeof(string));
            OrderName = (string)info.GetValue("OrderName", typeof(string));
            Depots = (List<Depot>)info.GetValue("Depots", typeof(List<Depot>));
            Products = (List<Product>)info.GetValue("Products", typeof(List<Product>));
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Updates the details of this customer
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <param name="CSVName">Name on CSV files</param>
        /// <param name="OrderName">Name on Orders</param>
        public void Update(string Name, string CSVName, string OrderName)
        {
            this.Name = Name;
            this.CSVName = CSVName;
            this.OrderName = OrderName;
        }

        /// <summary>
        /// A function that checks if a product with this name already exists for this customer
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <returns><see cref="bool"/> whether the products already exists or not</returns>
        public bool HasProduct(string Name)
        {
            foreach (Product product in Products)
            {
                if (product.Name == Name) return true;
            }

            return false;
        }

        /// <summary>
        /// Add a new product to the list of products if it doesn't already exist
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <param name="CSVName">Name on CSV files</param>
        /// <param name="OrderName">Name on Orders</param>
        /// <returns><see cref="bool"/> false if the product already exists</returns>
        public bool AddProduct(string Name, string CSVName, string OrderName)
        {
            // Make sure to check whether this product exists first
            // We don't want duplicate products
            if (!HasProduct(Name))
            {
                Products.Add(new Product(Name, CSVName, OrderName));
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
                if (!HasProduct(product.Name))
                {
                    Products.Add(product);
                }
            }
        }

        /// <summary>
        /// Delets a product from this customer
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        public void DeleteProduct(string Name)
        {
            foreach (Product product in Products)
            {
                if (product.Name == Name)
                {
                    Products.Remove(product);
                }
            }
        }

        /// <summary>
        /// Gets the product by it's name
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <returns><see cref="Product"/> object</returns>
        public Product GetProduct(string Name)
        {
            foreach (Product product in Products)
            {
                if (product.Name == Name) return product;
            }

            // If the product was not found, return null
            return null;
        }

        /// <summary>
        /// A function that checks if a depot with this name already exists for this customer
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <returns><see cref="bool"/> whether the depot already exists or not</returns>
        public bool HasDepot(string Name)
        {
            foreach (Depot depot in Depots)
            {
                if (depot.Name == Name) return true;
            }

            return false;
        }

        /// <summary>
        /// Add a new depot to the list of depots if it doesn't already exist
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <param name="CSVName">Name on CSV files</param>
        /// <param name="OrderName">Name on Orders</param>
        /// <returns><see cref="bool"/> false if the depot already exists</returns>
        public bool AddDepot(string Name, string CSVName, string OrderName)
        {
            // Make sure to check whether this depot exists first
            // We don't want duplicate depots
            if (!HasDepot(Name))
            {
                Depots.Add(new Depot(Name, CSVName, OrderName));
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
                if (!HasDepot(depot.Name))
                {
                    Depots.Add(depot);
                }
            }
        }

        /// <summary>
        /// Delets a depot from this customer
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        public void DeleteDepot(string Name)
        {
            foreach (Depot depot in Depots)
            {
                if (depot.Name == Name)
                {
                    Depots.Remove(depot);
                }
            }
        }

        /// <summary>
        /// Gets the depot by it's name
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <returns><see cref="Depot"/> object</returns>
        public Depot GetDepot(string Name)
        {
            foreach (Depot depot in Depots)
            {
                if (depot.Name == Name) return depot;
            }

            // If the depot was not found, return null
            return null;
        }

        /// <summary>
        /// Serializes this object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("CSVName", CSVName);
            info.AddValue("OrderName", OrderName);
            info.AddValue("Depots", Depots);
            info.AddValue("Products", Products);
        }

        #endregion
    }
}
