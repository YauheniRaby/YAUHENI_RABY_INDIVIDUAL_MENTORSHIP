using System;

namespace Weather.Tests.Infrastructure
{
    public static class Menu
    {
        public static string GetMenuRepresentation()
        {
            return "Select menu item:" +
                 $"{Environment.NewLine}0 - Exit" +
                 $"{Environment.NewLine}1 - Get currently weather" +
                 $"{Environment.NewLine}2 - Get weather for a period of time";
        }
    }
}
