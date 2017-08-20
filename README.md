# iptvm3uplaylist
A .NET Standard 2.0 library for parsing and creating IPTV-driven playlists and guides.

[![Build status](https://ci.appveyor.com/api/projects/status/4ap8lsjgrswnjpxr?svg=true)](https://ci.appveyor.com/project/tylerrichey/iptvm3uplaylist) [![NuGet](https://img.shields.io/nuget/v/IPTVM3UPlaylist.svg)](https://www.nuget.org/packages/IPTVM3UPlaylist/)

This has been built for personal use, but can be applicable as long as your playlist has the 'tvg' attributes and you have a standard IPTV XML guide.

Usage:
```c#
var playlist = await Playlist.LoadFromUrlAsync(Url);
var guide = Guide.LoadFromUrlAsync(GuideUrl);
```
Install:
```
Install-Package IPTVM3UPlaylist
```
or
```
dotnet add package IPTVM3UPlaylist
````
