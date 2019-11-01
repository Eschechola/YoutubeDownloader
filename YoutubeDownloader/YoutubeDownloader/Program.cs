using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Classes;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace YoutubeDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Digite o link da playlist\n> ");
            string linkPlaylist = Console.ReadLine();

            Console.Write("\n");
            Console.WriteLine("Digite o diretório onde serão salvos os vídeos");
            Console.WriteLine("Exemplo: C:\\Users\\Usuario\\Downloads");
            Console.Write("> ");
            string diretorioDeDownload = Console.ReadLine();

            Console.Write("Executando...");
            new Downloader(linkPlaylist, diretorioDeDownload).Execute();
            Console.ReadKey();
        }
    }
}
