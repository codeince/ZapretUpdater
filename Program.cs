using ZapretUpdater.Zapret;

namespace ZapretUpdater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool loadLists = !(args.Contains("-n") | args.Contains("--new"));
            bool ask = !(args.Contains("-s") || args.Contains("--skip-asking"));

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

            Console.Write("Нажмите любую клавишу для продолжения...");
            _ = Console.Read();

        }
    }
}
