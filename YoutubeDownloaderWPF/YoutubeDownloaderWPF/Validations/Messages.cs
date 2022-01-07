namespace YoutubeDownloaderWPF.Validations
{
    public static class Messages
    {
        public const string InternalServerError = "Ocorreu algum erro, por favor tente novamente";

        public const string InvalidLink = "O Link inserido não é válido, por favor tente novamente!";
        public const string InvalidYoutubeLink = "O Link informado não é do youtube, por favor tente novamente!";
        public const string DownloadTypeIsPlaylistAndLinkNot = "O tipo de download informado é de playlist, porém o link é de um vídeo, tente alterar o tipo de download.";
        public const string DownloadTypeIsVideoAndLinkNot = "O tipo de download informado é de vídeo, porém o link é de uma playlist, tente alterar o tipo de download.";

        public const string InvalidDirectory = "O Diretório inserido não é válido, por favor tente novamente!";
    }
}
