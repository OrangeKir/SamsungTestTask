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
    public record Response(IDictionary<(string, DateTime), decimal> Attributes);
    
    public class AttributeDto
    {
        public string CustomerId { get; set; }
        public DateTime PostingDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}