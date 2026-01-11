using ZapretUpdater.Zapret;
using ZapretUpdater.Zapret.Lists;

namespace ZapretUpdater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Directory.EnumerateFiles(".", "list-*.sources").ToList().ForEach(sourcePath =>
            {
                var filename = sourcePath.Replace(".\\", "").ToLower().Trim().Replace(".sources", ".txt");
                if (!ZapretManager.DomainLists.Select(x => x.FileName).Contains(filename))
                {
                    var content = File.ReadAllText(sourcePath);
                    ZapretManager.DomainLists.Add(new DomainExtraList(filename, content));
                }
            });
            Directory.EnumerateFiles(".", "ipset-*.sources").ToList().ForEach(sourcePath =>
            {
                var filename = sourcePath.Replace(".\\", "").ToLower().Trim().Replace(".sources", ".txt");
                if (!ZapretManager.IpLists.Select(x => x.FileName).Contains(filename))
                {
                    var content = File.ReadAllText(sourcePath);
                    ZapretManager.IpLists.Add(new IpExtraList(filename, content));
                }
            });

            ZapretManager.LoadAllLists();
            ZapretManager.FindExtraSources();
            ZapretManager.DownloadAllLists();
            ZapretManager.SaveAllLists();

            if (args.Contains("-p")) return;
            Console.Write("Нажмите любую клавишу для продолжения...");
            _ = Console.Read();
        }
    }
}
