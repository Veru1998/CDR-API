using CDR.Models;
using CDR.Repositories;
using CDR.Services;
using Moq;

namespace CDR.Tests.Services
{
    public class CDRAnalyticsServiceTests
    {
        private readonly ICDRAnalyticsService _service;
        private readonly Mock<IDatabaseRepository> _repository;
        private List<CallDetailRecord> costCallCDRs = new List<CallDetailRecord>()
        {
            new()
            {
                CallerId = "441215598896",
                Recipient = "448005636481",
                CallDate = DateTime.Parse("16/08/2016"),
                EndTime = TimeOnly.Parse("14:21:33"),
                Duration = 43,
                Cost = 0,
                Reference = "C5629724701EEBBA95CA2CC5617BA93E4",
                Currency = "GBP"
            },
            new()
            {
                CallerId = "441215896896",
                Recipient = "446548596481",
                CallDate = DateTime.Parse("16/08/2016"),
                EndTime = TimeOnly.Parse("14:51:35"),
                Duration = 123,
                Cost = 0.506m,
                Reference = "C659572470152HOH95CA2CC5617BA93E4",
                Currency = "GBP"
            },
            new()
            {
                CallerId = "441301978896",
                Recipient = "446548596481",
                CallDate = DateTime.Parse("20/09/2016"),
                EndTime = TimeOnly.Parse("07:23:33"),
                Duration = 49,
                Cost = 0.013m,
                Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                Currency = "GBP"
            },
            new()
            {
                CallerId = "441032594896",
                Recipient = "448068596481",
                CallDate = DateTime.Parse("16/09/2016"),
                EndTime = TimeOnly.Parse("16:01:35"),
                Duration = 23,
                Cost = 0.5m,
                Reference = "C659572456KD8GHOH95CA2CC5617BA93E4",
                Currency = "GBP"
            },
            new()
            {
                CallerId = "441032594896",
                Recipient = "448068596481",
                CallDate = DateTime.Parse("16/09/2016"),
                EndTime = TimeOnly.Parse("16:01:35"),
                Duration = 23,
                Cost = 0.15m,
                Reference = "C659572456KD8958H95CA2CC5617BA93E4",
                Currency = "GBP"
            }
        };

        public CDRAnalyticsServiceTests()
        {
            _repository = new Mock<IDatabaseRepository>();
            _service = new CDRAnalyticsService(_repository.Object);
        }

        [Fact]  
        public async Task AverageCallDurationByCallerId_ValidRecords_ReturnsData()
        {
            // Arrange
            _repository.Setup(x => x.GetRecordsByCallerIdAsync(It.IsAny<string>()))
                .ReturnsAsync(
                [
                    new() 
                    {
                        CallerId = "441301978896",
                        Recipient = "446548596481",
                        CallDate = DateTime.Parse("20/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 49,
                        Cost = 0.013m,
                        Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                        Currency = "GBP"
                    },
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446658996481",
                        CallDate = DateTime.Parse("20/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 23,
                        Cost = 0.013m,
                        Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                        Currency = "GBP"
                    },
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446665985681",
                        CallDate = DateTime.Parse("20/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 95,
                        Cost = 0.013m,
                        Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                        Currency = "GBP"
                    }
                ]);

            // Act
            var result = await _service.AverageCallDuration("441301978896", null, null);

            // Assert
            Assert.Equal(55.666, result);
        }

        [Fact]
        public async Task AverageCallDurationByDateRange_ValidRecords_ReturnsData()
        {
            // Arrange
            _repository.Setup(x => x.GetRecordsByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(
                [
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446658996481",
                        CallDate = DateTime.Parse("15/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 23,
                        Cost = 0.013m,
                        Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                        Currency = "GBP"
                    },
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446665985681",
                        CallDate = DateTime.Parse("17/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 95,
                        Cost = 0.013m,
                        Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                        Currency = "GBP"
                    }
                ]);

            // Act
            var result = await _service.AverageCallDuration(null, DateTime.Parse("15/09/2016"), DateTime.Parse("17/09/2016"));

            // Assert
            Assert.Equal(59, result);
        }

        [Fact]
        public async Task AverageCallDuration_NoRecords_ReturnsZero()
        {
            // Arrange
            _repository.Setup(x => x.GetRecordsByCallerIdAsync(It.IsAny<string>()))
                .ReturnsAsync([]);

            // Act
            var result = await _service.AverageCallDuration("441301978896", null, null);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CallVolume_ValidRecords_ReturnsData()
        {
            // Arrange
            _repository.Setup(x => x.GetRecordsByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(
                [
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446658996481",
                        CallDate = DateTime.Parse("15/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 23,
                        Cost = 0.013m,
                        Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                        Currency = "GBP"
                    },
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446665985681",
                        CallDate = DateTime.Parse("17/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 95,
                        Cost = 0.013m,
                        Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                        Currency = "GBP"
                    },
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446658996481",
                        CallDate = DateTime.Parse("15/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 23,
                        Cost = 0.013m,
                        Reference = "C5629724701KDKOK95CA2CC5617BA93E4",
                        Currency = "GBP"
                    },
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446665985681",
                        CallDate = DateTime.Parse("17/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 95,
                        Cost = 0.013m,
                        Reference = "C5629724701KO78395CA2CC5617BA93E4",
                        Currency = "GBP"
                    }
                ]);

            // Act
            var result = await _service.CallVolume(DateTime.Parse("15/09/2016"), DateTime.Parse("17/09/2016"));

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public async Task CostCalls_ValidRecordsHigher_ReturnsData()
        {
            // Arrange
            _repository.Setup(x => x.GetAllCDRs()).ReturnsAsync(costCallCDRs);

            // Act 
            var result = await _service.CostCalls(0.5m, true);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CostCalls_ValidRecordsHigher_ReturnsNoData()
        {
            // Arrange
            _repository.Setup(x => x.GetAllCDRs()).ReturnsAsync(costCallCDRs);

            // Act 
            var result = await _service.CostCalls(1, true);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task CostCalls_ValidRecordsLower_ReturnsData()
        {
            // Arrange
            _repository.Setup(x => x.GetAllCDRs()).ReturnsAsync(costCallCDRs);

            // Act 
            var result = await _service.CostCalls(0.2m, false);

            // Assert
            Assert.Equal(3, result.Count);
        }
    }
}
