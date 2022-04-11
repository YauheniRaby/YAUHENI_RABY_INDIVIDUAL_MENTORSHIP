using BusinessLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstract
{
    public interface IBackgroundJobService
    {
        Task UpdateJobs(IEnumerable<CityOptionDTO> options);
    }
}
