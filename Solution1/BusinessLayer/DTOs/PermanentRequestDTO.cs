using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    public class PermanentRequestDTO
    {
        public IEnumerable<CityOptionDTO> CitiesOptions { get; set; }
    }
    
    public class CityOptionDTO
    {
        public string CityName { get; set; }

        public int Timeout { get; set; }
    }
}