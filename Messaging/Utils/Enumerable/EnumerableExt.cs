using System.Text;

namespace Utils.Enumerable
{
    public static class EnumerableExt
    {
        public static bool HasItem<T>(this IEnumerable<T>? list)
        {
            if (list == null)
            {
                return false;
            }

            return list.Count() > 0;
        }

        public static string SplitToString<T>(this IEnumerable<T>? list, string separater = ",")
        {
            if (!list.HasItem())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.AppendJoin(separater, list!.ToArray());

            return sb.ToString();
        }
    }
}
