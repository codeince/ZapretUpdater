using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class DomainExcludeList : IBaseList
    {
        public string Id => "domainlistexclude";
        public string FileName => "list-exclude.txt";

        private static ConcurrentHashSet<Uri> _urls = [.. FTSInterpreter.ReadCode(
@"
@pastebin#iTqfMKc0
@github#sch-izo/shizapret+lists/list-exclude.txt
@github#HotCakeX/MicrosoftDomains+Microsoft%20Domains.txt
@github#hxehex/russia-mobile-internet-whitelist+whitelist.txt
@github#Flowseal/zapret-discord-youtube+lists/list-exclude.txt
@github#V3nilla/IPSets-For-Bypass-in-Russia+exclude-domains.txt
@github#remittor/zapret-openwrt+zapret/ipset/zapret-hosts-user-exclude.txt+zap1
").SelectUri()];

        private static ConcurrentHashSet<string> set = [];


        ConcurrentHashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        ConcurrentHashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
