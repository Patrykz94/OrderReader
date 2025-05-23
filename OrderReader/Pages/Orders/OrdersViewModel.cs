﻿using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader.Core.DataModels.FileHandling;
using OrderReader.Core.DataModels.Orders;
using OrderReader.Core.Interfaces;
using OrderListViewModel = OrderReader.Controls.Orders.OrderListViewModel;

namespace OrderReader.Pages.Orders;

public class OrdersViewModel : Conductor<IScreen>, IFilesDropped
{
    private readonly OrdersLibrary _ordersLibrary;

    public OrderListViewModel OrderListControl { get; set; }

    public OrdersViewModel(OrdersLibrary ordersLibrary, OrderListViewModel orderListViewModel)
    {
        _ordersLibrary = ordersLibrary;
        OrderListControl = orderListViewModel;
    }

    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        await Orders();
    }

    public async void OnFilesDropped(string[] files)
    {
        // Process the order
        foreach (var file in files)
        {
            await FileImport.ProcessFileAsync(file);
        }

        UpdateOrders();
    }

    private async Task Orders()
    {
        await ActivateItemAsync(OrderListControl);
        UpdateOrders();
    }

    private void UpdateOrders()
    {
        var ordersList = _ordersLibrary.GetUniqueOrderIDs();
        foreach (var order in ordersList)
        {
            OrderListControl.AddItem(order);
        }
    }
}