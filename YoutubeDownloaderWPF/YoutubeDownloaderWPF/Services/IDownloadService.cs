using System.Collections.Generic;
using System.Threading.Tasks;
using YoutubeDownloaderWPF.Enums;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace YoutubeDownloaderWPF.Services
{
    public interface IDownloadService
    {
        Task<List<string>> GetVideosTitlesFromLinkAsync(string link, bool isPlaylist = false);
        Task<IEnumerable<PlaylistVideo>> GetPlayListVideosAsync(string link);
        Task<Video> GetVideoAsync(string link);
        Task<bool> DownloadVideo(string link, string directory, DownloadType type);
    }
}
