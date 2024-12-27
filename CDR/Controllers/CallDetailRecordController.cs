using CDR.Services;
using Microsoft.AspNetCore.Mvc;

namespace CDR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallDetailRecordController : ControllerBase
    {
        private readonly ICsvProcessingService _csvProcessingService;

        public CallDetailRecordController(ICsvProcessingService csvProcessingService)
        {
            _csvProcessingService = csvProcessingService;
        }

        /// <summary>
        /// Upload and save a CSV file to DB
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <remarks>
        /// <p>Items in file should have heading row following with the data.</p> 
        /// <p>Columns should be in this order:
        ///     caller_id, recipient, call_date, end_time, duration, cost, reference, currency</p>
        /// </remarks>
        /// <response code="200">"File processed successfully."</response>
        /// <response code="400">"No file uploaded or file is empty."</response>
        /// <response code="500">"An error occurred: [error message]."</response>
        [HttpPost("upload-csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
       
        public async Task<IActionResult> UploadCsv(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return StatusCode(400, "No file uploaded or file is empty.");
            }

            try
            {
                await _csvProcessingService.ProcessCsvFile(file);
                return Ok("File processed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
