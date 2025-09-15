using Application.Common.Interfaces.Persistence;
using Domain.Entities;

namespace ApiSDH.Common.Services;

public static class ConfigEnforcementService
{
    public static void EnsureSingleConfig(ISensorContext context)
    {
        var users = context.Configs.ToList();

        if (users.Count == 0)
        {
            context.Configs.Add(new Config());
            context.SaveChanges();
        }
        else if (users.Count > 1)
        {
            throw new InvalidOperationException("Expected exactly 1 user in the system.");
        }
    }
}