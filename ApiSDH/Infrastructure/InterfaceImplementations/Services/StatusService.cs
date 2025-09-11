using Application.Common.Interfaces.Services;

namespace Infrastructure.InterfaceImplementations.Services;

public class StatusService : IStatusService
{
    // Could come from config
    private readonly string _statusFolder = Path.Combine("logs", "status");

    public StatusService()
    {
        if (!Directory.Exists(_statusFolder))
            Directory.CreateDirectory(_statusFolder);
    }

    public void Write(string message)
    {
        var fileName = $"Status{DateTime.Now:yyyy-MM-dd}.txt";
        var filePath = Path.Combine(_statusFolder, fileName);

        var logEntry = $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}";
        File.AppendAllText(filePath, logEntry);
    }
}