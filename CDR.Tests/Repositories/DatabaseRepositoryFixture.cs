using CDR.Models;
using CDR.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CDR.Tests.Repositories
{
    public class DatabaseRepositoryFixture : IDisposable
    {
        public CallDetailRecordDBContext DbContext { get; private set; }
        public DatabaseRepository Repository { get; private set; }

        public DatabaseRepositoryFixture()
        {
            var options = new DbContextOptionsBuilder<CallDetailRecordDBContext>()
                .UseInMemoryDatabase(databaseName: "GetRecordsTestDB")
                .Options;

            DbContext = new CallDetailRecordDBContext(options);
            Repository = new DatabaseRepository(DbContext);

            SeedDatabase().Wait();
        }

        private async Task SeedDatabase()
        {
            if (await DbContext.CDRs.AnyAsync())
                return; // Skip seeding if data already exists

            var callRecords = new List<CallDetailRecord>
            {
                new() {
                    CallerId = "441215598896",
                    Recipient = "448005636481",
                    CallDate = DateTime.Parse("16/08/2016"),
                    EndTime = TimeOnly.Parse("14:21:33"),
                    Duration = 43,
                    Cost = 0,
                    Reference = "C5629724701EEBBA95CA2CC5617BA93E4",
                    Currency = "GBP"
                },
                new() {
                    CallerId = "441215896896",
                    Recipient = "446548596481",
                    CallDate = DateTime.Parse("16/08/2016"),
                    EndTime = TimeOnly.Parse("14:51:35"),
                    Duration = 123,
                    Cost = 0.006m,
                    Reference = "C659572470152HOH95CA2CC5617BA93E4",
                    Currency = "GBP"
                },
                new() {
                    CallerId = "441301978896",
                    Recipient = "446548596481",
                    CallDate = DateTime.Parse("20/09/2016"),
                    EndTime = TimeOnly.Parse("07:23:33"),
                    Duration = 49,
                    Cost = 0.013m,
                    Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                    Currency = "GBP"
                },
                new() {
                    CallerId = "441032594896",
                    Recipient = "448068596481",
                    CallDate = DateTime.Parse("16/09/2016"),
                    EndTime = TimeOnly.Parse("16:01:35"),
                    Duration = 23,
                    Cost = 0,
                    Reference = "C659572456KD8GHOH95CA2CC5617BA93E4",
                    Currency = "GBP"
                }
            };

            await Repository.SaveRecordsAsync(callRecords);
        }

        public void Dispose()
        {
            DbContext?.Dispose();
        }
    }
}
