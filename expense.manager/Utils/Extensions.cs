using System;
using System.Collections.Generic;
using System.Linq;

namespace expense.manager.Utils
{
    public static class Extensions
    {
        public static string ToMonthId(this DateTime date)
        {
            return $"{date:MM/yyyy}";
        }


        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return !source.Any() ? source :
                source.Concat(
                    source
                        .SelectMany(i => selector(i).EmptyIfNull())
                        .SelectManyRecursive(selector)
                );
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}
