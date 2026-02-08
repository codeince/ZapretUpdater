using ZapretUpdater.Utils;
using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class IpList : IBaseList
    {
        public string Id => "iplist";
        public string FileName => "ipset-all.txt";

        private static ConcurrentHashSet<Uri> _urls = [.. FTSInterpreter.ReadCode(
@"https://cloudflare.com/ips-v{ip}
https://antifilter.download/list/ipresolve.lst
https://antifilter.download/list/allyouneed.lst
https://iplist.opencck.org/?format=text&data=cidr{ip}
https://community.antifilter.download/list/community.lst
https://github.com/1andrevich/Re-filter-lists/releases/latest/download/ipsum.lst
@github#bol-van/rulist+reestr_smart{ip}.txt
@github#1andrevich/Re-filter-lists+ipsum.lst
@github#sch-izo/shizapret+.service/ipset-all.txt
@github#1andrevich/Re-filter-lists+discord_ips.lst
@github#1andrevich/Re-filter-lists+community_ips.lst
@github#V3nilla/IPSets-For-Bypass-in-Russia+ipset-all.txt
@github#Flowseal/zapret-discord-youtube+.service/ipset-service.txt").SelectUri()];

        private static ConcurrentHashSet<string> set = [];


        ConcurrentHashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        ConcurrentHashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
