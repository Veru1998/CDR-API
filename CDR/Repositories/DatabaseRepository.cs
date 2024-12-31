using CDR.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<CallDetailRecord>> GetRecordsByCallerIdAsync(string callerId)
        {
            if (string.IsNullOrEmpty(callerId))
            {
                return [];
            }

            return await _dbContext.CDRs.Where(c => c.CallerId == callerId).ToListAsync();
        }

        public async Task<List<CallDetailRecord>> GetRecordsByRecipientAsync(string recipient) 
        {
            if (string.IsNullOrEmpty(recipient))
            {
                return [];
            }

            return await _dbContext.CDRs.Where(c => c.Recipient == recipient).ToListAsync();
        }

        public async Task<List<CallDetailRecord>> GetRecordsByDateAsync(DateTime? date)
        {
            if (date == null)
            {
                return [];
            }

            return await _dbContext.CDRs.Where(c => c.CallDate == date).ToListAsync();
        }

        public async Task<List<CallDetailRecord>> GetRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.CDRs
                .Where(record => record.CallDate >= startDate && record.CallDate <= endDate)
                .ToListAsync();
        }

        public async Task<List<CallDetailRecord>> GetXRecordsByDateAsync(int numberOfRecords, bool ascending, DateTime? startDate, DateTime? endDate) 
        {
            var records = await _dbContext.CDRs.ToListAsync();
            if (startDate != null && endDate != null)
            {
                records = await GetRecordsByDateRangeAsync((DateTime)startDate, (DateTime)endDate);
            }
            var result = ascending
                ? records.OrderBy(c => c.CallDate)
                : records.OrderByDescending(c => c.CallDate);

            return result.Take(numberOfRecords).ToList();
        }

        public async Task<List<CallDetailRecord>> GetAllCDRs()
        {
            return await _dbContext.CDRs.ToListAsync();
        }
    }
}
