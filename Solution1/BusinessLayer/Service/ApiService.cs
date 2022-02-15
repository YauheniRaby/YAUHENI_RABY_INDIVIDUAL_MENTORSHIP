using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service.Abstract
{
    public class ApiService : IApiService
    {
        readonly HttpClient client = new HttpClient();
        public Task<WeatherShortDTO> GetJsonByCityName(string cityName)
        {
            var urlResult = Const.UrlApi.Replace("cityName", cityName).Replace("keyApi", Const.KeyApi);

            return client.GetFromJsonAsync<WeatherShortDTO>(urlResult);            
        }
    }
}
