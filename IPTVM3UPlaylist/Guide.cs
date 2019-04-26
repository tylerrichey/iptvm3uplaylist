using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace IPTVM3UPlaylist
{
    [XmlRoot("tv")]
    public class Guide
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Guide));

        public static async Task<Guide> LoadFromUrlAsync(string url)
        {
            var stream = await httpClient.GetStreamAsync(url).ConfigureAwait(true);
            return await LoadFromStreamAsync(stream);
        }

        public static async Task<Guide> LoadFromFileAsync(string file)
        {
			return await LoadFromStreamAsync(
				new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous));
        }

        public static async Task<Guide> LoadFromStreamAsync(Stream stream)
        {
            try
            {
                var metric = Stopwatch.StartNew();
                var guide = await Task.Run(() => serializer.Deserialize(stream) as Guide);
                metric.Stop();
                Console.WriteLine($"Guide parsed in {metric.ElapsedMilliseconds}ms - {guide.Channels.Count} channels - {guide.Programmes.Count} programmes.");
                return guide;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Guide FilterByPlaylist(Playlist playlist)
        {
            var favs = playlist.Entries.Select(p => p.TvgId);
            return new Guide
            {
                Generatorinfoname = Generatorinfoname,
                Generatorinfourl = Generatorinfourl,
                Programmes = Programmes.Where(p => favs.Contains(p.Channel)).ToList(),
                Channels = Channels.Where(c => favs.Contains(c.Id)).ToList()
            };
        }

        public async Task<Stream> SaveToStreamAsync()
        {
            var stream = new MemoryStream();
			try
			{
                await Task.Run(() => serializer.Serialize(stream, this));
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
			}
			catch (Exception e)
			{
				throw e;
			}
        }

        public List<Programme> Search(string search)
        {
            return Programmes.Where(p => p.Title != null)
                             .Where(p => p.Title.Text.Contains(search))
                             .OrderBy(p => p.StartDateTime)
                             .ToList();
        }

		[XmlElement(ElementName = "channel")]
		public List<Channel> Channels { get; set; }
		[XmlElement(ElementName = "programme")]
		public List<Programme> Programmes { get; set; }
		[XmlAttribute(AttributeName = "generator-info-name")]
		public string Generatorinfoname { get; set; }
		[XmlAttribute(AttributeName = "generator-info-url")]
		public string Generatorinfourl { get; set; }

        [XmlRoot(ElementName = "display-name")]
        public class Displayname
        {
            [XmlAttribute(AttributeName = "lang")]
            public string Lang { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "icon")]
        public class Icon
        {
            private string _src;

            [XmlAttribute(AttributeName = "src")]
            public string Src 
            { 
                get
                {
                    return _src;
                }
                set
                {
                    _src = value.Replace(" ", "%20");
                }
            }
        }

        [XmlRoot(ElementName = "channel")]
        public class Channel
        {
            [XmlElement(ElementName = "display-name")]
            public Displayname Displayname { get; set; }
            [XmlElement(ElementName = "icon")]
            public Icon Icon { get; set; }
            [XmlElement(ElementName = "url")]
            public string Url { get; set; }
            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }
        }

        [XmlRoot(ElementName = "title")]
        public class Title
        {
            [XmlAttribute(AttributeName = "lang")]
            public string Lang { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "sub-title")]
        public class Subtitle
        {
            [XmlAttribute(AttributeName = "lang")]
            public string Lang { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "desc")]
        public class Desc
        {
            [XmlAttribute(AttributeName = "lang")]
            public string Lang { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "credits")]
        public class Credits
        {
            [XmlElement(ElementName = "actor")]
            public List<string> Actor { get; set; }
        }

        [XmlRoot(ElementName = "category")]
        public class Category
        {
            [XmlAttribute(AttributeName = "lang")]
            public string Lang { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "programme")]
        public class Programme
        {
            [XmlElement(ElementName = "title")]
            public Title Title { get; set; }
            [XmlElement(ElementName = "sub-title")]
            public Subtitle Subtitle { get; set; }
            [XmlElement(ElementName = "desc")]
            public Desc Desc { get; set; }
            [XmlElement(ElementName = "credits")]
            public Credits Credits { get; set; }
            [XmlElement(ElementName = "category")]
            public List<Category> Category { get; set; }
            [XmlElement(ElementName = "icon")]
            public Icon Icon { get; set; }
            [XmlElement(ElementName = "episode-num")]
            public string Episodenum { get; set; }
            [XmlAttribute(AttributeName = "start")]
            public string Start { get; set; }
            [XmlAttribute(AttributeName = "stop")]
            public string Stop { get; set; }
            [XmlAttribute(AttributeName = "channel")]
            public string Channel { get; set; }

            private DateTime _startDateTime;
            public DateTime StartDateTime 
            {
                get
                {
                    if (_startDateTime == DateTime.MinValue)
                    {
                        DateTime.TryParseExact(Start, "yyyyMMddHHmmss zzz", CultureInfo.CurrentCulture, DateTimeStyles.None, out _startDateTime);
                    }
                    return _startDateTime;
                }
            }

            private DateTime _stopDateTime;
			public DateTime StopDateTime
			{
				get
				{
                    if (_stopDateTime == DateTime.MinValue)
                    {
                        DateTime.TryParseExact(Stop, "yyyyMMddHHmmss zzz", CultureInfo.CurrentCulture, DateTimeStyles.None, out _stopDateTime);
                    }
                    return _stopDateTime;
				}
			}
        }
    }
}
