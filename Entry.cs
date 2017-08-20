using System;
using System.Text.RegularExpressions;

namespace IPTVM3UPlaylist
{
    public class Entry
    {
        private const string pattern = @"#EXTINF:(.*)\stvg-id=""(.*)""\stvg-name=""(.*)""\stvg-logo=""(.*)""\sgroup-title=""(.*)"",(.*)";
        private const string format = @"#EXTINF:{0} tvg-id=""{1}"" tvg-name=""{2}"" tvg-logo=""{3}"" group-title=""{4}"",{5}";

        public string TvgId { get; set; }
        public string TvgName { get; set; }
        public string TvgLogo { get; set; }
        public string GroupTitle { get; set; }
        public string Duration { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        private Uri _pathUri;
        public Uri PathUri
        {
            get
            {
                if (_pathUri == null)
                {
                    _pathUri = new Uri(Path);
                }
                return _pathUri;
            }
            internal set
            {
                _pathUri = value;
            }
        }

        public static Entry Parse(string rawString, string path)
		{
			var entry = new Entry();
			try
			{
                var match = Regex.Match(rawString, pattern);
				entry.Duration = match.Groups[1].ToString();
				entry.TvgId = match.Groups[2].ToString();
				entry.TvgName = match.Groups[3].ToString();
                entry.TvgLogo = match.Groups[4].ToString().Replace(" ", "%20");
				entry.GroupTitle = match.Groups[5].ToString();
				entry.Title = match.Groups[6].ToString();
				entry.Path = path;
				entry.PathUri = new Uri(path);

				return entry;
			}
#pragma warning disable CS0168 // Variable is declared but never used
            catch (UriFormatException e)
            {
				//ignore Uri issues on object create
				return entry;
			}
			catch (Exception e)
			{
				throw new Exception($"Error processing entry: {Environment.NewLine}{rawString}{Environment.NewLine}{path}{Environment.NewLine}{e.Message}");
			}
		}

        public override string ToString()
        {
            return string.Format(format, Duration, TvgId, TvgName, TvgLogo, GroupTitle, Title)
                         + Environment.NewLine
                         + Path;
        }
    }
}
