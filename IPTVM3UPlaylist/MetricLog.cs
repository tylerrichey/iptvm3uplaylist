using System;
using System.Collections.Generic;
using System.Text;

namespace IPTVM3UPlaylist
{
    public class MetricLog
    {
        public static event EventHandler<MetricLog> GetMetrics;

        public static void Push(MetricLog metric) => GetMetrics?.Invoke(null, metric);

        public Type Source { get; set; }
        public string Name { get; set; }
        public decimal ElapsedMs { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
