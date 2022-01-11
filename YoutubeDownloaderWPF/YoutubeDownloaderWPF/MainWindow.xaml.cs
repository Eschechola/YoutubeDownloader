using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YoutubeDownloaderWPF.Enums;
using YoutubeDownloaderWPF.Factories;
using YoutubeDownloaderWPF.Services;
using YoutubeDownloaderWPF.Validations;
using YoutubeDownloaderWPF.ViewModels;

namespace YoutubeDownloaderWPF
{
    public partial class MainWindow : Window
    {
        private readonly IDownloadService _downloadService;
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _downloadService = new DownloadService();
            _mainViewModel = new MainViewModel();

            DataContext = _mainViewModel;
        }

        #region Events

        private async void ButtonDownloadClickEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                SetDownloadInProgressStatus("Iniciando Download...");
                DisableDownloadButton();

                if (!ValidateAllParameters())
                {
                    SetApplicationStatus(ApplicationStatus.InvalidParameters);
                    return;
                }

                var link = GetLinkToDownload();
                var directory = GetDirectoryToDownload();
                var downloadType = GetDownloadType();

                if (DownloadTypeIsPlaylist())
                {
                    var playlistVideos = await _downloadService.GetPlayListVideosAsync(link);

                    foreach(var video in playlistVideos)
                    {
                        try
                        {
                            SetDownloadInProgressStatus(video.Title);
                            await _downloadService.DownloadVideo(video.Url, directory, downloadType);
                            
                            SetDonwloadStatusInListBox(ApplicationStatus.Success, video.Title);
                            IncreaseVideosDownloaded();
                        }
                        catch (Exception)
                        {
                            IncreaseVideosError();
                            SetDonwloadStatusInListBox(ApplicationStatus.Error, video.Title);
                            continue;
                        }
                    }
                }
                else
                {
                    var video = await _downloadService.GetVideoAsync(link);
                    try
                    {
                        SetDownloadInProgressStatus(video.Title);
                        await _downloadService.DownloadVideo(link, directory, downloadType);
                        SetDonwloadStatusInListBox(ApplicationStatus.Success, video.Title);
                    }
                    catch (Exception)
                    {
                        IncreaseVideosError();
                        SetDonwloadStatusInListBox(ApplicationStatus.Error, video.Title);
                    }
                }

                SetApplicationStatus(ApplicationStatus.Success);
            }
            catch (Exception)
            {
                SetApplicationStatus(ApplicationStatus.Error);
                MessageBox.Show(Messages.InternalServerError, "Erro!");
            }

            EnableDownloadButton();
        }

        private async void ButtonSearchClickEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateLinkToDownload())
                {
                    SetApplicationStatus(ApplicationStatus.InvalidParameters);
                    return;
                }

                SetApplicationStatus(ApplicationStatus.SearchInProgress);

                var link = GetLinkToDownload();
                var isPlaylist = DownloadTypeIsPlaylist();
                var videosTitles = await _downloadService.GetVideosTitlesFromLinkAsync(link, isPlaylist);

                ClearOldSearchResults();
                foreach (var video in videosTitles)
                    _mainViewModel.TitlesSearchResult.Add(new ListBoxItem { Content = video });

                ListBoxSearchResult.ItemsSource = _mainViewModel.TitlesSearchResult;

                UpdateVideosInQueue(_mainViewModel.TitlesSearchResult.Count);
                AddSearchResult();
                EnableDownloadButton();
                SetApplicationStatus(ApplicationStatus.ResultsFound);
            }
            catch (Exception)
            {
                SetApplicationStatus(ApplicationStatus.Error);
                MessageBox.Show(Messages.InternalServerError, "Erro!");
            }
        }

        #endregion

        #region Methods

        private bool ValidateAllParameters()
        {
            if (!ValidateLinkToDownload())
                return false;

            if (!ValidateDirectoryToDownload())
                return false;

            return true;
        }

        private bool ValidateDirectoryToDownload()
        {
            var directory = GetDirectoryToDownload();

            switch (directory)
            {
                case string directoryToValidate when string.IsNullOrEmpty(directoryToValidate):
                    MessageBox.Show(Messages.InvalidDirectory, "Alerta!");
                    return false;
            }

            return true;
        }

        private bool ValidateLinkToDownload()
        {
            var link = GetLinkToDownload();

            switch (link)
            {
                case string linkToValidate when string.IsNullOrEmpty(linkToValidate):
                    MessageBox.Show(Messages.InvalidLink, "Alerta!");
                    return false;

                case string linkToValidate when !Uri.IsWellFormedUriString(linkToValidate, UriKind.Absolute):
                    MessageBox.Show(Messages.InvalidLink, "Alerta!");
                    return false;

                case string linkToValidate when !linkToValidate.ToLower().Contains("youtube.com"):
                    MessageBox.Show(Messages.InvalidYoutubeLink, "Alerta!");
                    return false;

                case string linkToValidate when DownloadTypeIsPlaylist() && linkToValidate.ToLower().Contains("watch"):
                    MessageBox.Show(Messages.DownloadTypeIsPlaylistAndLinkNot, "Alerta!");
                    return false;

                case string linkToValidate when (!DownloadTypeIsPlaylist()) && linkToValidate.ToLower().Contains("playlist"):
                    MessageBox.Show(Messages.DownloadTypeIsVideoAndLinkNot, "Alerta!");
                    return false;
            }

            return true;
        }

        private void AddSearchResult()
        {
            if (_mainViewModel.TitlesSearchResult.Any())
            {
                LblSearchMessage.Visibility = Visibility.Collapsed;

                ListBoxSearchResult.Visibility = Visibility.Visible;
                ListBoxSearchResult.ItemsSource = _mainViewModel.TitlesSearchResult;

                BtnSearch.SetValue(Grid.ColumnSpanProperty, 1);
                BtnDownload.Visibility = Visibility.Visible;
            }
            else
            {
                LblSearchMessage.Content = "Nenhum resultado encontrado.";
                ListBoxSearchResult.Visibility = Visibility.Collapsed;
                BtnDownload.Visibility = Visibility.Collapsed;
                BtnSearch.SetValue(Grid.ColumnSpanProperty, 0);
            }
        }

        private bool DownloadTypeIsPlaylist()
        {
            var downloadType = GetDownloadType();

            return downloadType == DownloadType.VideoPlaylist ||
                   downloadType == DownloadType.MusicPlaylist;
        }

        private string GetLinkToDownload()
            => TxtLink.Text;

        private string GetDirectoryToDownload()
            => TxtDirectory.Text;

        private DownloadType GetDownloadType()
        {
            var downloadTypeText = ComboDownloadType.SelectedValue.ToString();
            downloadTypeText = downloadTypeText.Replace("System.Windows.Controls.ComboBoxItem: ", "");

            return GetDownloadTypeFromText(downloadTypeText);
        }

        private DownloadType GetDownloadTypeFromText(string text)
        {
            switch (text)
            {
                case "Vídeo (.mp4)":
                    return DownloadType.Video;

                case "Música (.mp3)":
                    return DownloadType.Music;

                case "Playlist de Vídeo (.mp4)":
                    return DownloadType.VideoPlaylist;

                case "Playlist de Música (.mp3)":
                    return DownloadType.MusicPlaylist;

                default:
                    return DownloadType.Video;
            }
        } 

        private void SetDownloadInProgressStatus(string fileTitle)
        {
            LblStatus.Foreground = StatusColorFactory.GetColorFromStatus(ApplicationStatus.DownloadInProgress);
            LblStatus.Content = "Baixando: " + fileTitle;
        }

        private void SetApplicationStatus(ApplicationStatus status)
        {
            LblStatus.Foreground = StatusColorFactory.GetColorFromStatus(status);
            LblStatus.Content = StatusMessageFactory.GetMessageFromStatus(status);

            switch (status)
            {
                case ApplicationStatus.ResultsFound:
                    MessageBox.Show("Vídeo(s) encontrado(s)!");
                    break;

                case ApplicationStatus.DownloadInProgress:
                    MessageBox.Show("Iniciando download...");
                    break;

                case ApplicationStatus.Success:
                    MessageBox.Show("Download realizado com sucesso, salvo em: " + TxtDirectory.Text);
                    break;
            }
        }

        private void SetDonwloadStatusInListBox(ApplicationStatus status, string videoTitle)
        {
            SolidColorBrush colorBrushLine;

            switch (status)
            {
                case ApplicationStatus.Success:
                    colorBrushLine = Brushes.Green;
                break;

                case ApplicationStatus.Error:
                    colorBrushLine= Brushes.Red;
                break;

                default:
                    colorBrushLine = Brushes.Red;
                break;
            }

            for(int i = 0; i < _mainViewModel.TitlesSearchResult.Count; i++)
            {
                var lineTitle = _mainViewModel.TitlesSearchResult[i]
                    .Content
                    .ToString()
                    .ToLower();

                if (lineTitle == videoTitle.ToLower())
                {
                    _mainViewModel.TitlesSearchResult[i].Foreground = colorBrushLine;
                    break;
                }
            }

            ListBoxSearchResult.ItemsSource =  _mainViewModel.TitlesSearchResult;
        }

        private void EnableDownloadButton()
            => BtnDownload.IsEnabled = true;

        private void DisableDownloadButton()
            => BtnDownload.IsEnabled = false;

        private void ClearOldSearchResults()
        {
            ResetQueue();
            _mainViewModel.TitlesSearchResult.Clear();
            ListBoxSearchResult.ItemsSource = new List<ListBoxItem>();
        }

        private void UpdateVideosInQueue(int count)
        {
            _mainViewModel.SetVideosInQueueCount(count);
            LblVideosInQueueCount.Content = _mainViewModel.VideosInQueueCount;
        }

        private void IncreaseVideosDownloaded()
        {
            DecreaseVideosInQueue();
            _mainViewModel.IncreaseVideosDownloadedCount();
            LblVideosDownloadedCount.Content = _mainViewModel.VideosDownloadedCount;
        }

        private void IncreaseVideosError()
        {
            DecreaseVideosInQueue();
            _mainViewModel.IncreaseVideosErrorCount();
            LblVideosErrorCount.Content = _mainViewModel.VideosErrorsCount;
        }

        private void DecreaseVideosInQueue()
        {
            _mainViewModel.DecreaseVideosInQueueCount();
            LblVideosInQueueCount.Content = _mainViewModel.VideosInQueueCount;
        }

        private void ResetQueue()
            => _mainViewModel.QueueReset();

        #endregion
    }
}
