namespace ZapretUpdater.Zapret.FTS.Handlers
{
    public class PastebinHandler: IHandler
    {
        public string Id => "pastebin";
        public int ArgumentCount => 1;

        public Uri GetUri(string[] arguments)
        {
            if (arguments.Length < ArgumentCount)
            {
                throw new ArgumentException($"Handler \"{Id}\" requires {ArgumentCount} argument: <paste-key>");
            }
            string pasteKey = arguments[0];
            return new Uri($"https://pastebin.com/raw/{pasteKey}");
        }
    }
}
