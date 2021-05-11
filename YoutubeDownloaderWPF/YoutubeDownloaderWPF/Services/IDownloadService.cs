using System.Threading.Tasks;

namespace YoutubeDownloaderWPF.Services
{
    public interface IDownloadService
    {
        Task DownloadByYoutubeLink(string link);
    }
}
