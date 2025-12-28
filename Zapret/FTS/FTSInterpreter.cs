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
            if (!inverted && line.StartsWith("@!")) return string.Empty;

            if (inverted ? line.StartsWith("@!") : line.StartsWith('@'))
            {
                var handlerId = line.StartsWith("@!") ? line[2..line.IndexOf('#')] : line[1..line.IndexOf('#')];
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

        public static ConcurrentHashSet<Uri> ReadCode(string code, bool inverted = false)
        {
            return code.Split(Environment.NewLine)
                .WhereNotEmpty()
                .SelectTrim()
                .SelectMany(ReplaceIps)
                .Select(url => ReadLine(url, inverted))
                .WhereNotEmpty()
                .SelectUri()
                .ToConcurrentHashSet();
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
