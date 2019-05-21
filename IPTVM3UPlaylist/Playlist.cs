using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace IPTVM3UPlaylist
{
    public class Playlist
    {
        private const string startOfFile = "#EXTM3U";
        private static HttpClient httpClient = new HttpClient();
        public List<Entry> Entries { get; set; }
        public List<string> ChannelList => Entries.Select(p => p.Title).OrderBy(p => p).ToList();
        public string Name { get; set; }

        public Playlist(List<Entry> entries)
        {
            Entries = entries;
        }

        public static async Task<Playlist> LoadFromFileAsync(string file)
        {
            return await LoadFromStreamAsync(
                new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous),
                Path.GetFileName(file));
        }

        public static async Task<Playlist> LoadFromUrlAsync(string url)
        {
            try
            {
                var stream = await httpClient.GetStreamAsync(url).ConfigureAwait(false);
                return await LoadFromStreamAsync(stream, new Uri(url).Host);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task<Playlist> LoadFromStreamAsync(Stream stream, string name = "")
        {
            var metric = new Stopwatch();
            metric.Start();
            var entries = new ConcurrentBag<Entry>();
            using (StreamReader sr = new StreamReader(stream))
            {
                var header = await sr.ReadLineAsync().ConfigureAwait(false);
                if (!header.StartsWith(startOfFile, StringComparison.CurrentCulture))
                {
                    throw new Exception("Not a valid M3U playlist. No header.");
                }

                sr.GetStringArrayEnumerable(2)
                  .AsParallel()
                  .ForAll(entry => entries.Add(Entry.Parse(entry[0], entry[1])));
            }
            var playlist = new Playlist(entries.ToList());
            metric.Stop();
            playlist.Name = name;
            MetricLog.Push(new MetricLog
            {
                Source = typeof(Playlist),
                Name = name,
                ElapsedMs = metric.ElapsedMilliseconds,
                Metadata = new Dictionary<string, string>
                {
                    { "Items", playlist.Entries.Count.ToString() }
                }
            });
            return playlist;
        }

        public async Task<MemoryStream> SaveToStreamAsync()
        {
            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            await sw.WriteLineAsync(startOfFile).ConfigureAwait(false);
            foreach (var e in Entries)
            {
                await sw.WriteLineAsync(e.ToString()).ConfigureAwait(false);
            }
            await sw.FlushAsync().ConfigureAwait(false);
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
        }

        public async Task SaveToFileAsync(string file, bool overwrite = false)
        {
            using (var fs = new FileStream(file, overwrite ? FileMode.Create : FileMode.CreateNew, 
                                           FileAccess.Write, FileShare.Write, 4096, FileOptions.Asynchronous))
            {
                var stream = await SaveToStreamAsync().ConfigureAwait(false);
                await stream.CopyToAsync(fs).ConfigureAwait(false);
                await fs.FlushAsync();
            }
        }

        public async Task<string> ToStringAsync()
        {
            using (var sr = new StreamReader(await SaveToStreamAsync().ConfigureAwait(false)))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
