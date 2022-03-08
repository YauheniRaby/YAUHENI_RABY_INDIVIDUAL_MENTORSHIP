using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Tests.Infrastructure
{
    public static class Menu
    {
        public static string GetMenuRepresentation()
        {
            return "Select menu item:" +
                "\r\n0 - Get currently weather" +
                "\r\n1 - Get weather for a period of time" +
                "\r\n2 - Exit";
        }
    }
}
