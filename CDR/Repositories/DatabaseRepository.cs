using CDR.Models;

namespace CDR.Repositories
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly CallDetailRecordDBContext _dbContext;

        public DatabaseRepository(CallDetailRecordDBContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        public async Task SaveRecordsAsync(IEnumerable<CallDetailRecord> records)
        {
            await _dbContext.CDRs.AddRangeAsync(records);
            await _dbContext.SaveChangesAsync();
        }
    }
}
