using CDR.Models;

namespace CDR.Repositories
{
    public interface IDatabaseRepository
    {
        Task SaveRecordsAsync(IEnumerable<CallDetailRecord> records);
        Task<List<CallDetailRecord>> GetRecordsByCallerIdAsync(string callerId);
        Task<List<CallDetailRecord>> GetRecordsByRecipientAsync(string recipient);
        Task<List<CallDetailRecord>> GetRecordsByDateAsync(DateTime? date);
        Task<List<CallDetailRecord>> GetRecordsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<CallDetailRecord>> GetXRecordsByDateAsync(int numberOfRecords, bool ascending, DateTime? startDate, DateTime? endDate);
    }
}
