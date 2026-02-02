namespace OrderReader.Core.DataModels.Customers
{
    /// <summary>
    /// A class that stores information about depot
    /// </summary>
    public class Depot
    {
        #region Public Properties

        /// <summary>
        /// Unique Depot ID number
        /// </summary>
        public int Id { get; private set; } = -1;

        /// <summary>
        /// ID of the customer that this depot belongs to
        /// </summary>
        public int CustomerId { get; }

        /// <summary>
        /// This is the name of the depot that will be displayed in the application
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

        #endregion

        #region Constructor

        public Depot()
        {
            Name = string.Empty;
            CsvName = string.Empty;
            OrderName = string.Empty;
        }
        
        /// <summary>
        /// Constructor that creates a new depot
        /// </summary>
        /// <param name="customerId">The Id number of customer that owns this depot</param>
        /// <param name="name">Name as will appear in the UI</param>
        /// <param name="csvName">Name on CSV files</param>
        /// <param name="orderName">Name on Orders</param>
        public Depot(int customerId, string name, string csvName, string orderName)
        {
            CustomerId = customerId;
            Name = name;
            CsvName = csvName;
            OrderName = orderName;
        }

        /// <summary>
        /// Constructor that creates a new depot. Used when loading depots from databse
        /// </summary>
        /// <param name="id">The Id number of this depot</param>
        /// <param name="customerId">The Id number of customer that owns this depot</param>
        /// <param name="name">Name as will appear in the UI</param>
        /// <param name="csvName">Name on CSV files</param>
        /// <param name="orderName">Name on Orders</param>
        public Depot(int id, int customerId, string name, string csvName, string orderName)
        {
            Id = id;
            CustomerId = customerId;
            Name = name;
            CsvName = csvName;
            OrderName = orderName;
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Updates the depot information
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

        /// <summary>
        /// Update the ID of the depot. This is usually used when creating a new depot, and we don't know the ID yet
        /// </summary>
        /// <param name="id">The ID number of the depot</param>
        public void UpdateId(int id)
        {
            Id = id;
        }

        #endregion
    }
}
