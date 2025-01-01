
using CDR.Controllers;
using CDR.Models;
using CDR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CDR.Tests.Controllers
{
    public class CallDetailRecordControllerTests
    {
        private readonly CallDetailRecordController _controller;
        private readonly Mock<ICDRAnalyticsService> _analyticsService;
        private readonly Mock<ICsvProcessingService> _csvProcessingService;

        public CallDetailRecordControllerTests() 
        {
            _csvProcessingService = new Mock<ICsvProcessingService>();
            _analyticsService = new Mock<ICDRAnalyticsService>();

            _controller = new CallDetailRecordController(_csvProcessingService.Object, _analyticsService.Object);
        }

        [Fact]
        public async Task UploadCsv_ValidFile_ReturnsOk()
        {
            // Arrange
            var csvContent = @"caller_id,recipient,call_date,end_time,duration,cost,reference,currency
                           441215598896,448000096481,16/08/2016,14:21:33,43,0,C5DA9724701EEBBA95CA2CC5617BA93E4,GBP";
            var file = CreateMockFormFile(csvContent);

            // Act
            var result = await _controller.UploadCsv(file);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File processed successfully.", okResult.Value);
        }

        [Fact]
        public async Task UploadCsv_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var csvContent = @"";
            var file = CreateMockFormFile(csvContent);

            // Act
            var result = await _controller.UploadCsv(file);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("No file uploaded or file is empty.", okResult.Value);
        }

        [Fact]
        public async Task UploadCsv_NoFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadCsv(null);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("No file uploaded or file is empty.", okResult.Value);
        }

        [Fact]
        public async Task UploadCsv_ExceptionOccured_ReturnsInternalServerError()
        {
            // Act
            var result = await _controller.UploadCsv(null);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("No file uploaded or file is empty.", okResult.Value);
        }

        [Fact]
        public async Task AverageCallDuration_ValidParams_ReturnsOk()
        {
            // Arrange
            _analyticsService.Setup(x => x.AverageCallDuration(It.IsAny<string>(), null, null))
                .ReturnsAsync(59);

            // Act
            var result = await _controller.GetAverageCallDuration("456985656589", null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<double>(okResult.Value);
            Assert.Equal(59, value);
        }

        [Fact]
        public async Task AverageCallDuration_InvalidParams_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetAverageCallDuration(null, null, null);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("Wrong parameters provided. Provide either callerId or time range.", okResult.Value);
        }

        [Fact]
        public async Task AverageCallDuration_InvalidParams_ReturnsInternalServerError()
        {
            // Act
            var result = await _controller.GetAverageCallDuration(null, " v", "v");

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, okResult.StatusCode);
        }

        [Fact]
        public async Task CallVolume_ValidParams_ReturnsOk()
        {
            // Arrange
            _analyticsService.Setup(x => x.CallVolume(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(16);

            // Act
            var result = await _controller.GetCallVolume("15/09/2016", "15/09/2016");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<int>(okResult.Value);
            Assert.Equal(16, value);
        }

        [Fact]
        public async Task CallVolume_InvalidParams_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetCallVolume("", " ");

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("Wrong parameters provided. Provide valid time range.", okResult.Value);
        }

        [Fact]
        public async Task CallVolume_InvalidParams_ReturnsInternalServerError()
        {
            // Act
            var result = await _controller.GetCallVolume("v", " v");

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, okResult.StatusCode);
        }

        [Fact]
        public async Task HighCostCalls_ValidParams_ReturnsOk()
        {
            // Arrange
            _analyticsService.Setup(x => x.CostCalls(It.IsAny<decimal>(), true))
                .ReturnsAsync([
                    new()
                    {
                        CallerId = "441301978896",
                        Recipient = "446658996481",
                        CallDate = DateTime.Parse("15/09/2016"),
                        EndTime = TimeOnly.Parse("07:23:33"),
                        Duration = 23,
                        Cost = 0.53m,
                        Reference = "C5629724701KDO9395CA2CC5617BA93E4",
                        Currency = "GBP"
                    }]);

            // Act
            var result = await _controller.GetHighCostCalls(0.5m);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ListReturnTypeModel>(okResult.Value);
            Assert.Equal(1, value.Count);
        }

        [Fact]
        public async Task HighCostCalls_InvalidParams_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetHighCostCalls(0);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("Threshhold cannot be 0.", okResult.Value);
        }

        [Fact]
        public async Task LowCostCalls_ValidParams_ReturnsOk()
        {
            // Arrange
            _analyticsService.Setup(x => x.CostCalls(It.IsAny<decimal>(), false))
                .ReturnsAsync([
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
                    }]);

            // Act
            var result = await _controller.GetLowCostCalls(0.2m);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ListReturnTypeModel>(okResult.Value);
            Assert.Equal(1, value.Count);
        }

        [Fact]
        public async Task LowCostCalls_InvalidParams_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetLowCostCalls(0);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("Threshhold cannot be 0.", okResult.Value);
        }

        [Fact]
        public async Task TotalCallCost_ValidParams_ReturnsOk()
        {
            // Arrange
            _analyticsService.Setup(x => x.TotalCallCost(It.IsAny<string>()))
                .ReturnsAsync(1.235m);

            // Act
            var result = await _controller.GetTotalCallCost("441217966476");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<decimal>(okResult.Value);
            Assert.Equal(1.235m, value);
        }

        [Fact]
        public async Task TotalCallCost_InvalidParams_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetTotalCallCost(null);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("Wrong parameters provided. Provide valid either callerId or date.", okResult.Value);
        }

        [Fact]
        public async Task TotalCalls_ValidParams_ReturnsOk()
        {
            // Arrange
            _analyticsService.Setup(x => x.TotalCalls(It.IsAny<string>(), null, null))
                .ReturnsAsync(126);

            // Act
            var result = await _controller.GetTotalCalls("458965896542", null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<int>(okResult.Value);
            Assert.Equal(126, value);
        }

        [Fact]
        public async Task TotalCalls_InvalidParams_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetTotalCalls("458965896542", "18/08/2016", null);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("Wrong parameters provided. Provide either callerId.", okResult.Value);
        }

        [Fact]
        public async Task TotalCalls_InvalidParams_ReturnsInternalServerError()
        {
            // Act
            var result = await _controller.GetTotalCalls("458965896542", " v", "v");

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, okResult.StatusCode);
        }

        private static FormFile CreateMockFormFile(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "file", "test.csv");
        }
    }
}
