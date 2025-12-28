
namespace ZapretUpdater.Zapret.FTS.Handlers
{
    public class GithubHandler : IHandler
    {
        public string Id => "github";

        public int ArgumentCount => 2;

        public Uri GetUri(string[] arguments)
        {
            if (arguments.Length < ArgumentCount)
            {
                throw new ArgumentException($"Handler \"{Id}\" requires {ArgumentCount} arguments: <repository>+<file-path>");
            }
            string repository = arguments[0];
            string path = arguments[1];
            string branch = arguments.Length >= 3 ? arguments[2] : "main";
            return new Uri($"https://raw.githubusercontent.com/{repository}/{branch}/{path}");
        }
    }
}
