using System.Collections.Generic;
using System.Windows.Controls;

namespace YoutubeDownloaderWPF.ViewModels
{
    public class MainViewModel
    {
        public List<ListBoxItem> TitlesSearchResult { get; private set; }
        public int VideosInQueueCount { get; private set; }
        public int VideosDownloadedCount { get; private set; }
        public int VideosErrorsCount { get; private set; }

        public MainViewModel()
        {
            QueueReset();
            TitlesSearchResult = new List<ListBoxItem>();
        }

        public void SetTitlesSearchResult(List<ListBoxItem> result)
            => TitlesSearchResult = result;

        public void SetVideosInQueueCount(int count)
            => VideosInQueueCount = count;

        public void IncreaseVideosInQueueCount(int increaseValue = 1)
            => VideosInQueueCount += increaseValue;

        public void DecreaseVideosInQueueCount(int decreaseValue = 1)
        {
            var value = VideosInQueueCount - decreaseValue;

            if (value < 0)
                VideosInQueueCount = 0;
            else
                VideosInQueueCount = value;
        }

        public void IncreaseVideosDownloadedCount(int increaseValue = 1)
            => VideosDownloadedCount += increaseValue;

        public void DecreaseVideosDownloadedCount(int decreaseValue = 1)
        {
            var value = VideosDownloadedCount - decreaseValue;

            if (value < 0)
                VideosDownloadedCount = 0;
            else
                VideosDownloadedCount = value;
        }

        public void IncreaseVideosErrorCount(int increaseValue = 1)
            => VideosErrorsCount += increaseValue;

        public void DecreaseVideosErrorsCount(int decreaseValue = 1)
        {
            var value = VideosErrorsCount - decreaseValue;

            if (value < 0)
                VideosErrorsCount = 0;
            else
                VideosErrorsCount = value;
        }

        public void QueueReset()
        {
            VideosInQueueCount = 0;
            VideosDownloadedCount = 0;
            VideosErrorsCount = 0;
        }
    }
}
