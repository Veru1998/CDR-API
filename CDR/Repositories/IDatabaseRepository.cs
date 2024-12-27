using CDR.Models;

namespace CDR.Repositories
{
    public interface IDatabaseRepository
    {
        Task SaveRecordsAsync(IEnumerable<CallDetailRecord> records);
    }
}
