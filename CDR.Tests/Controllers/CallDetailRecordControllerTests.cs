
using CDR.Controllers;
using CDR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CDR.Tests.Controllers
{
    public class CallDetailRecordControllerTests
    {
        [Fact]
        public async Task UploadCsv_ValidFile_ReturnsOk()
        {
            // Arrange
            var mockService = new Mock<ICsvProcessingService>();
            var controller = new CallDetailRecordController(mockService.Object);

            var csvContent = @"caller_id,recipient,call_date,end_time,duration,cost,reference,currency
                           441215598896,448000096481,16/08/2016,14:21:33,43,0,C5DA9724701EEBBA95CA2CC5617BA93E4,GBP";
            var file = CreateMockFormFile(csvContent);

            // Act
            var result = await controller.UploadCsv(file);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File processed successfully.", okResult.Value);
        }

        [Fact]
        public async Task UploadCsv_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ICsvProcessingService>();
            var controller = new CallDetailRecordController(mockService.Object);

            var csvContent = @"";
            var file = CreateMockFormFile(csvContent);

            // Act
            var result = await controller.UploadCsv(file);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("No file uploaded or file is empty.", okResult.Value);
        }

        [Fact]
        public async Task UploadCsv_NoFile_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ICsvProcessingService>();
            var controller = new CallDetailRecordController(mockService.Object);

            // Act
            var result = await controller.UploadCsv(null);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("No file uploaded or file is empty.", okResult.Value);
        }

        [Fact]
        public async Task UploadCsv_ExceptionOccured_ReturnsInternalServerError()
        {
            // Arrange
            var mockService = new Mock<ICsvProcessingService>();
            mockService.Setup((s) => s.ProcessCsvFile(It.IsAny<IFormFile>())).ThrowsAsync(new Exception());

            var controller = new CallDetailRecordController(mockService.Object);

            // Act
            var result = await controller.UploadCsv(null);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
            Assert.Equal("No file uploaded or file is empty.", okResult.Value);
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
