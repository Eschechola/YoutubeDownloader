using System.Windows.Media;
using YoutubeDownloaderWPF.Enums;

namespace YoutubeDownloaderWPF.Factories
{
    public static class StatusColorFactory
    {
        public static SolidColorBrush GetColorFromStatus(ApplicationStatus status)
        {
            switch (status)
            {
                // gray
                case ApplicationStatus.QueueClean:
                    return new SolidColorBrush(Color.FromRgb(74, 74, 74));

                // yellow
                case ApplicationStatus.SearchInProgress:
                    return new SolidColorBrush(Color.FromRgb(194, 168, 23));
                
                // pink
                case ApplicationStatus.ResultsFound:
                    return new SolidColorBrush(Color.FromRgb(181, 38, 164));

                // blue
                case ApplicationStatus.DownloadInProgress:
                    return new SolidColorBrush(Color.FromRgb(45, 66, 99));

                // green
                case ApplicationStatus.Success:
                    return new SolidColorBrush(Color.FromRgb(126, 191, 77));

                //purple
                case ApplicationStatus.InvalidParameters:
                    return new SolidColorBrush(Color.FromRgb(146, 29, 209));

                // red
                case ApplicationStatus.Error:
                    return new SolidColorBrush(Color.FromRgb(184, 70, 79));

                // red
                default:
                    return new SolidColorBrush(Color.FromRgb(184, 70, 79));
            }
        }
    }
}
