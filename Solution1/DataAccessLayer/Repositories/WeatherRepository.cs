using DataAccessLayer.Configuration;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly RepositoryContext _context;

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

        public async Task<IEnumerable<Weather>> GetWeatherListAsync(string cityName, DateTime startPeriod, DateTime endPeriod, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return await _context.CurrentWeathers
                .Where(x =>
                    x.CityName == cityName
                    && x.Datetime >= startPeriod
                    && x.Datetime <= endPeriod)
                .ToListAsync(token);
        }
    }
}
