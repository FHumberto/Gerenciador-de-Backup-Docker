using System.IO.Compression;

namespace SSV.Console.Utilities;

public static class FileHandler
{
    public static string CriarDiretorio (string diretorio)
    {
        string caminho = $"{diretorio}\\";

        string diretorioTemporario = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, caminho);

        if (!Directory.Exists(diretorioTemporario))
        {
            Directory.CreateDirectory(diretorioTemporario);
        }

        return diretorioTemporario;
    }

    public static void ProcessarVolumes (string diretorioCloud, string diretorioTemporario, Dictionary<string, string> diretorioInfo, int quantidade)
    {
        if (diretorioInfo.TryGetValue("Alias", out var volumeAlias) && diretorioInfo.TryGetValue("Path", out var caminhoVolume))
        {
            if (Directory.Exists(caminhoVolume))
            {
                string dataFormatada = DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
                string nomeArquivoZip = $"{diretorioTemporario}{volumeAlias}-{dataFormatada}.zip";

                ZipFile.CreateFromDirectory(caminhoVolume, nomeArquivoZip);

                if (Directory.Exists(diretorioCloud))
                {
                    var arquivosParaRemover = Directory.GetFiles(diretorioCloud, $"{volumeAlias}*.zip")
                        .Where(file => file.Contains(volumeAlias))
                        .OrderByDescending(File.GetLastWriteTime)
                        .Skip(quantidade)
                        .ToList();

                    arquivosParaRemover.ForEach(File.Delete);
                }
                else
                {
                    throw new DirectoryNotFoundException($"O diretório {diretorioCloud} não existe.");
                }

                File.Move(nomeArquivoZip, Path.Combine(diretorioCloud, Path.GetFileName(nomeArquivoZip)));
            }
            else
            {
                Logger.LogMessage($"Diretório não existe -> {volumeAlias}: {caminhoVolume}");
            }
        }
    }
}