/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using Microsoft.Extensions.DependencyInjection;

using WCountLib.Counters;
using WCountLib.Counters.Abstractions;

using WCountLib.Detectors;
using WCountLib.Detectors.Abstractions;
// ReSharper disable UnusedMember.Global

namespace WCountLib.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseWCount(this IServiceCollection services)
        {
            services.AddSingleton<IWordDetector, WordDetector>();
            
            services.AddSingleton<ILineCounter, LineCounter>();
            services.AddSingleton<IByteCounter, ByteCounter>();
            services.AddSingleton<ICharCounter, CharCounter>();
            services.AddSingleton<IWordCounter, WordCounter>();
            
            return services;
        }
    }
}