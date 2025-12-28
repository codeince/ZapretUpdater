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
                .Where(s => Uri.TryCreate(s, UriKind.Absolute, out _) || Uri.TryCreate($"http://{s}", UriKind.Absolute, out _));
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
                .WhereNotComment()
                .WhereUri()
                .Select(url =>
                Uri.TryCreate(url, UriKind.Absolute, out var result) ? result! : new Uri($"http://{url}"));
                //{
                //    url = url.Trim().Trim("\\/?,.<>:;").ToString();
                //    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var result))
                //        return result!;
                //    else if (Uri.TryCreate($"http://{url}", UriKind.RelativeOrAbsolute, out var resultWithHttp))
                //        return resultWithHttp!;
                //    else
                //    {
                //        try
                //        {
                //            return new Uri($"http://{url}");
                //        }
                //        catch (UriFormatException)
                //        {
                //            throw new UriFormatException($"Invalid URL format: {url}");
                //        }
                //    }
                //});
        }

        public static IEnumerable<string> SelectHosts(this IEnumerable<string> list, bool isDns = true)
        {
            return list
                .SelectUri()
                .Where(uri =>
                {
                    if (isDns)
                        return uri.HostNameType == UriHostNameType.Dns;
                    else
                        return uri.HostNameType == UriHostNameType.IPv4 || uri.HostNameType == UriHostNameType.IPv6;
                })
                .Select(url =>
                {
                    if (!url.IsAbsoluteUri && Uri.TryCreate($"http://{url}", UriKind.Absolute, out var absUrl))
                        return absUrl.DnsSafeHost;
                    if (url.IsAbsoluteUri)
                        return url.DnsSafeHost;

                    Console.WriteLine(url);
                    return url.AbsolutePath;
                });
            //.Select(url => url[(url.Contains(Uri.SchemeDelimiter) ? (url.IndexOf(Uri.SchemeDelimiter) + 1) : 0)..].Replace('\\', '/').Split('/')[0]);
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
