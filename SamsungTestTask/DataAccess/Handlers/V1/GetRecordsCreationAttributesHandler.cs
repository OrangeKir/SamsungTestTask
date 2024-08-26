using Dapper;
using MediatR;
using SamsungTestTask.Infrastructure;
using static SamsungTestTask.DataAccess.Handlers.V1.GetRecordsCreationAttributes;

namespace SamsungTestTask.DataAccess.Handlers.V1;

public class GetRecordsCreationAttributesHandler : IRequestHandler<Request, Response>
{
    private const string Query = @$"
        SELECT
            customer_id,
            posting_date,
            sum(amount)     AS total_amount
        FROM {PgTables.Record}
        WHERE posting_date = ANY(@PostingDates) AND customer_id = ANY(@CustomerIds)
        GROUP BY customer_id, posting_date;";

    private readonly IDbContext _dbContext;

    public GetRecordsCreationAttributesHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        await using var dbConnection = _dbContext.GetConnection();
        await dbConnection.OpenAsync(cancellationToken);

        var attributes = await dbConnection.QueryAsync<AttributeDto>(Query, request.AsParameters);
        return new Response(attributes.ToDictionary(x=>(x.CustomerId, x.PostingDate), x=> x.TotalAmount));
    }
}