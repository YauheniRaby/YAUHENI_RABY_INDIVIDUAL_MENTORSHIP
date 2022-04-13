using BusinessLayer.Models;
using System.Collections.Generic;

namespace BusinessLayer.Services.Abstract
{
    public interface IBackgroundJobService
    {
        void UpdateJobs(IEnumerable<CityOption> options);
    }
}
