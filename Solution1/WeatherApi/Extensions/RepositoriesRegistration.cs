using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Abstract;
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
