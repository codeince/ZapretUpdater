using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class IpExcludeList : IBaseList
    {
        public string Id => "iplistexclude";
        public string FileName => "ipset-exclude.txt";

        private static ConcurrentHashSet<Uri> _urls = [.. FTSInterpreter.ReadCode(
@"@github#V3nilla/IPSets-For-Bypass-in-Russia+exclude.txt
@github#Flowseal/zapret-discord-youtube+lists/ipset-exclude.txt
@github#hxehex/russia-mobile-internet-whitelist+cidrwhitelist.txt
@github#remittor/zapret-openwrt+zapret/ipset/zapret-ip-exclude.txt+zap1
@github#bol-van/zapret2+ipset/zapret-hosts-user-exclude.txt.default+master").SelectUri()];

        private static ConcurrentHashSet<string> set = [];


        ConcurrentHashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        ConcurrentHashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
