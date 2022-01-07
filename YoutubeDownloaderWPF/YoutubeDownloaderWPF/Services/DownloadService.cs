using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeDownloaderWPF.Enums;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloaderWPF.Services
{
    public class DownloadService : IDownloadService
    {
        private YoutubeClient _youtubeClient;

        public DownloadService()
        {
            _youtubeClient = new YoutubeClient();
        }

        public async Task<List<string>> GetVideosTitlesFromLinkAsync(string link, bool isPlaylist = false)
        {
            if (isPlaylist)
                return GetVideoTitlesListFromPlayList(await GetPlayListVideosAsync(link));

            var video = await GetVideoAsync(link);
            return new List<string> { video.Title };
        }

        public async Task<IEnumerable<PlaylistVideo>> GetPlayListVideosAsync(string link)
        {
            var playlistId = GetYoutubeUrlId(link, isPlaylist: true);
            var playlistVideos = await _youtubeClient.Playlists.GetVideosAsync(playlistId);

            return playlistVideos;
        }

        public async Task<Video> GetVideoAsync(string link)
            => await _youtubeClient.Videos.GetAsync(link);

        public async Task<bool> DownloadVideo(string link, string directory, DownloadType type)
        {
            if (type == DownloadType.Video || type == DownloadType.VideoPlaylist)
                return await DownloadMP4VideoFromYoutubeAsync(link, directory);
            else
                return await DownloadMP3AudioFromYoutubeAsync(link, directory);
        }

        private async Task<bool> DownloadMP4VideoFromYoutubeAsync(string link, string directory)
        {
            var videoInfo = await GetVideoAsync(link);
            var videoStreamManifest = await GetVideoStreamManifestAsync(link);
            var streamInfo = videoStreamManifest
                .GetMuxedStreams()
                .Where(s => s.Container == Container.Mp4)
                .GetWithHighestVideoQuality();

            var fileName = CreateFileNameFormatted(videoInfo.Title, ".mp4");
            var downloadPath = GetDownloadPath(directory, isAudio: false) + "\\" + fileName;

            DeleteFileIfAlreadyExists(downloadPath);

            await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, downloadPath);

            return true;
        }

        private async Task<bool> DownloadMP3AudioFromYoutubeAsync(string link, string directory)
        {
            var audioInfo = await GetVideoAsync(link);
            var audioStreamManifest = await GetVideoStreamManifestAsync(link);
            var streamInfo = audioStreamManifest
                .GetMuxedStreams()
                .GetWithHighestBitrate();

            var fileName = CreateFileNameFormatted(audioInfo.Title, ".mp3");
            var downloadPath = Path.Combine(GetDownloadPath(directory, isAudio: true), fileName);

            DeleteFileIfAlreadyExists(downloadPath);

            await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, downloadPath);

            return true;
        }

        private void DeleteFileIfAlreadyExists(string fileDirectory)
        {
            if (File.Exists(fileDirectory))
                File.Delete(fileDirectory);
        }

        private string CreateFileNameFormatted(string fileName, string extension)
        {
            var fileNameFormatted = fileName.Replace("/", " - ");
            fileNameFormatted = fileNameFormatted + extension;

            return fileNameFormatted;
        }

        private string GetDownloadPath(string directory, bool isAudio = false)
        {
            var nowToString = DateTime.Now.ToString("dd-MM-yyyy");
            string pathExtension = isAudio ? "\\Audios\\" : "\\Videos\\";

            string directoryPath = directory + pathExtension + nowToString;

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return directoryPath;
        }


        private async Task<StreamManifest> GetVideoStreamManifestAsync(string link)
        {
            var videoId = GetYoutubeUrlId(link);
            return await _youtubeClient.Videos.Streams.GetManifestAsync(videoId);
        }

        private List<string> GetVideoTitlesListFromPlayList(IEnumerable<PlaylistVideo> videos)
        {
            var titleList = new List<string>();

            foreach (var video in videos)
                titleList.Add(video.Title);

            return titleList;
        }

        private string GetYoutubeUrlId(string link, bool isPlaylist = false)
        {
            //remove youtube url
            string videoId = "";

            if (isPlaylist)
                videoId = link.Replace("https://www.youtube.com/playlist?list=", "");
            else
                videoId = link.Replace("https://www.youtube.com/watch?v=", "");

            //remove additional parammeters from url
            if(videoId.Contains("&"))
                videoId = videoId.Substring(0, videoId.IndexOf("&"));

            return videoId;
        }
    }
}
