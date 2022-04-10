using DataAccessLayer.Repository;
using DataAccessLayer.Repository.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApi.Extensions
{
    public static class RepositoryRegistration
    {
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddSingleton<IWeatherRepository, WeatherRepository>();
        }
    }
}
