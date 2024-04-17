using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader;
using OrderReader.Core;
using OrderListViewModel = OrderReaderUI.ViewModels.Controls.Orders.OrderListViewModel;

namespace OrderReaderUI.ViewModels;

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

        var ordersList = _ordersLibrary.GetUniqueOrderIDs();
        foreach (var order in ordersList)
        {
            OrderListControl.AddItem(order);
        }
    }

    private async Task Orders()
    {
        await ActivateItemAsync(OrderListControl);
    }
}