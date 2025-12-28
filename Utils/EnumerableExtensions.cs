using System.Net;

namespace ZapretUpdater.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<string> WhereNotEmpty(this IEnumerable<string> list)
        {
            return list.Where(x => !string.IsNullOrWhiteSpace(x));
        }

        public static IEnumerable<string> WhereNotComment(this IEnumerable<string> list)
        {
            return list.Where(x => !x.StartsWith('#'));
        }

        public static IEnumerable<string> WhereUri(this IEnumerable<string> list)
        {
            return list
                .WhereNotEmpty()
                .SelectTrim()
                //.Where(s => IPAddress.TryParse(s, out _) || Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out _));
                .Where(s => Uri.IsWellFormedUriString(s, UriKind.RelativeOrAbsolute));
        }

        public static IEnumerable<string> DistinctSet(this IEnumerable<string> list)
        {
            return list
                .WhereUri()
                .Distinct();
        }

        public static IEnumerable<Uri> DistinctSet(this IEnumerable<Uri> list)
        {
            return list.DistinctBy(value => value.AbsoluteUri);
        }

        public static IEnumerable<string> SelectTrim(this IEnumerable<string> list)
        {
            return list.Select(s => s.Trim())
                .WhereNotEmpty();
        }

        public static IEnumerable<Uri> SelectUri(this IEnumerable<string> list)
        {
            return list
                .WhereUri()
                .Select(url =>
                {
                    try
                    {
                        return new Uri(url);
                    }
                    catch
                    {
                        return new Uri("http://" + url);
                    }
                });
        }

        public static IEnumerable<string> SelectHosts(this IEnumerable<string> list)
        {
            return list
                .SelectUri()
                .Select(url => url.DnsSafeHost);
            //.Select(url => url[(url.Contains(Uri.SchemeDelimiter) ? (url.IndexOf(Uri.SchemeDelimiter) + 1) : 0)..].Replace('\\', '/').Split('/')[0]);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }

        public static bool Contains(this IEnumerable<Uri> list, Uri value)
        {
            return list.Any(item => item.AbsoluteUri.Equals(value.AbsoluteUri, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
