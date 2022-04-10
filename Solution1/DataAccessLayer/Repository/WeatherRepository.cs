using DataAccessLayer.Configuration;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class WeatherRepository : IWeatherRepository
    {
        readonly IDbContextFactory<RepositoryContext> _repositoryFactory;

        public WeatherRepository(IDbContextFactory<RepositoryContext> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task AddWeatherAsync (Weather weather)
        {
            using (var repository = _repositoryFactory.CreateDbContext())
            {
                await repository.CurrentWeathers.AddAsync(weather);
                await repository.SaveChangesAsync();
            }            
        }
        
    }
}
