using System.IO.Compression;

using Microsoft.Extensions.Configuration;
using SSV.Console;

IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

Settings? settings = config.GetRequiredSection("Settings").Get<Settings>();

try
{
    string diretorioTemporario = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");

    if (settings?.DiretoriosDosVolumes?.Count > 0)
    {
        foreach (Dictionary<string, string> diretorioInfo in settings.DiretoriosDosVolumes)
        {
            if (diretorioInfo.TryGetValue("Alias", out var volumeAlias) && diretorioInfo.TryGetValue("Path", out var volumePath))
            {
                if (Directory.Exists(volumePath))
                {
                    Directory.CreateDirectory(diretorioTemporario);

                    //! Adiciona a data formatada ao nome do arquivo ZIP
                    string dataFormatada = DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
                    string nomeArquivoZip = $"{diretorioTemporario}{volumeAlias}-{dataFormatada}.zip";

                    // Cria o arquivo zipado
                    ZipFile.CreateFromDirectory(volumePath, nomeArquivoZip);

                    if (Directory.Exists(settings.DiretorioCloud))
                    {
                        // TODO: IMPLEMENTAR A LÓGICA DE MOVER ARQUIVO E MANTER O PENULTIMO BACKUP
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"O Diretório {settings.DiretorioCloud} não existe.");
                    }
                }
                else
                {
                    Console.WriteLine($"Diretório não existe -> {volumeAlias}: {volumePath}");
                }
            }
        }
    }
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Erro de autorização: {ex.Message}");
    Console.WriteLine("Certifique-se de que você tem as permissões necessárias para acessar o diretório.");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
}