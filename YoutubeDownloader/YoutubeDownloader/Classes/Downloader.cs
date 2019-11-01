using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace YoutubeDownloader.Classes
{
    public class Downloader
    {
        private List<string> _listaDeVideosPlaylist = new List<string>();
        private string _linkDaPlaylist { get; set; }
        private string _diretorioSalvar { get; set; }
        private List<string> _listaDeNomesVideos = new List<string>();


        public Downloader(string linkDaPlaylist, string diretorioSalvar)
        {
            _linkDaPlaylist = GetIdFromLink(linkDaPlaylist);
            _diretorioSalvar = diretorioSalvar;
        }


        public async void Execute()
        {
            await GetAllLinks();
            DownloadAllVideos();
        }

        private string GetIdFromLink(string link)
        {
            return YoutubeClient.ParsePlaylistId(link);
        }

        public async Task<List<string>> ReturnAllLinks()
        {
            await GetAllLinks();
            return _listaDeVideosPlaylist;
        }

        public string FormatTitle(string titulo)
        {
            string padraoString = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s]";
            Regex regex = new Regex(padraoString);
            string resultado = regex.Replace(titulo, "");

            return resultado;
        }

        private async Task<bool> GetAllLinks()
        {
            try
            {
                var cliente = new YoutubeClient();
                var playlistCompleta = await cliente.GetPlaylistAsync(_linkDaPlaylist);

                foreach (var video in playlistCompleta.Videos)
                {
                    _listaDeVideosPlaylist.Add(video.Id);
                    _listaDeNomesVideos.Add(FormatTitle(video.Title));
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private async void DownloadAllVideos()
        {

            var cliente = new YoutubeClient();

            for (int i = 0; i < _listaDeVideosPlaylist.Count; i++)
            {
                try
                {
                    var streamInfoSet = await cliente.GetVideoMediaStreamInfosAsync(_listaDeVideosPlaylist[i]);
                    var streamInfo = streamInfoSet.Audio.WithHighestBitrate();
                    await cliente.DownloadMediaStreamAsync(streamInfo, @_diretorioSalvar + _listaDeNomesVideos[i] + ".mp3");
                    Console.WriteLine(_listaDeNomesVideos[i] + " Baixado com sucesso");
                }
                catch (Exception)
                {
                    Console.WriteLine("Ocorreu algum erro....");
                    continue;
                }
            }
        }
    }
}
