namespace CDR.Services
{
    public interface ICsvProcessingService
    {
        Task ProcessCsvFile(IFormFile file);
    }
}
