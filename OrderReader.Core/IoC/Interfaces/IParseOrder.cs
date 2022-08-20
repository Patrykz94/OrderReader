using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderReader.Core
{
    /// <summary>
    /// An interface that defines what each order parser should have
    /// </summary>
    public interface IParseOrder
    {
        /// <summary>
        /// The supported file extension that this processor reads
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Check to see if this processor can process this customers order
        /// </summary>
        /// <param name="orderText">A <see cref="Dictionary{TKey, TValue}"/> with all order text</param>
        /// <param name="customers">A <see cref="CustomersHandler"/> with a list of all <see cref="Customer"/> objects</param>
        /// <returns>A <see cref="Customer"/> object or <see cref="null"/></returns>
        Customer GetCustmer(Dictionary<string, string[]> orderText, CustomersHandler customers);

        /// <summary>
        /// Parses the order by reading the text
        /// </summary>
        /// <param name="orderText">A <see cref="Dictionary{TKey, TValue}"/> with all order text</param>
        /// <param name="fileName">Name of the file to be used in error messages</param>
        /// <param name="customer">A <see cref="Customer"/> object that holds all information about this customer, products and depots</param>
        Task ParseOrderAsync(Dictionary<string, string[]> orderText, string fileName, Customer customer);
    }
}
