using System;
using System.Runtime.Serialization;

namespace OrderReader.Core
{
    /// <summary>
    /// A class that stores information about product
    /// </summary>
    [Serializable]
    public class Product : ISerializable
    {
        #region Public Properties

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

        #endregion

        #region Constructor

        public Product() { }

        /// <summary>
        /// Default constructor that creates a new product
        /// </summary>
        /// <param name="Name">Name as will appear in the UI</param>
        /// <param name="CSVName">Name on CSV files</param>
        /// <param name="OrderName">Name on Orders</param>
        public Product(string Name, string CSVName, string OrderName)
        {
            this.Name = Name;
            this.CSVName = CSVName;
            this.OrderName = OrderName;
        }

        /// <summary>
        /// A constructor for deserializing this class
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public Product(SerializationInfo info, StreamingContext context)
        {
            Name = (string)info.GetValue("Name", typeof(string));
            CSVName = (string)info.GetValue("CSVName", typeof(string));
            OrderName = (string)info.GetValue("OrderName", typeof(string));
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Updates the product information
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
        /// Serializes the this object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("CSVName", CSVName);
            info.AddValue("OrderName", OrderName);
        }

        #endregion
    }
}
