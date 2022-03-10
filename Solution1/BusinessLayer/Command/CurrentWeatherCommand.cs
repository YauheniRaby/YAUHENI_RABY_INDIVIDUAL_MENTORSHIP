using System.Threading.Tasks;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Services.Abstract;

namespace BusinessLayer.Command
{
    public class CurrentWeatherCommand : ICommand
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly string _cityName;

        public CurrentWeatherCommand(IWeatherServiсe weatherServiсe, string cityName)
        {
            _weatherServiсe = weatherServiсe;
            _cityName = cityName;
        }

        public async Task<object> ExecuteAsync()
        {
            return await _weatherServiсe.GetByCityNameAsync(_cityName);            
        }
    }
}
