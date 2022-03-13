using System;
using System.ComponentModel;

namespace Weather.Tests.Infrastructure.Extensions
{
    public static class AttributesHelperExtension
    {
        public static string GetString(this Enum value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }
    }
}
