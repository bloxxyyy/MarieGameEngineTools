using MarieBaseLibrary.Database.DialogDB.Db;
using MarieBaseLibrary.Database.DialogDB.repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MarieBaseTestProject;

static class Program
{
    static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var app = serviceProvider.GetService<App>();
        app?.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        _ = new DialogDatabase();
        services.AddSingleton<IDialogRepository>(new DialogRepository());
        services.AddTransient<App>();
    }
}
