using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberTileGame
{
    public static class Extensions
    {
        public static string ToHumanReadableString<T>(this ICollection<T> collection)
        {
            if (collection.Count == 0)
            {
                return "<None>";
            }
            
            StringBuilder stringBuilder = new StringBuilder();

            int i = 0;
            foreach (T item in collection)
            {
                stringBuilder.AppendLine($"[{i}]: {item}");
                i++;
            }

            return stringBuilder.ToString();
        }
    }
}