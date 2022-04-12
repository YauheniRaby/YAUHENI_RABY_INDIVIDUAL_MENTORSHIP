using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.Abstract
{
    public interface IWeatherRepository
    {
        Task BulkSaveWeatherAsync(IEnumerable<Weather> weather);
    }
}
