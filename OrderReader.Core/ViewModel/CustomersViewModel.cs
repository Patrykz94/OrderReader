using System.Collections.Generic;
using System.Windows.Input;

namespace OrderReader.Core
{
    /// <summary>
    /// The View Model for customer settings
    /// </summary>
    public class CustomersViewModel : BaseViewModel
    {
        #region Public Properties

        public List<string> Customers { get; set; }

        public CustomersHandler cust { get; set; }

        public string Text { get; set; } = "";

        #endregion

        #region Commands

        public ICommand LoadCustomers { get; set; }
        public ICommand SaveCustomers { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public CustomersViewModel()
        {
            Customers = new List<string>
            {
                "Keelings [Co-op]",
                "Keelings [Direct]",
                "Lidl"
            };

            cust = new CustomersHandler();

            LoadCustomers = new RelayCommand(() => cust.LoadCustomers());
            SaveCustomers = new RelayCommand(() => cust.SaveCustomers());
        }

        #endregion
    }
}
