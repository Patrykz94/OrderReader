using System.Threading.Tasks;
using Caliburn.Micro;
using OrderReader;
using OrderReaderUI.ViewModels.Controls.Orders;
using OrderReaderUI.ViewModels.Dialogs;

namespace OrderReaderUI.ViewModels;

public class OrdersViewModel : Conductor<IScreen>, IFilesDropped
{
    private readonly IWindowManager _windowManager;

    public OrderListViewModel OrderListControl { get; set; }

    public OrdersViewModel(IWindowManager windowManager)
    {
        _windowManager = windowManager;
        OrderListControl = IoC.Get<OrderListViewModel>();
    }
    
    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        await Orders();
    }
    
    public void OnFilesDropped(string[] files)
    {
        var filesList = string.Join(",\n", files);
        _windowManager.ShowDialogAsync(new DialogMessageViewModel(
            "Following files have been dropped:\n" +
            $"{filesList}"));
        foreach (var file in files)
            OrderListControl.AddItem(file);
    }

    public async Task Orders()
    {
        await ActivateItemAsync(OrderListControl);
    }
}