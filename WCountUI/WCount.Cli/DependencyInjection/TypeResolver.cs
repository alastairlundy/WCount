using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace WCount.Cli.DependencyInjection;

public class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _serviceProvider;

    public TypeResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public object? Resolve(Type? type)
    {
        return _serviceProvider.GetRequiredService(type!);
    }


    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}