using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSV.Console.Utilities;

public static class Logger
{
    private static readonly string _logFileName = "log\\log.txt";
    private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logFileName);

    public static void LogMessage (string message)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

        // Adiciona a mensagem ao arquivo de log
        File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
    }

    public static void LogException (Exception exception)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - EXCEPTION: {exception.GetType().FullName} - {exception.Message}";

        // Adiciona detalhes da exceção ao arquivo de log
        File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
    }
}