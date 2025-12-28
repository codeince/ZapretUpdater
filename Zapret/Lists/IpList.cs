using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class IpList : IBaseList
    {
        public string Id => "iplist";
        public string FileName => "ipset-all.txt";

        private static HashSet<Uri> _urls = FTSInterpreter.ReadCode(
@"https://cloudflare.com/ips-v{ip}
https://antifilter.download/list/ipresolve.lst
https://antifilter.download/list/allyouneed.lst
https://iplist.opencck.org/?format=text&data=cidr{ip}
@github#bol-van/rulist+reestr_smart{ip}.txt
@github#V3nilla/IPSets-For-Bypass-in-Russia+ipset-all.txt
@github#Flowseal/zapret-discord-youtube+.service/ipset-service.txt
@github#V3nilla/IPSets-For-Bypass-in-Russia+Разблокировка множества сервисов(пример - ChatGPT)/Адреса для zapret под Instagram.txt");

        private static HashSet<string> set = [];


        HashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        HashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
