using System.IO;
using Xunit;
using Xunit.Extensions;

namespace IPTVM3UPlaylist.Tests
{
    public class PlaylistTests
    {
        private const string TestFile = "Data" + "\\test.m3u";

        [Theory]
        [InlineData(TestFile)]
        public void LoadPlaylist(string file)
        {
            var playlist = Playlist.LoadFromFileAsync(file).Result;
            Assert.Equal(2, playlist.Entries.Count);
        }

        [Theory]
        [InlineData(TestFile)]
        public void TestMetrics(string file)
        {
            MetricLog.GetMetrics += MetricLog_GetMetrics;
            var playlist = Playlist.LoadFromFileAsync(file).Result;
            MetricLog.GetMetrics -= MetricLog_GetMetrics;
        }

        private void MetricLog_GetMetrics(object sender, MetricLog e)
        {
            Assert.Equal("2", e.Metadata["Items"]);
        }
    }
}
