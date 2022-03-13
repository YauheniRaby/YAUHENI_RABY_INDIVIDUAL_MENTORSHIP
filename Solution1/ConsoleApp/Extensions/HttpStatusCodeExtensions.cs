using System.Net;

namespace ConsoleApp.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static string GetStringRepresentation(this HttpStatusCode httpStatusCode)
        {
            return $"{(int)httpStatusCode} {httpStatusCode}";
        }
    }
}
