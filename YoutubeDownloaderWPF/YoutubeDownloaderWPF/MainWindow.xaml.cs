using System;
using System.Windows;
using System.Windows.Media;
using YoutubeDownloaderWPF.Enums;
using YoutubeDownloaderWPF.Services;

namespace YoutubeDownloaderWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDownloadService _downloadService;

        public MainWindow()
        {
            InitializeComponent();
            _downloadService = new DownloadService();
            SetDownloadStatus(DownloadStatus.QueueClean);
        }

        private async void BtnDownloadClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtLinkInput.Text))
                    MessageBox.Show("Por favor digite o link corretamente!", "Alerta!");

                SetDownloadStatus(DownloadStatus.InProgress);
                
                string link = TxtLinkInput.Text;
                await _downloadService.DownloadByYoutubeLink(link);

                SetDownloadStatus(DownloadStatus.Success);

                MessageBox.Show("Download efetuado!");
            }
            catch (Exception)
            {
                SetDownloadStatus(DownloadStatus.Error);
                MessageBox.Show("Ocorreu algum erro, por favor tente novamente", "Erro!");
            }
        }

        private void SetDownloadStatus(DownloadStatus status)
        {
            switch (status)
            {
                case DownloadStatus.QueueClean:
                    LblStatus.Content = "Nenhum vídeo na fila";
                    LblStatus.Foreground = Brushes.Gray;
                    break;

                case DownloadStatus.InProgress:
                    LblStatus.Content = "Baixando vídeo...";
                    LblStatus.Foreground = Brushes.Blue;
                    break;

                case DownloadStatus.Success:
                    LblStatus.Content = "Vídeo baixado!";
                    LblStatus.Foreground = Brushes.Green;
                    break;

                case DownloadStatus.Error:
                    LblStatus.Content = "Ocorreu um erro, por favor tente novamente.";
                    LblStatus.Foreground = Brushes.Red;
                    break;
            }
        }
    }
}
