using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Abstract
{
    public interface IWeatherRepository
    {
        Task BulkSaveWeatherListAsync(IEnumerable<Weather> weatherList);
    }
}
