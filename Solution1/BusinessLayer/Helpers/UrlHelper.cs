namespace BusinessLayer.Helpers
{
    public static class UrlHelper
    {
        public static string Combine(params string[] partsUsl)
        {
            return string.Join(null, partsUsl);
        }
    }
}
