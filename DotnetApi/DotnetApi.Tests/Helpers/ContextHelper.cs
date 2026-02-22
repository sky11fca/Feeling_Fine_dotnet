using DotnetApi.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Tests.Helpers;

public class ContextHelper
{
    public static ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var dbContext = new ApplicationDbContext(options);
        return dbContext;
    }
}