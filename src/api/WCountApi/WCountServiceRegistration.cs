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