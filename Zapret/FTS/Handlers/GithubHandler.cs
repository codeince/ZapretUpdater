
using ZapretUpdater.Utils;

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
                throw new ArgumentException($"Handler \"{Id}\" requires {ArgumentCount} arguments: <repository>+<file-path>+<branch>");
            }
            string repository = arguments[0];
            string path = arguments[1];
            string branch = arguments.Length >= 3 ? arguments[2] : "main";
            return new Uri($"https://raw.githubusercontent.com/{repository}/{branch}/{path}");
        }
        public bool CanZipUri(Uri uri)
        {
            return uri.ToString().StartsWith("https://raw.githubusercontent.com/") &&
                uri.AbsolutePath.Split('/').WhereNotEmpty().ToArray().Length >= ArgumentCount + 2;
        }

        public string ZipUri(Uri uri)
        {
            var splitter = '/';
            var uriPath = uri.AbsolutePath.Split(splitter).WhereNotEmpty().ToArray();
            var repository = string.Join(splitter, uriPath[..2]);
            var branch = uriPath[2];
            var path = string.Join(splitter, uriPath[3..]);

            return $"@{Id}#{repository}+{path}+{branch}";
        }
    }
}
