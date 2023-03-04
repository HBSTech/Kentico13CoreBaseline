using CSharpFunctionalExtensions;
using System.Linq;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static Maybe<T> FirstOrMaybe<T>(this IEnumerable<T> collection, bool defaultIsNone = true)
        {
            if (collection.Any())
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (defaultIsNone && collection.First().Equals(default(T)))
                {
                    return Maybe.None;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                return Maybe.From(collection.First());
            }
            else
            {
                return Maybe.None;
            }
        }

        public static Maybe<T> FirstOrMaybe<T>(this IEnumerable<T> collection, Func<T, bool> predicate, bool defaultIsNone = true)
        {
            var items = collection.Where(predicate);
            if (items.Any())
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (defaultIsNone && items.First().Equals(default(T)))
                {
                    return Maybe.None;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                return Maybe.From(items.First());
            }
            else
            {
                return Maybe.None;
            }
        }

        public static Maybe<IEnumerable<T>> WithEmptyAsNone<T>(this IEnumerable<T> value) =>
            value == null || !value.Any() ?
            Maybe<IEnumerable<T>>.None :
            Maybe.From(value);

        public static string StringJoin(this IEnumerable<string> collection, string separator)
        {
            return string.Join(separator, collection.ToArray());
        }
    }
}
