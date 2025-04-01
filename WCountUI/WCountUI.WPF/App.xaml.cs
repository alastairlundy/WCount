using System;
using System.Configuration;
using System.Data;
using System.Windows;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Detectors;
using AlastairLundy.WCountLib.Counters;
using AlastairLundy.WCountLib.Detectors;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WCountUI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            IHostBuilder hostBuilder = new HostBuilder()
        .ConfigureServices(services =>
        {
            services.AddTransient<IWordDetector, WordDetector>();
            services.AddTransient<IWordCounter, WordCounter>();
            services.AddTransient<ICharacterCounter, CharacterCounter>();

            services.AddTransient<MainWindow>();
        });


            IHost host = hostBuilder.Build();

            using IServiceScope scope = host.Services.CreateScope();

            try
            {
                MainWindow form = scope.ServiceProvider.GetRequiredService<MainWindow>();
                form.Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An exception occurred, here's the details: {exception.Message}");
                Console.ReadLine();
            }
        }
    }

}
