using System;
using System.Net.Http;
using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Detectors;
using AlastairLundy.WCountLib.Counters;
using AlastairLundy.WCountLib.Detectors;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;


using WCountUI.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IWordDetector, WordDetector>();
builder.Services.AddScoped<IWordCounter, WordCounter>();
builder.Services.AddScoped<ICharacterCounter, CharacterCounter>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddFluentUIComponents();


//builder.Services.UseWCount();

await builder.Build().RunAsync();