using Microsoft.Extensions.Hosting;

namespace EventDrivenCheckout.Logistics;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        var host = builder.Build();
        host.Run();
    }
}