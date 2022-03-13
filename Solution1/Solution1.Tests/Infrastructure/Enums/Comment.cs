using System.ComponentModel;

namespace Weather.Tests.Infrastructure.Enums
{
    enum WeatherComments
    {
        [Description("Dress warmly.")]
        DressWarmly,

        [Description("It's fresh.")]
        Fresh,

        [Description("Good weather.")]
        GoodWeather,

        [Description("It's time to go to the beach.")]
        GoToBeach
    }    
}
