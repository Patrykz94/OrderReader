using System.Threading.Tasks;
using OrderReader.Core.DataModels.Customers;
using OrderReader.Core.DataModels.Orders;

namespace OrderReader.Core.Interfaces;

/// <summary>
/// An interface for classes that read contents of files
/// </summary>
public interface IFileImport
{
    /// <summary>
    /// Function that identifies customer and calls apropriate plugin to process that file
    /// </summary>
    /// <returns>A <see cref="bool"/> whether the order could be processed</returns>
    Task<bool> ProcessFileAsync(CustomersHandler customersHandler, OrdersLibrary ordersLibrary);
}