using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class DomainGoogleList : IBaseList
    {
        public string Id => "domainlistgoogle";
        public string FileName => "list-google.txt";

        private static ConcurrentHashSet<Uri> _urls = [.. FTSInterpreter.ReadCode(
@"
@github#remittor/zapret-openwrt+zapret/ipset/zapret-hosts-google.txt+zap1
@github#Flowseal/zapret-discord-youtube+lists/list-google.txt
@github#sch-izo/shizapret+lists/list-google.txt
").SelectUri()];
        private static ConcurrentHashSet<string> set = [];

        ConcurrentHashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        ConcurrentHashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
