using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.Api;
using ZapretUpdater.Zapret.FTS;
using System.Net.Http.Headers;
using System.Collections.Immutable;

namespace ZapretUpdater.Zapret.Lists
{
    public static class ZapretListExtensions
    {
        /// <summary>
        /// Finds all extra sources in .sources file
        /// </summary>
        /// <param name="list">Current list</param>
        public static void FindExtraSources(this IBaseList list, string data)
        {
            data = data.ReplaceLineEndings().Trim();

            var set = FTSInterpreter.GetUrls(data).ToConcurrentHashSet();
            if (set.Count > 0)
            {
                Console.WriteLine($"[{list.FileName} extras] {set.Count} extra values found.");
                foreach (var url in set)
                    list.Set.Add(url);
            }

            var values = FTSInterpreter.ReadCode(data).SelectUri().Except(list.Urls).ToList();
            if (values.Count > 0)
            {
                Console.WriteLine($"[{list.FileName} extras] {values.Count} extra sources found.");
                foreach (var url in values)
                    list.Urls.Add(url);
                list.Urls = [.. list.Urls.DistinctSet()];
            }

        }

        /// <summary>
        /// Finds all extra sources in .sources file
        /// </summary>
        /// <param name="list">Current list</param>
        public static void FindAntiSources(this IBaseList list, string data)
        {
            data = data.ReplaceLineEndings().Trim();

            var antiSet = FTSInterpreter.GetUrls(data, true).ToConcurrentHashSet();
            if (antiSet.Count > 0)
            {
                Console.WriteLine($"[{list.FileName} extras] {antiSet.Count} anti-values found.");
                list.Set = [.. list.Set
                            .WhereNotEmpty()
                            .SelectTrim()
                            .Except(antiSet)];
            }

            var antiSources = FTSInterpreter.ReadCode(data, inverted: true).SelectUri().ToConcurrentHashSet();
            if (antiSources.Count > 0)
            {
                Console.WriteLine($"[!{list.FileName} extras] {antiSources.Count} anti-sources found. Loading and downloading...");
                IBaseList AntiList = new IpExtraList(list.Id, string.Join(Environment.NewLine, antiSources));
                AntiList.DownloadList();
                list.Set = [.. list.Set
                            .WhereNotEmpty()
                            .SelectTrim()
                            .Except(AntiList.Set)];
            }
        }

        /// <summary>
        /// Loads all ips and domains if files are exists
        /// </summary>
        /// <param name="list">Current list</param>
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
                        .WhereNotComment()
                        .SelectTrim()
                        .ToList();
                    Console.WriteLine($"[{list.FileName}] {values.Count} values");
                    foreach (var url in values)
                        list.Set.Add(url);
                }

                list.Set = [.. list.Set.Distinct()];
            }
        }

        /// <summary>
        /// Fetches all source's for ips or domains
        /// </summary>
        /// <param name="list">Current list</param>
        /// <param name="url">Source url</param>
        /// <returns></returns>
        public static async Task DownloadUrl(this IBaseList list, Uri url)
        {
            using HttpClient httpclient = new(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseCookies = false,
            });

            foreach (var apiType in KeyContainer.ApiKeys.Keys)
            {
                if (url.DnsSafeHost.Split('.').Contains(apiType, StringComparer.InvariantCultureIgnoreCase))
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", KeyContainer.ApiKeys[apiType]);
            }

            var random = new Random();

            var userAgent = new ProductInfoHeaderValue("Gecko", $"{random.Next()}");
            var comment = new ProductInfoHeaderValue("Firefox", "143.0");
            httpclient.DefaultRequestHeaders.UserAgent.Add(userAgent);
            httpclient.DefaultRequestHeaders.UserAgent.Add(comment);

            try
            {
                var data = await httpclient.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(data))
                {
                    Console.WriteLine($"[WARNING] {url} returned empty data.");
                }
                else
                {
                    data = data.ReplaceLineEndings().Trim();
                    var values = data.Split(Environment.NewLine).SelectTrim().Distinct().ToList();

                    if (values.Count == 0)
                    {
                        Console.WriteLine($"[WARNING] {url} returned no valid values.");
                        return;
                    }
                    Console.WriteLine($"[{FTSInterpreter.ZipUri(url)}] {values.Count} values");
                    foreach (var value in values)
                        list.Set.Add(value);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"[ERROR] {e.Message}\nURL: {url}");
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

        /// <summary>
        /// Fetches all list's sources for ips or domains
        /// </summary>
        /// <param name="list">Current list</param>
        public static void DownloadList(this IBaseList list)
        {
            list.Urls = [.. list.Urls.DistinctBy(url => url.AbsoluteUri)];
            Parallel.ForEachAsync(list.Urls, async (url, _) =>
            {
                await list.DownloadUrl(url);
            }).Wait();
            list.Set = [.. list.Set.Distinct()];
        }

        /// <summary>
        /// Saves list to file
        /// </summary>
        /// <param name="list">Current list</param>
        public async static void SaveToFile(this IBaseList list)
        {
            if (list.Set.Count > 0)
            {
                if (list.Id.StartsWith("ip"))
                {
                    list.Set = [.. list.Set.Except(list.Set.SelectHosts())];

                    if (list.Id == new IpList().Id)
                    {
                        list.Set = [.. list.Set.Except(["203.0.113.113/32"])];
                    }

                }
                else if (list.Id.StartsWith("domain"))
                {
                    list.Set = [.. list.Set.SelectHosts()];
                }

                list.Set.Add(Environment.NewLine);
                list.Set = [.. list.Set
                        .WhereNotEmpty()
                        .WhereNotComment()
                        .SelectTrim()
                        .Distinct()];

                if (list.Set.Count == 0)
                {
                    Console.WriteLine($"List {list.FileName} is empty!\nSkipping...");
                    return;
                }

                ImmutableSortedSet<string> sortedSet = [.. list.Set];
                await File.WriteAllLinesAsync(list.FileName, sortedSet);

                string userPath = $"{Path.GetFileNameWithoutExtension(list.FileName)}-user.{Path.GetExtension(list.FileName)}";
                if (Path.Exists(userPath))
                {
                    Console.WriteLine($"List {userPath} was found!");
                    await File.WriteAllLinesAsync(userPath, sortedSet);
                }
            }
        }
    }
}