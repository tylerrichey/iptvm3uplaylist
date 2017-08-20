using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace IPTVM3UPlaylist
{
    public static class ExtensionMethods
    {
        public static IEnumerable<string[]> GetStringArrayEnumerable(this StreamReader sr, int numberOfLinesPerArray)
        {
            while (!sr.EndOfStream)
            {
                yield return Enumerable.Range(0, numberOfLinesPerArray)
                                       .Select(i => sr.ReadLine())
                                       .ToArray();
            }
            yield break;
        }
    }
}
