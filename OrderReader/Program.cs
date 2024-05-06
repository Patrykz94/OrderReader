using System;
using Velopack;

namespace OrderReader;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build().Run();
        var application = new App();
        application.InitializeComponent();
        application.Run();
    }
}