using BusinessLayer.DTOs;
using System.Collections.Generic;

namespace BusinessLayer.Services.Abstract
{
    public interface IBackgroundJobService
    {
        void UpdateJobs(IEnumerable<CityOptionDTO> options);
    }
}
