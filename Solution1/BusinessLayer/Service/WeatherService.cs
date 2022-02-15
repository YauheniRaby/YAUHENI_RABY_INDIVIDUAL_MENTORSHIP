using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class WeatherService : IWeatherServise
    {
        readonly IMapper _mapper;
        readonly IApiService _apiService;
        readonly IPrintService _printService;

        public WeatherService(IMapper mapper, IApiService apiService, IPrintService printService) 
        { 
            _mapper = mapper;
            _apiService = apiService;
            _printService = printService;
        }

        public async Task<string> GetByCityNameAsync(string cityName)
        {
            try
            {
                var weather = await _apiService.GetJsonByCityName(cityName);
                var weatherShort = _mapper.Map<WeatherDTO>(weather);
                return _printService.Print(_mapper.Map<WeatherDTO>(weather));                 
            }
            catch (HttpRequestException)
            {
                return Const.BadCityName;
            }                   
        }
    }
}
