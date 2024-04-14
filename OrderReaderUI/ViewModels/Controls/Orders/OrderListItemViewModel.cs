﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using OrderReader.Core;
using IoC = Caliburn.Micro.IoC;

namespace OrderReaderUI.ViewModels.Controls.Orders;

public class OrderListItemViewModel : Screen
{

    #region Private Variables
    
    private readonly OrdersLibrary _ordersLibrary;
    
    private DataTable _ordersTable = new ();

    #endregion
    
    #region Public Properties

    public string OrderId { get; init; }

    public ObservableCollection<Order> Orders { get; private set; } = [];

    public DataView OrdersView => _ordersTable.DefaultView;

    #endregion

    #region Constructors

    public OrderListItemViewModel(string orderId)
    {
        _ordersLibrary = IoC.Get<OrdersLibrary>();
        OrderId = orderId;
        
        UpdateOrders();
    }

    #endregion

    #region Public Methods
    
    public void UpdateOrders()
    {
        // Get the orders with an ID that we want
        Orders = _ordersLibrary.GetAllOrdersWithID(OrderId);
        
        // TODO - if there are no orders with this Id, this view model should not exist, therefore we need to destroy it
        
        // Update the data table with the new data
        DataTable tempTable = new();

        // Add required basic columns
        tempTable.Columns.Add("Depot", typeof(string));
        tempTable.Columns.Add("PO Number", typeof(string));
        
        // Get a list of all unique products
        var uniqueProducts = Orders.SelectMany(x => x.Products).DistinctBy(x => x.ProductID).OrderBy(x => x.ProductName).ToList();
        
        // Add a column for each product name
        foreach (var uniqueProduct in uniqueProducts)
        {
            tempTable.Columns.Add(uniqueProduct.ProductName, typeof(double));
        }
        
        // Add the totals column at the end
        tempTable.Columns.Add("Total", typeof(double));
        
        // Add records to the table
        foreach (var order in Orders)
        {
            var newRow = tempTable.NewRow();
            newRow["Depot"] = order.DepotName;
            newRow["PO Number"] = order.OrderReference;

            var totalQuantity = 0.0;
            foreach (var product in order.Products)
            {
                newRow[product.ProductName] = product.Quantity;
                totalQuantity += product.Quantity;
            }

            newRow["Total"] = totalQuantity;
            
            tempTable.Rows.Add(newRow);
        }
        
        // Add total row
        var totalRow = tempTable.NewRow();
        totalRow["Depot"] = string.Empty;
        totalRow["PO Number"] = "Totals";

        for (var i = 2; i <= tempTable.Columns.Count; i++)
        {
            var column = tempTable.Columns[i];
            totalRow[column.ColumnName] = tempTable.Rows.Cast<DataRow>().Sum(row => (double)row[column.ColumnName]);
        }
            
        tempTable.Rows.Add(totalRow);
        
    }

    #endregion
}