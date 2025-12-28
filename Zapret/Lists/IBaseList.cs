namespace ZapretUpdater.Zapret.Lists
{
    public interface IBaseList
    {
        public string Id { get; }
        public HashSet<Uri> Urls { get; set; }

        public HashSet<string> Set { get; set; }

        public string FileName { get; }
    }
}
