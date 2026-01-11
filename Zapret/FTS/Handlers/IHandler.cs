namespace ZapretUpdater.Zapret.FTS.Handlers
{
    public interface IHandler
    {
        string Id { get;}
        int ArgumentCount { get; }

        Uri GetUri(string[] arguments);

        bool CanZipUri(Uri uri);
        string ZipUri(Uri uri);
    }
}
