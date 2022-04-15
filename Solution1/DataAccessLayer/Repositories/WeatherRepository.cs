using DataAccessLayer.Configuration;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        readonly RepositoryContext _context;

        public WeatherRepository(RepositoryContext context)
        {
            _context = context;
        }
        
        public async Task BulkSaveWeatherListAsync(IEnumerable<Weather> weatherList)
        {
            await _context.CurrentWeathers.AddRangeAsync(weatherList);
            await _context.SaveChangesAsync();            
        }
    }
}
