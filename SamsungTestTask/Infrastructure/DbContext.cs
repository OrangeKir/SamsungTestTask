using System.Data.Common;
using Npgsql;

namespace SamsungTestTask.Infrastructure;

public interface IDbContext
{
    public DbConnection GetConnection();
}

public class DbContext : IDbContext
{
    private readonly IConfiguration _configuration;

    public DbContext(IConfiguration configuration)
        => _configuration = configuration;

    public DbConnection GetConnection()
        => new NpgsqlConnection(_configuration.GetConnectionString("Default"));
}