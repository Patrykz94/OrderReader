using System.Collections.Generic;
using System.Linq;

namespace OrderReader.Core.DataModels.Customers;

public class CustomerProfile
{
    public int Id { get; }
    public string Name { get; set; }
    public string Identifier { get; set; }
    public List<Customer> Customers { get; } = [];
    public List<Product> Products { get; } = [];

    public CustomerProfile()
    {
        Name = string.Empty;
        Identifier = string.Empty;
    }
    
    public CustomerProfile(int id, string name, string identifier)
    {
        Id = id;
        Name = name;
        Identifier = identifier;
    }

    public bool HasCustomer(int id)
    {
        return Customers.Any(customer => customer.Id == id);
    }

    public bool HasCustomerName(string name)
    {
        return Customers.Any(customer => customer.Name == name);
    }

    public bool HasCustomerOrderName(string orderName)
    {
        return Customers.Any(customer => customer.OrderName == orderName);
    }
    
    public bool AddCustomer(Customer customer)
    {
        if (Customers
            .Any(x => x.Id == customer.Id ||
                      x.Name == customer.Name ||
                      x.CsvName == customer.CsvName ||
                      x.OrderName == customer.OrderName))
            return false;
        
        Customers.Add(customer);
        return true;
    }
    
    public void DeleteCustomer(int id)
    {
        Customers.RemoveAll(x => x.Id == id);
    }
    
    public Customer? GetCustomer(int id)
    {
        return Customers.SingleOrDefault(customer => customer.Id == id);
    }

    public Customer? GetCustomer(string orderName)
    {
        return Customers.SingleOrDefault(customer => customer.OrderName == orderName);
    }
    
    public void Update(string name, string identifier)
    {
        Name = name;
        Identifier = identifier;
    }
    
    public bool HasProductName(string name)
    {
        return Products.Any(product => product.Name == name);
    }
    
    public bool HasProductOrderName(string orderName)
    {
        return Products.Any(product => product.OrderName == orderName);
    }

    public bool AddProduct(Product product)
    {
        if (Products
            .Any(x => x.Id == product.Id ||
                      x.Name == product.Name ||
                      x.CsvName == product.CsvName ||
                      x.OrderName == product.OrderName))
            return false;
        
        Products.Add(product);
        return true;
    }
    
    public void DeleteProduct(int id)
    {
        Products.RemoveAll(x => x.Id == id);
    }

    public Product? GetProduct(int id)
    {
        return Products.SingleOrDefault(x => x.Id == id);
    }
    
    public Product? GetProduct(string orderName)
    {
        return Products.SingleOrDefault(x => x.OrderName == orderName);
    }
}