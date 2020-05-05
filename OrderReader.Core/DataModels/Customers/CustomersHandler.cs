using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OrderReader.Core
{
    /// <summary>
    /// A class that manages the customers
    /// </summary>
    public class CustomersHandler
    {
        #region Public Properties
        
        /// <summary>
        /// A list of all known customers
        /// </summary>
        public List<Customer> Customers { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// A default constructor that loads a list of customers
        /// </summary>
        public CustomersHandler()
        {
            Customers = new List<Customer>();
        }

        #endregion

        #region Public Helpers

        public void SaveCustomers()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>), new XmlRootAttribute("Customers"));

            using(TextWriter tw = new StreamWriter($@"{Environment.CurrentDirectory}\test.xml"))
            {
                serializer.Serialize(tw, Customers);
            }
        }

        public void LoadCustomers()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<Customer>), new XmlRootAttribute("Customers"));

            using(TextReader reader = new StreamReader($@"{Environment.CurrentDirectory}\test.xml"))
            {
                object obj = deserializer.Deserialize(reader);
                Customers = (List<Customer>)obj;
            }
        }

        #endregion
    }
}
