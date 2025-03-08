using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace WCount.Cli.DependencyInjection;

internal class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _serviceCollection;
    
    internal TypeRegistrar(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }
    
    public void Register(Type service, Type implementation)
    {
        throw new NotImplementedException();
    }

    public void RegisterInstance(Type service, object implementation)
    {
        throw new NotImplementedException();
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        throw new NotImplementedException();
    }

    public ITypeResolver Build()
    {
        throw new NotImplementedException();
    }
}