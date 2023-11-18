using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using SBV.Console;
using SBV.Console.Utilities;

class Program
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow ();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow (IntPtr hWnd, int nCmdShow);

    private const int SW_HIDE = 0;

    private static void Main ()
    {
        // Obtém o identificador da janela do console
        IntPtr hWnd = GetConsoleWindow();

        // Esconde a janela do console
        ShowWindow(hWnd, SW_HIDE);

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
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }
}