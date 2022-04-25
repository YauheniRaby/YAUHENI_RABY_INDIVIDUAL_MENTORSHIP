using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Exceptions
{
    public class BackgroundJobException : Exception
    {
        public BackgroundJobException(IEnumerable<WeatherResponseDTO> failResponse)
            : base(string.Join(Environment.NewLine, failResponse.Select(x => $"{x.CityName}: {x.ResponseStatus} - {x.ErrorMessage};")))
        {
            
        }
    }
}
