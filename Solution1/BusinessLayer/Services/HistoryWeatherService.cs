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

        public async Task<IEnumerable<WeatherWithDateTimeDTO>> GetByCityNameAndPeriodAsync(HistoryWeatherRequestDTO requestDto, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var wetherList = await _weatherRepository.GetWeatherListAsync(requestDto.CityName, requestDto.StartPeriod, requestDto.EndPeriod, token);
            return _mapper.Map<IEnumerable<WeatherWithDateTimeDTO>>(wetherList);
        }

        public Task BulkSaveWeatherListAsync(IEnumerable<Weather> weatherList, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return _weatherRepository.BulkSaveWeatherListAsync(weatherList, token);
        }
    }
}
