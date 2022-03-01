using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Extensions
{
    public static class Comment
    {
        public static string FillByTemp(double temp)
        {
            return temp switch
            {
                _ when temp < 0 => "Dress warmly.",
                _ when temp >= 0 && temp < 20 => "It's fresh.",
                _ when temp >= 20 && temp < 30 => "Good weather.",
                _ => "It's time to go to the beach.",
            };
        }
    }
}
