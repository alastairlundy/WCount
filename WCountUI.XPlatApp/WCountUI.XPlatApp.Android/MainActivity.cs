using System;
using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Counters;
using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using WCountUI.DependencyInjection.Extensions;
using WCountUI.XPlatApp.ViewModels;
using WCountUI.XPlatApp.Views;

namespace WCountUI.XPlatApp.Android;

[Activity(
    Label = "WCount",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
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