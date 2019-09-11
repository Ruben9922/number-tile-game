using System.Collections.Generic;
using System.Text;

namespace NumberTileGame
{
    public static class Extensions
    {
        public static string ToHumanReadableString<T>(this IEnumerable<T> enumerable)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            int i = 0;
            foreach (T item in enumerable)
            {
                stringBuilder.AppendLine($"[{i}]: {item}");
                i++;
            }
            
            return stringBuilder.ToString();
        }
    }
}