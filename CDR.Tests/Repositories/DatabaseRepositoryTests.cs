using CDR.Models;
using CDR.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CDR.Tests.Repositories
{
    public class DatabaseRepositoryTests
    {
        [Fact]
        public async Task SaveRecordsAsync_ValidRecord_SavesToDatabase()
        {
            // Arrange
            DbContextOptions<CallDetailRecordDBContext> options = 
                new DbContextOptionsBuilder<CallDetailRecordDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            using var dbContext = new CallDetailRecordDBContext(options);
            var repository = new DatabaseRepository(dbContext);

            var callRecords = new List<CallDetailRecord>
        {
            new() {
                CallerId = "441215598896",
                Recipient = "448000096481",
                CallDate = DateTime.Parse("16/08/2016"),
                EndTime = TimeOnly.Parse("14:21:33"),
                Duration = 43,
                Cost = 0,
                Reference = "C5DA9724701EEBBA95CA2CC5617BA93E4",
                Currency = "GBP"
            }
        };

            // Act
            await repository.SaveRecordsAsync(callRecords);

            // Assert
            var records = await dbContext.CDRs.ToListAsync();
            Assert.Single(records);
            Assert.Equal("C5DA9724701EEBBA95CA2CC5617BA93E4", records[0].Reference);
        }

        [Fact]
        public async Task SaveRecordsAsync_ValidMultipleRecords_SavesToDatabase()
        {
            // Arrange
            DbContextOptions<CallDetailRecordDBContext> options =
                new DbContextOptionsBuilder<CallDetailRecordDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDB1")
                .Options;

            using var dbContext = new CallDetailRecordDBContext(options);
            var repository = new DatabaseRepository(dbContext);

            var callRecords = new List<CallDetailRecord>
        {
            new() {
                CallerId = "441215598896",
                Recipient = "448000096481",
                CallDate = DateTime.Parse("16/08/2016"),
                EndTime = TimeOnly.Parse("14:21:33"),
                Duration = 43,
                Cost = 0,
                Reference = "C5629724701EEBBA95CA2CC5617BA93E4",
                Currency = "GBP"
            },
            new() {
                CallerId = "441215896896",
                Recipient = "448068596481",
                CallDate = DateTime.Parse("16/08/2016"),
                EndTime = TimeOnly.Parse("14:51:35"),
                Duration = 123,
                Cost = 0.006m,
                Reference = "C659572470152HOH95CA2CC5617BA93E4",
                Currency = "GBP"
            }
        };

            // Act
            await repository.SaveRecordsAsync(callRecords);

            // Assert
            var records = await dbContext.CDRs.ToListAsync();
            Assert.Equal(2, records.Count);
        }

        [Fact]
        public async Task SaveRecordsAsync_InvalidSameRecords_ThrowsError()
        {
            // Arrange
            DbContextOptions<CallDetailRecordDBContext> options =
                new DbContextOptionsBuilder<CallDetailRecordDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDB2")
                .Options;

            using var dbContext = new CallDetailRecordDBContext(options);
            var repository = new DatabaseRepository(dbContext);

            var callRecords = new List<CallDetailRecord>
        {
            new() {
                CallerId = "441215598896",
                Recipient = "448000096481",
                CallDate = DateTime.Parse("16/08/2016"),
                EndTime = TimeOnly.Parse("14:21:33"),
                Duration = 43,
                Cost = 0,
                Reference = "C5629724701EEBBA95CA2CC5617BA93E4",
                Currency = "GBP"
            },
            new() {
                CallerId = "441215598896",
                Recipient = "448000096481",
                CallDate = DateTime.Parse("16/08/2016"),
                EndTime = TimeOnly.Parse("14:21:33"),
                Duration = 43,
                Cost = 0,
                Reference = "C5629724701EEBBA95CA2CC5617BA93E4",
                Currency = "GBP"
            },
        };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await repository.SaveRecordsAsync(callRecords);
            });
        }
    }
}
