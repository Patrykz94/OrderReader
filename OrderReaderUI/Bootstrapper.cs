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

        AppInitialization.Initialize();
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
        await DisplayRootViewForAsync(typeof(ShellViewModel));
    }

    protected override void Configure()
    {
        _container.Instance(ConfigureAutomapper());

        _container.Instance(_container);

        _container
            .Singleton<IWindowManager, WindowManager>()
            .Singleton<IEventAggregator, EventAggregator>();

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
