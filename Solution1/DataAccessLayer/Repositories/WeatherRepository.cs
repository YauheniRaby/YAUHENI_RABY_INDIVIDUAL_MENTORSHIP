using DataAccessLayer.Configuration;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        
        public async Task BulkSaveWeatherListAsync(IEnumerable<Weather> weatherList, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _context.CurrentWeathers.AddRangeAsync(weatherList, token);
            await _context.SaveChangesAsync(token);            
        }

        public async Task<IEnumerable<Weather>> GetWeatherListAsync(HistoryWeatherRequest historyWeatherRequest, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return await _context.CurrentWeathers
                .Where(x =>
                    x.CityName == historyWeatherRequest.CityName
                    && x.Datetime >= historyWeatherRequest.StartPeriod
                    && x.Datetime <= historyWeatherRequest.EndPeriod)
                .ToListAsync(token);
        }
    }
}
