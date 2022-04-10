using DataAccessLayer.Models;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.Abstract
{
    public interface IWeatherRepository
    {
        Task AddWeatherAsync(Weather weather);
    }
}
