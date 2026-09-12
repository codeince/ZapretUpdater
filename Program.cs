using ZapretUpdater.Zapret;
using ZapretUpdater.Zapret.Api;

namespace ZapretUpdater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            KeyContainer.GetApiKeys();

            bool loadLists = !(args.Contains("-n") || args.Contains("--new"));
            bool ask = !(args.Contains("-s") || args.Contains("--skip-asking"));
            bool clearDefault = args.Contains("-c") || args.Contains("--clear-default");

            if (clearDefault)
            {
                ZapretManager.ClearLists();
            }

            if (loadLists && Directory.EnumerateFiles(".", "*.txt").Count() > 0)
            {
                string? choice = "y";
                if (ask)
                {
                    Console.WriteLine("Read the lists(yes/no)?");
                    choice = Console.ReadLine();
                }

                if (string.IsNullOrEmpty(choice) || choice.StartsWith('y'))
                    ZapretManager.LoadAllLists();
            }

            ZapretManager.FindExtraSources();
            ZapretManager.DownloadAllLists();
            ZapretManager.FindAntiSources();
            ZapretManager.SaveAllLists();

            if (args.Contains("-p") || args.Contains("--pause")) return;

            Console.Write("Press any button to continue...");
            _ = Console.Read();

        }
    }
}
