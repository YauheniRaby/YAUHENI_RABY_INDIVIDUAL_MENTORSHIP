using DataAccessLayer.Abstract;
using DataAccessLayer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class WetherRepositor : IWetherRepository
    {
        public Weather GetByCityName(string cityName)
        {
            var request = WebRequest.Create($"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={Const.KeyApi}&units=metric");
            WebResponse response = request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<Weather>(sr.ReadToEnd());
            }
        }
    }
        

    
}
