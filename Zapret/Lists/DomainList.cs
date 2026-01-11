using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class DomainList : IBaseList
    {
        public string Id => "domainlist";
        public string FileName => "list-general.txt";

        private static ConcurrentHashSet<Uri> _urls = [.. FTSInterpreter.ReadCode(
@"@pastebin#SiLUnT9P
@github#bol-van/rulist+reestr_hostname.txt
https://antifilter.download/list/domains.lst
@github#azzimoda/zapret-lists+list-general.txt
https://iplist.opencck.org/?format=text&data=domains
@github#Flowseal/zapret-discord-youtube+lists/list-general.txt
@github#remittor/zapret-openwrt+zapret/ipset/zapret-hosts-user.txt+zap1
@github#V3nilla/IPSets-For-Bypass-in-Russia+Список доменов для обхода/Сам список.txt").SelectUri()];

        private static ConcurrentHashSet<string> set = [];


        ConcurrentHashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        ConcurrentHashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
