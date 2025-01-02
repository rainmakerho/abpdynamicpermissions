using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Acme.BookStore.Data;

public class BookStoreDbContextFactory : IDesignTimeDbContextFactory<BookStoreDbContext>
{
    public BookStoreDbContext CreateDbContext(string[] args)
    {
        BookStoreEfCoreEntityExtensionMappings.Configure();
        
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<BookStoreDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new BookStoreDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
