﻿using MediatR;
using SamsungTestTask.DataAccess.Handlers.V1;
using SamsungTestTask.Infrastructure.Extensions;
using SamsungTestTask.Infrastructure.Models;
using SamsungTestTask.Service.Helpers;

namespace SamsungTestTask.Service;

public class RecordService
{
    private readonly IMediator _mediator;
    private readonly ILogger<RecordService> _logger;
    private const decimal MaxCustomerAmountPerDay = 100;

    public RecordService(IMediator mediator, ILogger<RecordService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task CreateRecords(string source, DataType dataType, CancellationToken cancellationToken)
    {
        var data = RecordParserHelper.Parse(source, dataType);
        data = await FilterRecords(data, cancellationToken);
        var request = new CreateRecords.Request(data);
        await _mediator.Send(request, cancellationToken);
    }

    private async Task<IReadOnlyCollection<RecordData>> FilterRecords(IReadOnlyCollection<RecordData> records, CancellationToken cancellationToken)
    {
        var (validRecords, recordsWithNotPositiveAmount) = records.Fork(x => x.Amount > 0);

        foreach (var record in recordsWithNotPositiveAmount)
            _logger.LogWarning(string.Format("Posting at {0} from customer with Id {1} has not positive amount",
                record.PostingDate.Date, record.CustomerId));

        var validRecordsArray = validRecords.ToArray();
        var attributesRequest = new GetRecordsCreationAttributes.Request(
            validRecordsArray.Select(x => x.CustomerId).ToArray(),
            validRecordsArray.Select(x => x.PostingDate).ToArray());
        
        var creationAttributes = await _mediator.Send(attributesRequest, cancellationToken);

        (validRecords, var recordsWithManyAmount) = validRecordsArray
            .Fork(x
                => (creationAttributes.Attributes.TryGetValue((x.CustomerId, x.PostingDate), out var value)
                    ? value
                    : 0)
                + x.Amount <= MaxCustomerAmountPerDay);

        foreach (var record in recordsWithManyAmount)
            _logger.LogWarning(string.Format("Customer with Id {0} already has total amount at date {1}, that more then day limit {2}",
                record.CustomerId, record.PostingDate, MaxCustomerAmountPerDay));
        
        return validRecords.ToArray();
    }
}