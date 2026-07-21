using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        if (!configuration.GetValue("Database:Initialize", true))
            return;

        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();

        if (configuration.GetValue("Database:SeedDemoData", true))
            await DemoDataSeeder.SeedAsync(context);
    }
}
