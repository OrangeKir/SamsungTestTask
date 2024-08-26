using Dapper;
using MediatR;
using SamsungTestTask.Infrastructure;
using static SamsungTestTask.DataAccess.Handlers.V1.CreateRecords;

namespace SamsungTestTask.DataAccess.Handlers.V1;

public class CreateRecordsHandler : IRequestHandler<Request>
{
    private const string Query = @$"
        INSERT INTO {PgTables.Record} (customer_id, posting_date, amount) VALUES (@CustomerId, @PostingDate, @Amount)";

    private readonly IDbContext _dbContext;

    public CreateRecordsHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
        await using var dbConnection = _dbContext.GetConnection();
        await dbConnection.OpenAsync(cancellationToken);

        await dbConnection.ExecuteAsync(Query, request.RecordsData);
    }
}