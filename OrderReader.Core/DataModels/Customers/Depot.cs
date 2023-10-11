namespace OrderReader.Core
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
        public int Id { get; } = -1;

        /// <summary>
        /// Id of the customer that this depot belongs to
        /// </summary>
        public int CustomerId { get; }

        /// <summary>
        /// This is the name of the depot that will be displayed in the application
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

        /// <summary>
        /// Default constructor
        /// </summary>
        public Depot() { }

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
            CSVName = csvName;
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
            CSVName = csvName;
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
            CSVName = csvName;
            OrderName = orderName;
        }

        #endregion
    }
}
