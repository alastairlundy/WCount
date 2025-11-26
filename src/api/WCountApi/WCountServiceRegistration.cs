using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using AlastairLundy.WCountLib.Abstractions.Detectors;
using AlastairLundy.WCountLib.Abstractions.Detectors.Segments;
using AlastairLundy.WCountLib.Counters;
using AlastairLundy.WCountLib.Counters.Segments;
using AlastairLundy.WCountLib.Detectors;
using AlastairLundy.WCountLib.Detectors.Segments;
using Microsoft.Extensions.DependencyInjection;

namespace WCountApi;

public static class WCountServiceRegistration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddWCount()
        {
            services.AddScoped<ISegmentWordDetector, SegmentWordDetector>();
            
            services.AddScoped<ISegmentWordCounter, SegmentWordCounter>();
            services.AddScoped<ISegmentCharacterCounter, SegmentCharacterCounter>();

            return services;
        }
    }
}