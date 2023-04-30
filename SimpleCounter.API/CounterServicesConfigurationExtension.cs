using SimpleCounter.Common;
using SimpleCounter.Core;
using SimpleCounter.Data;
using SimpleCounter.Draw;

namespace SimpleCounter.API
{
    public static class CounterServicesConfigurationExtension
    {
        public static IServiceCollection AddCountersInfrastructure(
            this IServiceCollection services)
        {
            services.AddSingleton<IPageCounters, PageCounters>();
            services.AddSingleton<ICounterDraw, CounterDraw>();
            services.AddSingleton<ICounterData, CounterData>();
            services.AddSingleton<ICounterCore, CounterCore>();

            return services;
        }
    }
}
