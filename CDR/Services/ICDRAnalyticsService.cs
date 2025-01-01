using CDR.Models;

namespace CDR.Services
{
    public interface ICDRAnalyticsService
    {
        Task<double> AverageCallDuration(string? callerId, DateTime? startDate, DateTime? endDate);
        Task<int> CallVolume(DateTime startDate, DateTime endDate);
        Task<List<CallDetailRecord>> CostCalls(decimal threshhold, bool higher);
        Task<decimal> TotalCallCost(string callerId);
        Task<int> TotalCalls(string callerId, DateTime? startDate, DateTime? endDate);
    }
}
