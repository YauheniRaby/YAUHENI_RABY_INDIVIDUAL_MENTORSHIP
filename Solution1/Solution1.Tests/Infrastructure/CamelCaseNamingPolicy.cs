using System.Text.Json;

namespace Weather.Tests.Infrastructure
{
    public class CamelCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string propertyName)
        {
            if (propertyName == "DateTime")
            {
                return "dt_txt";
            }
            return propertyName.ToLower();
        }            
    }
}
