using ZapretUpdater.Zapret.Lists;

namespace ZapretUpdater.Zapret
{
    internal static class ZapretManager
    {

        public static List<IBaseList> IpLists = [new IpList(), new IpExcludeList()];
        public static List<IBaseList> DomainLists = [new DomainList(), new DomainGoogleList(), new DomainExcludeList()];

        public static List<IBaseList> Lists { get => [.. IpLists.Concat(DomainLists)]; }

        /// <summary>
        /// Loads all lists if files are exists
        /// </summary>
        public static void LoadAllLists()
        {
            Lists.AsParallel().ForAll(list => list.LoadList());
        }

        static IEnumerable<string> FindSourcesFiles()
        {
            return Directory.EnumerateFiles(".", "*-*.sources*");
        }

        /// <summary>
        /// Finds extra sources in .sources files
        /// </summary>
        public static void FindExtraSources()
        {
            FindSourcesFiles().AsParallel().ForAll(sourcePath =>
            {
                var filename = Path.GetFileNameWithoutExtension(sourcePath) + ".txt";
                var content = File.ReadAllText(sourcePath);

                if (filename.StartsWith("list"))
                {
                    if (DomainLists.Any(x => x.FileName == filename))
                    {
                        DomainLists = [.. DomainLists.Select(list =>
                        {
                            if (list.FileName == filename)
                            {
                                list.FindExtraSources(content);
                            }

                            return list;
                        })];
                    }
                    else
                    {
                        DomainLists.Insert(DomainLists.Count - 2, new DomainExtraList(filename, content));
                    }
                }
                else if (filename.StartsWith("ipset"))
                {
                    if (IpLists.Any(x => x.FileName == filename))
                    {
                        IpLists = [.. IpLists.Select(list =>
                        {
                            if (list.FileName == filename)
                            {
                                list.FindExtraSources(content);
                            }

                            return list;
                        })];
                    }
                    else
                    {
                        IpLists.Insert(IpLists.Count - 2, new IpExtraList(filename, content));
                    }
                }
            });
        }

        /// <summary>
        /// Fetches all lists sources' ips and domains
        /// </summary>
        public static void DownloadAllLists()
        {
            Lists.AsParallel().ForAll(list => list.DownloadList());

            IpLists[0].Set = [.. IpLists[0].Set.Except(
                IpLists[1..].SelectMany(x => x.Set)
            )];

            DomainLists[0].Set = [.. DomainLists[0].Set.Except(
                DomainLists[1..].SelectMany(x => x.Set)
            )];
        }

        public static void FindAntiSources()
        {
            FindSourcesFiles().AsParallel().ForAll(sourcePath =>
            {
                var filename = Path.GetFileNameWithoutExtension(sourcePath).ToLower() + ".txt";
                var content = File.ReadAllText(sourcePath);

                if (!content.Contains('!')) return;

                if (filename.StartsWith("list"))
                {
                    if (DomainLists.Any(x => x.FileName == filename))
                    {
                        DomainLists = [.. DomainLists.Select(list =>
                        {
                            if (list.FileName == filename)
                            {
                                list.FindAntiSources(content);
                            }

                            return list;
                        })];
                    }
                }
                else if (filename.StartsWith("ipset"))
                {
                    if (IpLists.Any(x => x.FileName == filename))
                    {
                        IpLists = [.. IpLists.Select(list =>
                        {
                            if (list.FileName == filename)
                            {
                                list.FindAntiSources(content);
                            }

                            return list;
                        })]; ;
                    }
                }
            });
        }

        /// <summary>
        /// Saves all lists to files
        /// </summary>
        public static void SaveAllLists()
        {
            Lists.AsParallel().ForAll(list => list.SaveToFile());

            IpLists.ForEach(x => Console.WriteLine($"[{x.FileName}] {x.Set.Count} ips"));
            DomainLists.ForEach(x => Console.WriteLine($"[{x.FileName}] {x.Set.Count} domains"));
        }
    }
}

