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
    string diretorioTemporario = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp\\");

    if (!Directory.Exists(diretorioTemporario))
    {
        Directory.CreateDirectory(diretorioTemporario);
    }

    if (settings?.DiretoriosDosVolumes?.Count > 0)
    {
        foreach (Dictionary<string, string> diretorioInfo in settings.DiretoriosDosVolumes)
        {
            if (diretorioInfo.TryGetValue("Alias", out var volumeAlias) && diretorioInfo.TryGetValue("Path", out var volumePath))
            {
                if (Directory.Exists(volumePath))
                {
                    //! Adiciona a data formatada ao nome do arquivo ZIP
                    string dataFormatada = DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
                    string nomeArquivoZip = $"{diretorioTemporario}{volumeAlias}-{dataFormatada}.zip";

                    //! Cria o arquivo zipado
                    ZipFile.CreateFromDirectory(volumePath, nomeArquivoZip);

                    //! Vasculha o diretório cloud e remove os arquivos antigos (mantendo os dois últimos)
                    if (Directory.Exists(settings.DiretorioCloud))
                    {
                        string[] files = Directory.GetFiles(settings.DiretorioCloud, "*", SearchOption.AllDirectories);

                        List<string> filesToRemove = [];

                        foreach (string file in files)
                        {
                            if (file.Contains(volumeAlias) && file.EndsWith(".zip"))
                            {
                                filesToRemove.Add(file);
                            }
                        }

                        if (filesToRemove.Count > 1)
                        {
                            filesToRemove.Sort();
                            filesToRemove.Reverse();

                            for (int i = 1; i < filesToRemove.Count; i++)
                            {
                                File.Delete(filesToRemove[i]);
                            }
                        }

                        File.Move(nomeArquivoZip, Path.Combine(settings.DiretorioCloud, Path.GetFileName(nomeArquivoZip)));
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