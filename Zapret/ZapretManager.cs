using ZapretUpdater.Zapret.Lists;

namespace ZapretUpdater.Zapret
{
    internal static class ZapretManager
    {
        private static readonly IBaseList _ipList = new IpList();
        private static readonly IBaseList _domainList = new DomainList();
        private static readonly IBaseList _domainGoogleList = new DomainGoogleList();
        private static readonly IBaseList _ipExcludeList = new IpExcludeList();
        private static readonly IBaseList _domainExcludeList = new DomainExcludeList();

        public static List<IBaseList> Lists = [_ipList, _domainList, _domainGoogleList, _ipExcludeList, _domainExcludeList];

        public static int GetListIndexById(string id)
        {
            return Lists.IndexOf(Lists.First(list => list.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        public static void LoadAllLists()
        {
            Lists.AsParallel().ForAll(list => list.LoadList());
        }

        public static void FindExtraSources()
        {
            Lists.AsParallel().ForAll(list => list.FindExtraSources());
        }

        public static void DownloadAllLists()
        {
            Lists.AsParallel().ForAll(list => list.DownloadList());

            _ipList.Set = [.. _ipList.Set.Except(_ipExcludeList.Set)];
            _domainList.Set = [.. _domainList.Set
                .Except(_domainGoogleList.Set)
                .Except(_domainExcludeList.Set)];

            Console.WriteLine($"{_domainList.Set.Count} domains and {_ipList.Set.Count} IP ranges loaded.\n{_domainExcludeList.Set.Count} excluded domains and {_ipExcludeList.Set.Count} excluded IP ranges loaded.");
        }

        public static void SaveAllLists()
        {
            Lists.AsParallel().ForAll(list => list.SaveToFile());
        }
    }
}

