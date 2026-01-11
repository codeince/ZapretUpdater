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
                .Where(s => Uri.TryCreate(s, UriKind.Absolute, out _) || Uri.TryCreate(Uri.UriSchemeHttp + Uri.SchemeDelimiter + s, UriKind.Absolute, out _));
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
                .WhereNotEmpty()
                .WhereNotComment()
                .SelectTrim()
                .WhereUri()
                .Select(url =>
                {
                    var uriBuilder = new UriBuilder(url.Trim());
                    if (string.IsNullOrEmpty(uriBuilder.Scheme))
                    {
                        uriBuilder.Scheme = Uri.UriSchemeHttp;
                    }
                    return uriBuilder.Uri;
                });
        }

        public static IEnumerable<string> SelectHosts(this IEnumerable<string> list)
        {
            return list
                .SelectUri()
                .Where(uri => uri.HostNameType == UriHostNameType.Dns)
                .Select(url => url.DnsSafeHost);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }

        public static ConcurrentHashSet<T> ToConcurrentHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
            where T : notnull
        {
            return new ConcurrentHashSet<T>(source, comparer);
        }

        public static bool Contains(this IEnumerable<Uri> list, Uri value)
        {
            return list.Any(item => item.AbsoluteUri.Equals(value.AbsoluteUri, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
