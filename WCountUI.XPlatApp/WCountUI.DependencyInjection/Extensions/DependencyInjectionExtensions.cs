using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Counters;
using Microsoft.Extensions.DependencyInjection;
using WCountUI.XPlatApp.ViewModels;
using WCountUI.XPlatApp.Views;
// ReSharper disable InconsistentNaming

namespace WCountUI.DependencyInjection.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddWCountUI(this IServiceCollection services)
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddTransient<IWordCounter, WordCounter>();
        collection.AddTransient<ICharacterCounter, CharacterCounter>();
        collection.AddTransient<ILineCounter, LineCounter>();

        // Add views and Main Window here.
        collection.AddTransient(sp => new MainWindow
        {
            DataContext = sp.GetRequiredService<MainViewModel>(),
        });
        collection.AddTransient(sp => new MainView()
        {
            DataContext = sp.GetRequiredService<MainViewModel>(),
        });
        collection.AddTransient(sp => new EditorView()
        {
            DataContext = sp.GetRequiredService<EditorViewModel>(),
        });
        collection.AddTransient(sp => new SettingsView()
        {
            DataContext = sp.GetRequiredService<SettingsViewModel>(),
        });

        return collection;
    }
}