using Application.Common.Interfaces.Services;

namespace Infrastructure.InterfaceImplementations.Services;

public class StatusService : IStatusService
{
    // Could come from config
    private static string StatusFolder;

    public StatusService()
    {
        //if (!Directory.Exists(_statusFolder))
          //  Directory.CreateDirectory(_statusFolder);
          // In Azure, HOME points to D:\home
          var logRoot = Environment.GetEnvironmentVariable("HOME")
                        ?? AppContext.BaseDirectory;

          StatusFolder = Path.Combine(logRoot, "LogFiles", "status");
          Directory.CreateDirectory(StatusFolder);
    }

    public void Write(string message)
    {
        string fileName = $"Status{DateTime.Now:yyyy-MM-dd}.txt";
        string filePath = Path.Combine(StatusFolder, fileName);

        string logEntry = $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}";
        File.AppendAllText(filePath, logEntry);
    }
}