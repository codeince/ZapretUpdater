using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class DomainList : IBaseList
    {
        public string Id => "domainlist";
        public string FileName => "list-general.txt";

        private static ConcurrentHashSet<Uri> _urls = [.. FTSInterpreter.ReadCode(
@"
@pastebin#SiLUnT9P
https://antifilter.download/list/domains.lst
https://iplist.opencck.org/?format=text&data=domains
https://community.antifilter.download/list/domains.lst
https://github.com/1andrevich/Re-filter-lists/releases/latest/download/domains_all.lst
@github#bol-van/rulist+reestr_hostname.txt
@github#azzimoda/zapret-lists+list-general.txt
@github#1andrevich/Re-filter-lists+community.lst
@github#1andrevich/Re-filter-lists+domains_all.lst
@github#1andrevich/Re-filter-lists+ooni_domains.lst
@github#Flowseal/zapret-discord-youtube+lists/list-general.txt
@github#remittor/zapret-openwrt+zapret/ipset/zapret-hosts-user.txt+zap1
@github#V3nilla/IPSets-For-Bypass-in-Russia+Список доменов для обхода/Сам список.txt
@github#V3nilla/IPSets-For-Bypass-in-Russia+Разблокировка множества сервисов(пример - ChatGPT)/Адреса для zapret под Instagram.txt
").SelectUri()];

        private static ConcurrentHashSet<string> set = [];


        ConcurrentHashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        ConcurrentHashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
