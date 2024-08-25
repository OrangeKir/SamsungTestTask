using MediatR;

namespace SamsungTestTask.DataAccess.Handlers.V1;

public class GetRecordsCreationAttributes
{
    public record Request(IReadOnlyCollection<string> CustomerIds, IReadOnlyCollection<DateTime> PostingDates) : IRequest<Response>
    {
        public readonly object AsParameters = new
        {
            CustomerIds,
            PostingDates
        };
    }
    public record Response(IReadOnlyCollection<Attribute> Attributes);
    public class Attribute
    {
        public string CustomerId { get; set; }
        public DateTime PostingDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}