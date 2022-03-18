using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Command
{
    public class BestWeatherCommand : ICommand<Dictionary<bool, IEnumerable<WeatherResponseDTO>>>
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IEnumerable<string> _cityNames;

        public BestWeatherCommand(IWeatherServiсe weatherServiсe, IEnumerable<string> cityNames)
        {
            _weatherServiсe = weatherServiсe;
            _cityNames = cityNames;
        }

        public Task<Dictionary<bool, IEnumerable<WeatherResponseDTO>>> ExecuteAsync()
        {
            return _weatherServiсe.GetWeatherByArrayCityNameAsync(_cityNames);
        }
    }
}
