using BusinessLayer.DTOs;
using DataAccessLayer.Helpers;

namespace BusinessLayer.Extensions
{
    public static class BaseWeatherDTOExtensions
    {
        public static T FillCommentByTemp <T> (this T weatherDTO)
            where T : BaseWeatherDTO
        {
            weatherDTO.Comment = CommentHelper.GetComment(weatherDTO.Temp); 
            return weatherDTO;
        }
    }
}
