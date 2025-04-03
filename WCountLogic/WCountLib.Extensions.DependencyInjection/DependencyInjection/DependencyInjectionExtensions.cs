/*
    WCountLib.Extensions
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Detectors;

using AlastairLundy.WCountLib.Counters;
using AlastairLundy.WCountLib.Detectors;

using Microsoft.Extensions.DependencyInjection;


// ReSharper disable UnusedMember.Global

namespace AlastairLundy.WCountLib.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Sets up the required services and implementations for Dependency Injection using Microsoft.Extensions.DependencyInjection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="lifetime">The service lifetime to register the services as.</param>
        /// <returns>The updated ServiceCollection with WCount's services.</returns>
        public static IServiceCollection AddWCount(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped<IWordDetector, WordDetector>();
                    
                    services.AddScoped<ILineCounter, LineCounter>();
                    services.AddScoped<IByteCounter, ByteCounter>();
                    services.AddScoped<ICharacterCounter, CharacterCounter>();
                    services.AddScoped<IWordCounter, WordCounter>();
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IWordDetector, WordDetector>();
                    
                    services.AddSingleton<ILineCounter, LineCounter>();
                    services.AddSingleton<IByteCounter, ByteCounter>();
                    services.AddSingleton<ICharacterCounter, CharacterCounter>();
                    services.AddSingleton<IWordCounter, WordCounter>();
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<IWordDetector, WordDetector>();
                    
                    services.AddTransient<ILineCounter, LineCounter>();
                    services.AddTransient<IByteCounter, ByteCounter>();
                    services.AddTransient<ICharacterCounter, CharacterCounter>();
                    services.AddTransient<IWordCounter, WordCounter>();
                    break;
            }
            
            return services;
        }
    }
}