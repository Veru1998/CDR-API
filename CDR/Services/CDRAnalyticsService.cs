using CDR.Models;
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

        public async Task<int> CallVolume(DateTime startDate, DateTime endDate)
        {
            return (await _repository.GetRecordsByDateRangeAsync(startDate, endDate)).Count;
        }

        public async Task<List<CallDetailRecord>> CostCalls(decimal threshhold, bool higher)
        {
            var data = await _repository.GetAllCDRs();
            data = higher 
                ? data.Where(d => d.Cost >= threshhold).ToList() 
                : data.Where(d => d.Cost <= threshhold).ToList();

            return data;
        }

        public async Task<decimal> TotalCallCost(string callerId)
        {
            var data = await _repository.GetRecordsByCallerIdAsync(callerId);
            return data.Sum(d => d.Cost);
        }

        public async Task<int> TotalCalls(string callerId, DateTime? startDate, DateTime? endDate)
        {
            var data = await _repository.GetRecordsByCallerIdAsync(callerId);
            
            if (startDate != null && endDate != null)
            {
                data = data.Where(record => record.CallDate >= startDate && record.CallDate <= endDate).ToList();
            }
           
            return data.Count;
        }

        public async Task<SummaryReturnType?> SummaryByDateRange(DateTime startDate, DateTime endDate)
        {
            var data = await _repository.GetRecordsByDateRangeAsync(startDate, endDate);

            if (data.Count == 0) return null;
            
            return new SummaryReturnType
            {
                TotalCalls = data.Count,
                TotalCost = data.Sum(d => d.Cost),  
                AverageDuration = Math.Round(data.Average(d => d.Duration), 3, MidpointRounding.ToZero),
                MostFrequentCaller = data.GroupBy(d => d.CallerId)
                .Select(d => new { CallerId = d.Key, Count = d.Count() })
                .OrderByDescending(d => d.Count)
               .FirstOrDefault()?.CallerId ?? "",
                MostFrequentRecipient = data.GroupBy(d => d.Recipient)
                .Select(d => new { Recipient = d.Key, Count = d.Count() })
                .OrderByDescending(d => d.Count)
               .FirstOrDefault()?.Recipient ?? ""
            };
        }
    }
}
