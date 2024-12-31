namespace CDR.Services
{
    public interface ICDRAnalyticsService
    {
        Task<double> AverageCallDuration(string? callerId, DateTime? startDate, DateTime? endDate);
        Task<int> CallVolume(DateTime startDate, DateTime endDate);
    }
}
