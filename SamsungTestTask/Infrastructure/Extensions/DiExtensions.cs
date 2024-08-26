using Dapper;

namespace SamsungTestTask.Infrastructure.Extensions;

public static class DiExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }
}