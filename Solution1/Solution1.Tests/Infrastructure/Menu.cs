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
                "\r\n0 - Exit" +
                "\r\n1 - Get currently weather" +
                "\r\n2 - Get weather for a period of time";
        }
    }
}
