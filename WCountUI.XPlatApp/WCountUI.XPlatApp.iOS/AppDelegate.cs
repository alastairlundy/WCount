using System;
using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Counters;
using Foundation;
using UIKit;
using Avalonia;
using Avalonia.Controls;
using Avalonia.iOS;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using WCountUI.DependencyInjection.Extensions;
using WCountUI.XPlatApp.ViewModels;
using WCountUI.XPlatApp.Views;

namespace WCountUI.XPlatApp.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the 
// User Interface of the application, as well as listening (and optionally responding) to 
// application events from iOS.
[Register("AppDelegate")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public partial class AppDelegate : AvaloniaAppDelegate<App>

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddWCountUI();

        
        IServiceProvider provider = collection.BuildServiceProvider();

        return base.CustomizeAppBuilder(builder)
            .With<App>(() => new App(provider))
            .WithInterFont();
    }
}