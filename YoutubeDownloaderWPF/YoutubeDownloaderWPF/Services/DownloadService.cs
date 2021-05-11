using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloaderWPF.Services
{
    public class DownloadService : IDownloadService
    {
        private string _downloadPath;
        private YoutubeClient _youtubeClient;

        public DownloadService()
        {
            _downloadPath = "C:/Youtube/Downloads";
            _youtubeClient = new YoutubeClient();
        }

        public async Task DownloadByYoutubeLink(string link)
        {
            var videoId = GetVideoId(link);
            var streamInfo = await GetStreamInfo(videoId);

            CreateDownloadDirectoryIfNotExists();

            DownloadVideo(streamInfo, link);
        }

        private async void DownloadVideo(IStreamInfo streamInfo, string link)
        {
            var title = await GetVideoTitle(link);

            await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, $"{_downloadPath}/{title}.{streamInfo.Container}");
        }

        private string GetVideoId(string link)
        {
            //Extract videoID from youtube link
            // https://www.youtube.com/watch?v=kdpGcyRteHg -> kdpGcyRteHg (video ID)
            var videoId = link.Replace("https://www.youtube.com/watch?v=", "");

            //Removes additional from ID
            //kdpGcyRteHg&ab_channel=LilWhind -> kdpGcyRteHg
            videoId = videoId.Substring(0, videoId.IndexOf("&"));

            return videoId;
        }

        private async Task<string> GetVideoTitle(string link)
        {
            var video = await _youtubeClient.Videos.GetAsync(link);

            return video.Title.Replace("/", " ");
        }

        private async Task<IStreamInfo> GetStreamInfo(string videoId)
        {
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoId);

            var streamInfo = streamManifest
                .GetMuxedStreams()
                .GetWithHighestBitrate();

            return streamInfo;
        }

        private async Task<Stream> GetStream(IStreamInfo streamInfo)
        {
            return await _youtubeClient.Videos.Streams.GetAsync(streamInfo);
        }

        private void CreateDownloadDirectoryIfNotExists()
        {
            if (!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
            }
        }
    }
}
