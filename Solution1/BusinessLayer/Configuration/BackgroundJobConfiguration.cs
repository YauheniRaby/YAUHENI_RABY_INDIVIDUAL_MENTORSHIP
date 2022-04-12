using System.Collections.Generic;

namespace BusinessLayer.Configuration
{
    public class BackgroundJobConfiguration
    {
        public IEnumerable<CityOption> CitiesOptions { get; set; }
    }
    
    public class CityOption
    {
        public string CityName { get; set; }

        public int Timeout { get; set; }
    }
}