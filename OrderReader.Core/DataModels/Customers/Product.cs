namespace OrderReader.Core
{
    /// <summary>
    /// A class that stores information about product
    /// </summary>
    public class Product
    {
        #region Public Properties

        /// <summary>
        /// Unique Product ID number
        /// </summary>
        public int Id { get; } = -1;

        /// <summary>
        /// Id of the customer that this product belongs to
        /// </summary>
        public int CustomerId { get; }

        /// <summary>
        /// This is the name of the product that will be displayed in the application
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
        /// The price of this product
        /// </summary>
        public decimal Price { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Product() { }

        /// <summary>
        /// Constructor that creates a new product
        /// </summary>
        /// <param name="customerId">The Id number of customer that owns this product</param>
        /// <param name="name">Name as will appear in the UI</param>
        /// <param name="csvName">Name on CSV files</param>
        /// <param name="orderName">Name on Orders</param>
        /// <param name="price">Price of this product</param>
        public Product(int customerId, string name, string csvName, string orderName, decimal price = 0.0m)
        {
            CustomerId = customerId;
            Name = name;
            CSVName = csvName;
            OrderName = orderName;
            Price = price;
        }

        /// <summary>
        /// Constructor that creates a new product. Used when loading products from databse
        /// </summary>
        /// <param name="id">The Id number of this product</param>
        /// <param name="customerId">The Id number of customer that owns this product</param>
        /// <param name="name">Name as will appear in the UI</param>
        /// <param name="csvName">Name on CSV files</param>
        /// <param name="orderName">Name on Orders</param>
        /// <param name="price">Price of this product</param>
        public Product(int id, int customerId, string name, string csvName, string orderName, decimal price = 0.0m)
        {
            Id = id;
            CustomerId = customerId;
            Name = name;
            CSVName = csvName;
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
            CSVName = csvName;
            OrderName = orderName;
            Price = price;
        }

        #endregion
    }
}
