using System.Threading.Tasks;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Services.Abstract;

namespace BusinessLayer.Command
{
    public class ForecastWeatherCommand : ICommand
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private string _cityName;
        private int _countDay;

        public ForecastWeatherCommand(IWeatherServiсe weatherServiсe, string cityName, int countDay)
        {
            _weatherServiсe = weatherServiсe;
            _cityName = cityName;
            _countDay = countDay;
        }

        public async Task<object> ExecuteAsync()
        {
            return await _weatherServiсe.GetForecastByCityNameAsync(_cityName, _countDay);
        }
    }
}
