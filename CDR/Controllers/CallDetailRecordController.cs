﻿using CDR.Services;
using Microsoft.AspNetCore.Mvc;

namespace CDR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallDetailRecordController : ControllerBase
    {
        private readonly ICsvProcessingService _csvProcessingService;
        private readonly ICDRAnalyticsService _analyticsService;

        public CallDetailRecordController(ICsvProcessingService csvProcessingService, ICDRAnalyticsService analyticsService)
        {
            _csvProcessingService = csvProcessingService;
            _analyticsService = analyticsService;
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

        /// <summary>
        /// Get average call duration for caller id or for some time range.
        /// </summary>
        /// <param name="callerId">Id of the caller.</param>
        /// <param name="startDate">Start date of the range.</param>
        /// <param name="endDate">End date of the range.</param>
        /// <remarks>
        /// <p>You can use either callerId or time range. Combination is not supported.</p> 
        /// <p>If all params are provided, callerId takes priority.</p>
        /// </remarks>
        /// <returns>Returns average value for selected parameters.</returns>
        /// <response code="400">"Wrong parameters provided. Provide either callerId or time range."</response>
        /// <response code="500">"An error occurred: [error message]."</response>
        [HttpGet("avg-call-duration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAverageCallDuration(string? callerId, string? startDate, string? endDate)
        {
            if (string.IsNullOrEmpty(callerId) && (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate)))
            {
                return StatusCode(400, "Wrong parameters provided. Provide either callerId or time range.");
            }

            try
            {
                var result = await _analyticsService.AverageCallDuration(callerId, 
                    !string.IsNullOrEmpty(startDate) ? DateTime.Parse(startDate) : null, 
                    !string.IsNullOrEmpty(endDate) ? DateTime.Parse(endDate) : null);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
