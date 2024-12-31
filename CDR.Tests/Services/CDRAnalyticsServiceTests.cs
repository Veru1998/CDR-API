using CDR.Repositories;
using CDR.Services;
using Moq;

namespace CDR.Tests.Services
{
    public class CDRAnalyticsServiceTests
    {
        private readonly ICDRAnalyticsService _service;
        private readonly Mock<IDatabaseRepository> _repository;

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
    }
}
