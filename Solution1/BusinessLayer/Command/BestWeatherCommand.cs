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
    public class BestWeatherCommand : ICommand<Dictionary<bool, List<WeatherResponseDTO>>>
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly string _arrayCityNames;

        public BestWeatherCommand(IWeatherServiсe weatherServiсe, string arrayCityNames)
        {
            _weatherServiсe = weatherServiсe;
            _arrayCityNames = arrayCityNames;
        }

        public Task<Dictionary<bool, List<WeatherResponseDTO>>> ExecuteAsync()
        {
            return _weatherServiсe.GetWeatherByArrayCityNameAsync(_arrayCityNames);
        }
    }
}
