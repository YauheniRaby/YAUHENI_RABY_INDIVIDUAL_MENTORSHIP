﻿using DataAccessLayer.Configuration;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class WeatherRepository : IWeatherRepository
    {
        readonly RepositoryContext _repository;

        public WeatherRepository(RepositoryContext repository)
        {
            _repository = repository;
        }
        
        public async Task BulkSaveWeatherAsync(IEnumerable<Weather> weather)
        {
            await _repository.CurrentWeathers.AddRangeAsync(weather);
            await _repository.SaveChangesAsync();            
        }
    }
}
