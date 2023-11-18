using System.IO.Compression;

using Microsoft.Extensions.Configuration;
using SSV.Console;
using SSV.Console.Utilities;

IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

Settings? settings = config.GetRequiredSection("Settings").Get<Settings>();

try
{
    string diretorioTemporario = FileHandler.CriarDiretorio("temp");

    if (settings?.DiretoriosDosVolumes?.Count > 0)
    {
        foreach (Dictionary<string, string> diretorioInfo in settings.DiretoriosDosVolumes)
        {
            FileHandler.ProcessarVolumes(settings.DiretorioCloud, diretorioTemporario, diretorioInfo, 1);
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