using ZapretUpdater.Zapret.FTS;

namespace ZapretUpdater.Zapret.Lists
{
    internal class DomainGoogleList : IBaseList
    {
        public string Id => "domaingooglelist";
        public string FileName => "list-google.txt";

        private static HashSet<Uri> _urls = FTSInterpreter.ReadCode(
@"@github#remittor/zapret-openwrt+zapret/ipset/zapret-hosts-google.txt+zap1
@github#Flowseal/zapret-discord-youtube+lists/list-google.txt
@github#sch-izo/shizapret+lists/list-google.txt");
        private static HashSet<string> set = [];

        HashSet<Uri> IBaseList.Urls { get => _urls; set => _urls = value; }
        HashSet<string> IBaseList.Set { get => set; set => set = value; }
    }
}
