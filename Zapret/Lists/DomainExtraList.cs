using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    public class DomainExtraList: IBaseList
    {
        public string Id => "domainlistextra";
        public string FileName => _filename;

        private static string _filename;
        private static ConcurrentHashSet<Uri> _urls;

        private static ConcurrentHashSet<string> set = [];


        ConcurrentHashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        ConcurrentHashSet<string> IBaseList.Set { get => set; set => set = value; }

        public DomainExtraList(string fileName, string sources)
        {
            _filename = fileName;
            
            _urls = [.. FTSInterpreter.ReadCode(sources).SelectUri()];
        }
    }
}
