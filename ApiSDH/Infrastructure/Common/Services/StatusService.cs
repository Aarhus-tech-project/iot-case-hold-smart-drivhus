using Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Common.Services;

public class StatusService : IStatusService
{
    private static string StatusFolder; // Why static 

    public StatusService(IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("CloudMode"))
        {
            var home = Environment.GetEnvironmentVariable("HOME") ?? "D:\\home";
            StatusFolder = Path.Combine(home, "LogFiles", "status");
        }
        else
        {
            var root = AppContext.BaseDirectory;
            StatusFolder = Path.Combine(root, "logs", "status");
        }

        Directory.CreateDirectory(StatusFolder);
    }

    public void Write(string message)
    {
        var fileName = $"Status{DateTime.Now:yyyy-MM-dd}.txt";
        var filePath = Path.Combine(StatusFolder, fileName);

        var logEntry = $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}";
        File.AppendAllText(filePath, logEntry);
    }
}