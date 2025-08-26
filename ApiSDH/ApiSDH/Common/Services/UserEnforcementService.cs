using Application.Common.Interfaces.Persistence;
using Domain.Entities;

namespace ApiSDH.Common.Services;

public static class UserEnforcementService
{
    public static void EnsureSingleUser(ISensorContext context)
    {
        var users = context.Users.ToList();

        if (users.Count == 0)
        {
            context.Users.Add(new UserInfo());
            context.SaveChanges();
        }
        else if (users.Count > 1)
        {
            throw new InvalidOperationException("Expected exactly 1 user in the system.");
        }
    }
}