using YoutubeDownloaderWPF.Enums;

namespace YoutubeDownloaderWPF.Factories
{
    public static class StatusMessageFactory
    {
        public static string GetMessageFromStatus(ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.QueueClean:
                    return "NENHUM VÍDEO NA FILA.";

                case ApplicationStatus.SearchInProgress:
                    return "BUSCANDO VÍDEO(S)...";

                case ApplicationStatus.ResultsFound:
                    return "VÍDEO(S) ENCONTRADO(S)!";

                case ApplicationStatus.DownloadInProgress:
                    return "BAIXANDO VÍDEO(S)...";

                case ApplicationStatus.Success:
                    return "VÍDEO(S) BAIXADO(S)!";

                case ApplicationStatus.InvalidParameters:
                    return "CAMPO(S) INVÁLIDO(S) POR FAVOR CORRIJA-OS!";

                case ApplicationStatus.Error:
                    return "ERRO AO TENTAR BAIXAR, POR FAVOR TENTE NOVAMENTE.";

                default:
                    return "ACONTECEU UM ERRO, POR FAVOR TENTE NOVAMENTE OU CONSULTE OS LOGS.";
            }
        }
    }
}
