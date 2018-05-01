using Xunit;

namespace IPTVM3UPlaylist.Tests
{
    public class EntryParseTests
    {
        [Theory]
        [InlineData("#EXTINF:-1 ,ABC News",
                    "localFile.ts")]
        [InlineData("#EXTINF:-1 tvg-id=\"ABCNews.au\",ABC News",
                    "http://iptv.example.org/au/Sydney/12345.ts")]
        public void ShouldParsePath(string extInf, string path)
        {
            var entry = Entry.Parse(extInf, path);
            Assert.NotNull(entry);
            Assert.Equal(path, entry.Path);
        }

        [Theory]
        [InlineData("#EXTINF:-1 ,ABC News",
            "http://iptv.example.org/au/Sydney/12345.ts", "")]
        [InlineData("#EXTINF:-1 tvg-id=\"ABCNews.au\" tvg-name=\"ABC NEWS\" tvg-logo=\"https://iptv-logo.example.org/logo.png\" group-title=\"Australia\",ABC News",
            "http://iptv.example.org/au/Sydney/12345.ts", "ABCNews.au")]
        public void ShouldParseTvgId(string extInf, string path, string tvgId)
        {
            var entry = Entry.Parse(extInf, path);
            Assert.NotNull(entry);
            Assert.Equal(tvgId, entry.TvgId);
        }

        [Fact]
        public void ShouldParse()
        {
            var entry = Entry.Parse("#EXTINF:-1 tvg-id=\"ABCNews.au\" tvg-name=\"ABC NEWS\" tvg-logo=\"https://iptv-logo.example.org/logo.png\" group-title=\"Australia\",ABC News",
                "http://iptv.example.org/au/Sydney/12345.ts");
            Assert.NotNull(entry);
            Assert.Equal("ABCNews.au", entry.TvgId);
            Assert.Equal("ABC News", entry.Title);
            Assert.Equal("ABC NEWS", entry.TvgName);
            Assert.Equal("Australia", entry.GroupTitle);
            Assert.Equal("https://iptv-logo.example.org/logo.png", entry.TvgLogo);
            Assert.Equal("http://iptv.example.org/au/Sydney/12345.ts", entry.Path);
        }
    }
}
