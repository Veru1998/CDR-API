using CDR.Models;
using CDR.Repositories;
using System.Globalization;

namespace CDR.Services
{
    public class CsvProcessingService : ICsvProcessingService
    {
        private readonly IDatabaseRepository _repository;

        public CsvProcessingService(IDatabaseRepository repository)
        {
            _repository = repository;
        }

        public async Task ProcessCsvFile(IFormFile file)
        {
            var records = new List<CallDetailRecord>();
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            int batchSize = 1000;       // number of rows to process per batch
            int rowCount = 0;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (rowCount == 0)      // skip header
                {
                    rowCount++;
                    continue;
                }

                var record = ParseCsvLine(line);
                if (record != null)
                {
                    records.Add(record);
                }

                rowCount++;

                if (rowCount % batchSize == 0)
                {
                    await _repository.SaveRecordsAsync(records);
                    records.Clear();
                }
            }

            if (records.Count != 0)
            {
                await _repository.SaveRecordsAsync(records);
            }
        }

        private static CallDetailRecord? ParseCsvLine(string? line)
        {
            if (line == null)
            {
                return null;
            }

            var values = line.Split(',');
            var valuesWithoutCallerIdCol = values.Skip(1).ToArray();

            // check whether we have all required values (caller_id can be empty) 
            if (values.Length < 8 || valuesWithoutCallerIdCol.Any(string.IsNullOrEmpty))
            {
                return null;
            }

            return new CallDetailRecord
            {
                CallerId = values[0],
                Recipient = values[1],
                CallDate = DateTime.Parse(values[2]),
                EndTime = TimeOnly.Parse(values[3]),
                Duration = int.Parse(values[4]),
                Cost = decimal.Parse(values[5], CultureInfo.InvariantCulture),
                Reference = values[6],
                Currency = values[7],
            };
        }
    }
}
