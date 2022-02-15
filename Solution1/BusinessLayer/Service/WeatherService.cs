using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.DTOs;
using DataAccessLayer.Abstract;
using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class WeatherService : IWeatherServise
    {
        readonly IWetherRepository _wetherRepository;
        readonly IMapper _mapper;

        public WeatherService(IWetherRepository wetherRepository, IMapper mapper) 
        { 
            _wetherRepository = wetherRepository; 
            _mapper = mapper;
        }

        public string GetByCityName(string CityName)
        {

            try
            {
                var weather = _wetherRepository.GetByCityName(CityName);
                return _mapper.Map<WeatherDTO>(weather).ToString();
            }
            catch (WebException ex)
            {
                return ex.Message;
            }

                   
        }
    }
}
