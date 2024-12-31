using CDR.Models;
using CDR.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CDR.Tests.Repositories
{
    public class DatabaseRepositoryTests : IClassFixture<DatabaseRepositoryFixture>
    {

        private readonly CallDetailRecordDBContext _dbContext;
        private readonly DatabaseRepository _repository;

        public DatabaseRepositoryTests(DatabaseRepositoryFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _repository = fixture.Repository;
        }

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

        [Fact]
        public async Task GetRecordsByCallerIdAsync_ValidRecords_ReturnsData()
        {
            // Act
            var result = await _repository.GetRecordsByCallerIdAsync("441301978896");

            // Assert
            Assert.Single(result);
            Assert.Equal("C5629724701KDO9395CA2CC5617BA93E4", result[0].Reference);
        }

        [Fact]
        public async Task GetRecordsByCallerIdAsync_NoRecords_ReturnsEmptyData()
        {
            // Act
            var result = await _repository.GetRecordsByCallerIdAsync("441306558896");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRecordsByRecipientAsync_NoRecords_ReturnsEmptyData()
        {
            // Act
            var result = await _repository.GetRecordsByRecipientAsync("446548336481");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRecordsByDateAsync_ValidRecords_ReturnsData()
        {
            // Act
            var result = await _repository.GetRecordsByDateAsync(DateTime.Parse("16/08/2016"));

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("C5629724701EEBBA95CA2CC5617BA93E4", result[0].Reference);
            Assert.Equal("C659572470152HOH95CA2CC5617BA93E4", result[1].Reference);
        }

        [Fact]
        public async Task GetRecordsByDateAsync_NoRecords_ReturnsEmptyData()
        {
            // Act
            var result = await _repository.GetRecordsByDateAsync(DateTime.Parse("10/08/2016"));

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetXRecordsByDateAsync_AscendingWithoutDate_ReturnsData()
        {
            // Act
            var result = await _repository.GetXRecordsByDateAsync(10, true, null, null);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("C5629724701EEBBA95CA2CC5617BA93E4", result[0].Reference);
        }

        [Fact]
        public async Task GetXRecordsByDateAsync_DescendingWithoutDate_ReturnsData()
        {
            // Act
            var result = await _repository.GetXRecordsByDateAsync(10, false, null, null);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("C5629724701KDO9395CA2CC5617BA93E4", result[0].Reference);
        }

        [Fact]
        public async Task GetXRecordsByDateAsync_AscendingWithDate_ReturnsData()
        {
            // Act
            var result = await _repository.GetXRecordsByDateAsync(10, true, DateTime.Parse("16/08/2016"), DateTime.Parse("16/09/2016"));

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("C5629724701EEBBA95CA2CC5617BA93E4", result[0].Reference);
        }

        [Fact]
        public async Task GetXRecordsByDateAsync_DescendingWithDate_ReturnsData()
        {
            // Act
            var result = await _repository.GetXRecordsByDateAsync(10, false, DateTime.Parse("16/08/2016"), DateTime.Parse("16/09/2016"));

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("C659572456KD8GHOH95CA2CC5617BA93E4", result[0].Reference);
        }

        [Fact]
        public async Task GetXRecordsByDateAsync_DescendingWithDate_ReturnsNoData()
        {
            // Act
            var result = await _repository.GetXRecordsByDateAsync(10, false, DateTime.Parse("10/09/2016"), DateTime.Parse("15/09/2016"));

            // Assert
            Assert.Empty(result);
        }
    }
}
