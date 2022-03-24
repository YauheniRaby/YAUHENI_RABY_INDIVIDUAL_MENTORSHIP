using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.Enum;
using BusinessLayer.Services.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Command
{
    public class BestWeatherCommand : ICommand<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>>
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IEnumerable<string> _cityNames;

        public BestWeatherCommand(IWeatherServiсe weatherServiсe, IEnumerable<string> cityNames)
        {
            _weatherServiсe = weatherServiсe;
            _cityNames = cityNames;
        }

        public Task<Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>> ExecuteAsync()
        {
            return _weatherServiсe.GetWeatherByArrayCityNameAsync(_cityNames);
        }
    }
}
