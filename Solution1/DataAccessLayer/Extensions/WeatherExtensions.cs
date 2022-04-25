using DataAccessLayer.Helpers;
using DataAccessLayer.Models;

namespace DataAccessLayer.Extensions
{
    public static class WeatherExtensions
    {
        public static Weather FillCommentByTemp(this Weather weather)
        {
            weather.Comment = CommentHelper.GetComment(weather.Temp);
            return weather;
        }
    }
}
