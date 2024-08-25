using Microsoft.AspNetCore.Mvc;
using SamsungTestTask.Infrastructure.Models;
using SamsungTestTask.Service;

namespace SamsungTestTask.Controllers.V1;

[ApiController]
[Route("record/v1")]
public class RecordController : ControllerBase
{
    private readonly RecordService _recordService;

    public RecordController(RecordService recordService)
    {
        _recordService = recordService;
    }

    [HttpPut("create")]
    public async Task CreateRecordFromString(string source, DataType dataType, CancellationToken cancellationToken)
    {
        await _recordService.CreateRecords(source, dataType, cancellationToken);
    }
}