using System.Threading.Tasks;

namespace OrderReader.Core
{
    /// <summary>
    /// An interface for classes that read contents of files
    /// </summary>
    public interface IFileImport
    {
        /// <summary>
        /// Function that identifies customer and calls apropriate plugin to process that file
        /// </summary>
        /// <returns>A <see cref="bool"/> whether or not the order could be processed</returns>
        Task<bool> ProcessFileAsync(CustomersHandler customersHandler, OrdersLibrary ordersLibrary);
    }
}
