using AutoMapper;
using Caliburn.Micro;
using OrderReader.Core;
using OrderReaderUI.Helpers;
using OrderReaderUI.Models;
using OrderReaderUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace OrderReaderUI;

public class Bootstrapper : BootstrapperBase
{
    private readonly SimpleContainer _container = new();

    public Bootstrapper()
    {
        Initialize();

        LogManager.GetLog = type => new DebugLog(type);
    }

    private IMapper ConfigureAutomapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDisplayModel>();
            cfg.CreateMap<Depot, DepotDisplayModel>();
            cfg.CreateMap<Customer, CustomerDisplayModel>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
                .ForMember(dest => dest.Depots, opt => opt.MapFrom(src => src.Depots));
        });

        var output = config.CreateMapper();

        return output;
    }

    protected override async void OnStartup(object sender, StartupEventArgs e)
    {
        // Temporarily prevent the app from shutting down as soon as the only window is closed
        // This is because during the startup procedure, we may need to display message boxes.
        // When a message box is closed, because that was the only window visible at the time,
        // the whole application would be shut down as at that moment, there is nothing to display
        var defaultShutdownMode = Application.Current.ShutdownMode;
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        
        // Go through the app initialisation phase, loading configs, database connections, etc.
        var appInitialization = _container.GetInstance<AppInitialization>();
        await appInitialization.Initialize();
        
        // Reset the shutdown mode back to default once the startup procedure is complete
        Application.Current.ShutdownMode = defaultShutdownMode;
        
        // Display the shell view
        await DisplayRootViewForAsync(typeof(ShellViewModel));
    }

    protected override void Configure()
    {
        _container.Instance(ConfigureAutomapper());

        _container.Instance(_container);

        _container
            .Singleton<IWindowManager, WindowManager>()
            .Singleton<IEventAggregator, EventAggregator>()
            .Singleton<OrdersLibrary>()
            .Singleton<CustomersHandler>()
            .PerRequest<AppInitialization>();

        foreach (var assembly in SelectAssemblies())
        {
            assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }
    }

    protected override object GetInstance(Type service, string key)
    {
        return _container.GetInstance(service, key);
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
        return _container.GetAllInstances(service);
    }

    protected override void BuildUp(object instance)
    {
        _container.BuildUp(instance);
    }
}
