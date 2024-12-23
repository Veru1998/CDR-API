using Microsoft.EntityFrameworkCore;

namespace CDR.Models
{
    public partial class CallDetailRecordDBContext: DbContext
    {
        public CallDetailRecordDBContext(DbContextOptions<CallDetailRecordDBContext> options) : base(options) { }
        public virtual DbSet<CallDetailRecord> CDRs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CallDetailRecord>(entity =>
            {
                entity.HasKey(k => k.Reference);
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
