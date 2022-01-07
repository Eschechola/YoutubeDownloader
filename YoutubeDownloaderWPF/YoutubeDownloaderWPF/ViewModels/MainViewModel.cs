using System.Collections.Generic;
using System.Windows.Controls;

namespace YoutubeDownloaderWPF.ViewModels
{
    public class MainViewModel
    {
        public List<ListBoxItem> TitlesSearchResult { get; set; }

        public MainViewModel()
        {
            TitlesSearchResult = new List<ListBoxItem>();
        }
    }
}
