using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class DomainExcludeList : IBaseList
    {
        public string Id => "domainlist_exclude";
        public string FileName => "list-exclude.txt";

        private static HashSet<Uri> _urls = FTSInterpreter.ReadCode(
@"@github#sch-izo/shizapret+lists/list-exclude.txt
@github#hxehex/russia-mobile-internet-whitelist+whitelist.txt
@github#Flowseal/zapret-discord-youtube+lists/list-exclude.txt
@github#V3nilla/IPSets-For-Bypass-in-Russia+exclude-domains.txt
@github#remittor/zapret-openwrt+zapret/ipset/zapret-hosts-user-exclude.txt+zap1");

        private static HashSet<string> set = [];


        HashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        HashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
