using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Abstract;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class HistoryWeatherService : IHistoryWeatherService
    {
        readonly IWeatherRepository _weatherRepository;
        readonly IMapper _mapper;

        public HistoryWeatherService(IWeatherRepository weatherRepository, IMapper mapper)
        {
            _weatherRepository = weatherRepository;
            _mapper = mapper;
        }

        public async Task<HistoryWeatherDTO> GetByCityNameAndPeriodAsync(HistoryWeatherRequestDTO historyWeatherRequestDTO, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var historyWeatherRequest = _mapper.Map<HistoryWeatherRequest>(historyWeatherRequestDTO);
            var wetherList = await _weatherRepository.GetWeatherListAsync(historyWeatherRequest, token);
            return new HistoryWeatherDTO() 
            { 
                CityName = historyWeatherRequest.CityName, 
                WeatherList = _mapper.Map<IEnumerable<WeatherWithDatetimeDTO>>(wetherList) 
            };
        }

        public async Task BulkSaveWeatherListAsync(IEnumerable<Weather> weatherList, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _weatherRepository.BulkSaveWeatherListAsync(weatherList, token);
        }
    }
}
