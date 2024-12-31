﻿using CDR.Models;
using CDR.Repositories;

namespace CDR.Services
{
    public class CDRAnalyticsService : ICDRAnalyticsService
    {
        private readonly IDatabaseRepository _repository;

        public CDRAnalyticsService(IDatabaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<double> AverageCallDuration(string? callerId, DateTime? startDate, DateTime? endDate)
        {
            var data = new List<CallDetailRecord>();
            if (!string.IsNullOrEmpty(callerId)) 
            {
                data = await _repository.GetRecordsByCallerIdAsync(callerId);
            }
            else if (startDate != null && endDate != null) 
            {
                data = await _repository.GetRecordsByDateRangeAsync((DateTime)startDate, (DateTime)endDate);
            }

            if (data.Count == 0) return 0;

            var result = data.Average(d => d.Duration);
            return Math.Round(result, 3, MidpointRounding.ToZero);
        }
    }
}