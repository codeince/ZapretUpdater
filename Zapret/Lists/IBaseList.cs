using ZapretUpdater.Utils;

namespace ZapretUpdater.Zapret.Lists
{
    public interface IBaseList
    {
        public string Id { get; }
        public ConcurrentHashSet<Uri> Urls { get; set; }

        public ConcurrentHashSet<string> Set { get; set; }

        public string FileName { get; }
    }
}
