using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using SamsungTestTask.Infrastructure.Models;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using DataType = SamsungTestTask.Infrastructure.Models.DataType;

namespace SamsungTestTask.Service.Helpers;

public static class RecordParserHelper
{
    public static IReadOnlyCollection<RecordData> Parse(string source, DataType dataType)
    {
        var parsedData = dataType switch
        {
            DataType.Csv => ParseCsv(source),
            DataType.Xml => ParseXml(source),
            DataType.Json => ParseJson(source),
            _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null)
        };

        return parsedData.Select(x => new RecordData
        {
            CustomerId = x.CustNo,
            PostingDate = x.PostingDate,
            Amount = x.Amount,
        }).ToArray();
    }
    
    private static IReadOnlyCollection<RecordDataDto> ParseCsv(string source)
    {
        var csvParserOptions = new CsvParserOptions(true, ';');
        var csvReaderOptions = new CsvReaderOptions(["\n"]);
        var csvMapper = new CsvRecordDataDtoMapping();
        var csvParser = new CsvParser<RecordDataDto>(csvParserOptions, csvMapper);

        return csvParser.ReadFromString(csvReaderOptions, source).Select(x => x.Result).ToArray();
    }

    private static IReadOnlyCollection<RecordDataDto> ParseXml(string source)
    {
        var serializer = new XmlSerializer(typeof(XmlRecordDataDto));
        
        var parsedData = (XmlRecordDataDto?)serializer.Deserialize(new XmlTextReader(source));

        if (parsedData is null)
            throw new ValidationException("XML file could not be parsed.");

        return parsedData.Entries;
    }

    private static IReadOnlyCollection<RecordDataDto> ParseJson(string source)
    {
        var parsedData = JsonSerializer.Deserialize<JsonRecordDataDto>(source);

        if (parsedData is null)
            throw new ValidationException("JSON file could not be parsed.");

        return parsedData.Entries;
    }
    
    public class RecordDataDto
    {
        public string CustNo { get; set; } = null!;
        public DateTime PostingDate { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class JsonRecordDataDto
    {
        public IReadOnlyCollection<RecordDataDto> Entries { get; set; } = null!;
    }
    
    public class XmlRecordDataDto
    {
        [XmlArray("Entry")]
        public IReadOnlyCollection<RecordDataDto> Entries { get; set; } = null!;
    }
    
    private class CsvRecordDataDtoMapping : CsvMapping<RecordDataDto>
    {
        public CsvRecordDataDtoMapping()
        {
            MapProperty(0, x => x.CustNo);
            MapProperty(1, x => x.PostingDate);
            MapProperty(2, x => x.Amount);
        }
    }

}