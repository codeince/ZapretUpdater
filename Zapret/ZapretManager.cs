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

        /// <summary>
        /// Finds extra sources in .sources files
        /// </summary>
        public static void FindExtraSources()
        {
            Lists.AsParallel().ForAll(list => list.FindExtraSources());
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

            DomainLists[0].Set = [.. DomainLists[0].Set
                .Except(
                    DomainLists[1..].SelectMany(x => x.Set))];
        }

        /// <summary>
        /// Saves all lists to files
        /// </summary>
        public static void SaveAllLists()
        {
            IpLists.ForEach(x => Console.WriteLine($"[{x.FileName}] {x.Set.Count} ips"));
            DomainLists.ForEach(x => Console.WriteLine($"[{x.FileName}] {x.Set.Count} domains"));

            Lists.AsParallel().ForAll(list => list.SaveToFile());
        }
    }
}

