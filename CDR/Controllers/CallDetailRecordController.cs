using CDR.Models;
using Microsoft.AspNetCore.Mvc;

namespace CDR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallDetailRecordController : ControllerBase
    {
        private readonly CallDetailRecordDBContext _dbContext;
        public CallDetailRecordController(CallDetailRecordDBContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
