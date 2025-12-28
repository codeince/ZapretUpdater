using ZapretUpdater.Zapret;

namespace ZapretUpdater
{
    internal class Program
    {
        static void Main()
        {
            ZapretManager.LoadAllLists();
            ZapretManager.FindExtraSources();
            ZapretManager.DownloadAllLists();
            ZapretManager.SaveAllLists();

            Console.Write("Нажмите любую клавишу для продолжения...");
            _ = Console.Read();
        }
    }
}
