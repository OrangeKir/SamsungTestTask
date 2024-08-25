using MediatR;
using SamsungTestTask.Infrastructure.Models;

namespace SamsungTestTask.DataAccess.Handlers.V1;

public class CreateRecords
{
    public record Request(IReadOnlyCollection<RecordData> RecordsData) : IRequest;
}