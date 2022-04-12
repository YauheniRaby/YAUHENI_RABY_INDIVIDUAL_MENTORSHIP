using DataAccessLayer.Repository;
using DataAccessLayer.Repository.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApi.Extensions
{
    public static class RepositoriesRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IWeatherRepository, WeatherRepository>();
        }
    }
}
