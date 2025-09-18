using Application.Common.Interfaces.Persistence;
using Domain.Entities;

namespace ApiSDH.Common.Services;

public static class ConfigEnforcementService
{
    /// <summary>
    ///     Ensure there is only 1 config in the system. Service runs once on app startup in <see cref="Program" />
    /// </summary>
    public static void EnsureSingleConfig(ISensorContext context)
    {
        var configs = context.Configs.ToList();

        if (configs.Count == 0)
        {
            context.Configs.Add(new Config());
            context.SaveChanges();
        }
        else if (configs.Count > 1)
        {
            throw new InvalidOperationException("Expected exactly 1 config in the system.");
        }
    }
}