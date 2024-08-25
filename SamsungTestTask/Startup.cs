using SamsungTestTask.DataAccess.Migrations;
using SamsungTestTask.Infrastructure;
using FluentMigrator.Runner;
using SamsungTestTask.Service;
using SimpleInjector;

namespace SamsungTestTask;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        Migrate();
        var type = typeof(Startup);

        services.AddSingleton<IDbContext, DbContext>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SchemaGeneratorOptions.SchemaIdSelector = t => t.ToFriendlyName();
        });
        services.AddControllers();
        services.AddMediatR(x => x.RegisterServicesFromAssembly(type.Assembly));
        services.AddScoped<RecordService>();
    }

    public void Configure(IApplicationBuilder app)
        => app
            .UseRouting()
            .UseCors()
            .UseAuthorization()
            .UseSwagger()
            .UseSwaggerUI()
            .UseEndpoints(endpoints => { endpoints.MapControllers(); });

    private void Migrate()
    {
        using var scope = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(_configuration.GetConnectionString("Default"))
                .ScanIn(typeof(InitMigration).Assembly).For.Migrations())
            .BuildServiceProvider(false)
            .CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IMigrationRunner>()
            .MigrateUp();
    }
}