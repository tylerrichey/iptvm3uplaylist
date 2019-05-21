using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace IPTVM3UPlaylist.Tests
{
    public class GuideTests
    {
        private const string TestFile = "Data" + "\\guide.xml";

        [Theory]
        [InlineData(TestFile)]
        public void LoadGuide(string file)
        {
            var guide = Guide.LoadFromFileAsync(file).Result;
            Assert.Equal(2, guide.Channels.Count);
            Assert.Equal(4, guide.Programmes.Count);
        }

        [Theory]
        [InlineData(TestFile)]
        public void TestMetrics(string file)
        {
            MetricLog.GetMetrics += MetricLog_GetMetrics;
            var guide = Guide.LoadFromFileAsync(file).Result;
            MetricLog.GetMetrics -= MetricLog_GetMetrics;
        }

        private void MetricLog_GetMetrics(object sender, MetricLog e)
        {
            Assert.Equal("2", e.Metadata["Channels"]);
            Assert.Equal("4", e.Metadata["Programmes"]);
            Assert.NotNull(e.Name);
        }
    }
}
