using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS.Handlers;

namespace ZapretUpdater.Zapret.FTS
{
    public static class FTSInterpreter
    {
        public static readonly IHandler[] Handlers =
        {
            new GithubHandler(),
            new PastebinHandler(),
        };
        private static string ReadLine(string line, bool inverted = false)
        {
            if (line.StartsWith('@'))
            {
                line = line[1..];
                if (!inverted == line.StartsWith('!')) return string.Empty;

                if (inverted)
                {
                    line = line[1..];
                }

                var handlerId = line[..line.IndexOf('#')];
                var argumentsPart = line[(line.IndexOf('#') + 1)..].Split('+')
                        .SelectTrim()
                        .ToArray();

                line = GetHandlerById(handlerId)?.GetUri(
                    argumentsPart
                ).AbsoluteUri ?? throw new ArgumentException($"Unknown handler ID: {handlerId}");
            }
            else
            {
                return string.Empty;
            }

            return line;
        }

        public static string ZipUri(Uri uri)
        {
            foreach (var handler in Handlers)
            {
                if (handler.CanZipUri(uri))
                    return handler.ZipUri(uri);
            }

            return uri.ToString();
        }

        public static IEnumerable<string> ReplaceIps(string line)
        {
            if (line.Contains("{ip}"))
            {
                return [line.Replace("{ip}", "4"), line.Replace("{ip}", "6")];
            }
            else
            {
                return [line];
            }
        }

        public static ConcurrentHashSet<string> ReadCode(string code, bool inverted = false)
        {
            code = code.ReplaceLineEndings();
            return code.Split(Environment.NewLine)
                .WhereNotEmpty()
                .SelectTrim()
                .SelectMany(ReplaceIps)
                .Select(url => ReadLine(url, inverted))
                .WhereNotEmpty()
                .ToConcurrentHashSet();
        }

        public static IEnumerable<string> GetUrls(string code, bool inverted = false)
        {
            code = code.ReplaceLineEndings();
            return code.Split(Environment.NewLine)
                .WhereNotEmpty()
                .SelectTrim()
                .SelectMany(ReplaceIps)
                .WhereNotEmpty()
                .Where(x => x.StartsWith('>'))
                .Select(url =>
                {
                    url = url[1..];
                    
                    if (!inverted == url.StartsWith('!')) return string.Empty;

                    if (inverted)
                    {
                        url = url[1..];
                    }

                    return url;
                })
                .WhereNotEmpty();
        }


        public static IHandler? GetHandlerById(string id)
        {
            foreach (var handler in Handlers)
            {
                if (handler.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    return handler;
                }
            }

            return null;
        }
    }
}
