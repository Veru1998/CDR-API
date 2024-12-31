namespace CDR.Services
{
    public interface ICDRAnalyticsService
    {
        Task<double> AverageCallDuration(string? callerId, DateTime? startDate, DateTime? endDate);
    }
}
