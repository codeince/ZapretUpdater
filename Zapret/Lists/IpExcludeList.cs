using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class IpExcludeList : IBaseList
    {
        public string Id => "iplist_exclude";
        public string FileName => "ipset-exclude.txt";

        private static HashSet<Uri> _urls = FTSInterpreter.ReadCode(
@"@github#sch-izo/shizapret+lists/ipset-exclude.txt
@github#V3nilla/IPSets-For-Bypass-in-Russia+exclude.txt
@github#Flowseal/zapret-discord-youtube+lists/ipset-exclude.txt
@github#hxehex/russia-mobile-internet-whitelist+cidrwhitelist.txt
@github#remittor/zapret-openwrt+zapret/ipset/zapret-ip-exclude.txt+zap1
@github#bol-van/zapret2+ipset/zapret-hosts-user-exclude.txt.default+master");

        private static HashSet<string> set = [];


        HashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        HashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
