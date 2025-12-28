using System.Collections.Immutable;
using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    public static class ZapretListExtensions
    {
        public static void FindExtraSources(this IBaseList list)
        {
            var extraFileName = list.FileName.Replace(".txt", ".sources");
            if (File.Exists(extraFileName))
            {
                Console.WriteLine($"[{extraFileName}] Extra sources file found. Loading...");
                var data = File.ReadAllText(extraFileName);

                if (string.IsNullOrWhiteSpace(data))
                {
                    Console.WriteLine($"[WARNING] {extraFileName} is empty.");
                }
                else
                {
                    data = data.ReplaceLineEndings().Trim();

                    var values = FTSInterpreter.ReadCode(data).Except(list.Urls).ToList();
                    if (values.Count > 0)
                    {
                        Console.WriteLine($"[{extraFileName}] {values.Count} extra sources found.");
                        foreach (var url in values)
                            list.Urls.Add(url);
                        list.Urls = [.. list.Urls.DistinctSet()];
                    }

                    var AntiSources = FTSInterpreter.ReadCode(data, inverted: true);
                    if (AntiSources.Count > 0)
                    {
                        Console.WriteLine($"[!{extraFileName}] {AntiSources.Count} anti-sources found. Loading and downloading...");
                        IBaseList AntiList = new IpList();
                        AntiList.Urls = AntiSources;
                        AntiList.DownloadList();
                        list.Set = [.. list.Set
                            .WhereNotEmpty()
                            .SelectTrim()
                            .Except(AntiList.Set)];
                    }
                }
            }
        }
        public static void LoadList(this IBaseList list)
        {
            if (File.Exists(list.FileName))
            {
                Console.WriteLine($"[{list.FileName}] File already exists!\n[{list.FileName}] Loading...");
                var data = File.ReadAllText(list.FileName);

                if (string.IsNullOrWhiteSpace(data))
                {
                    Console.WriteLine($"[WARNING] {list.FileName} is empty.");
                }
                else
                {
                    data = data.ReplaceLineEndings().Trim();
                    var values = data.Split(Environment.NewLine)
                        .WhereNotEmpty()
                        .SelectTrim()
                        .ToList();
                    Console.WriteLine($"[{list.FileName}] {values.Count} values");
                    foreach (var url in values)
                        list.Set.Add(url);
                }

                list.Set = [.. list.Set.DistinctSet()];
            }
        }

        public static async Task DownloadUrl(this IBaseList list, Uri url)
        {
            using HttpClient webclient = new();

            try
            {
                var data = await webclient.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(data))
                {
                    Console.WriteLine($"[WARNING] {url} returned empty data.");
                }
                else
                {
                    data = data.ReplaceLineEndings().Trim();
                    var values = data.Split(Environment.NewLine).Distinct().ToList();
                    Console.WriteLine($"[{url.Host}] {values.Count} values");
                    foreach (var value in values)
                        list.Set.Add(value);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($@"[ERROR] Host {url.Host} is unknown.
Check your internet connection and try again.
Got error: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($@"[ERROR] This url has no GET method
Got error: {e.Message}");
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($@"[ERROR] Operation canceled
Got error: {e.Message}");
            }
        }

        public static void DownloadList(this IBaseList list)
        {
            Parallel.ForEachAsync(list.Urls, async (url, _) =>
            {
                Console.WriteLine($"[{list.FileName}] Downloading {url}...");
                await list.DownloadUrl(url);
            }).Wait();
            list.Set = [.. list.Set.DistinctSet()];
        }

        public static void SaveToFile(this IBaseList list)
        {
            if (list.Set.Count > 0)
            {
                list.Set = [.. list.Set.SelectHosts(list.Id.StartsWith("domain"))];
                list.Set.Add(Environment.NewLine);
                list.Set = [.. list.Set.DistinctSet()];

                File.WriteAllLines(list.FileName, list.Set.ToImmutableSortedSet());
            }
        }
    }
}