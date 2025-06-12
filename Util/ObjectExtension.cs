namespace Util
{
    public static class ObjectExtension
    {
        public static bool HasItem<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                return false;
            }

            return list.Count() > 0;
        }
    }
}
