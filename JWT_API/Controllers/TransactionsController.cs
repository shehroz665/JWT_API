using JWT_API.Data;
using JWT_API.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationContext _db;
        private readonly LoggingInterface _logging;
        public TransactionsController(IConfiguration configuration, ApplicationContext db, LoggingInterface logging)
        {
            _configuration=configuration;
            _db=db;
            _logging=logging;
        }
    }
}
