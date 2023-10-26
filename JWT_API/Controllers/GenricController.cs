using Azure;
using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace JWT_API.Controllers
{
    [Route("api/generic")]
    [ApiController]
    public class GenricController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly LoggingInterface _logging;
        public GenricController(ApplicationContext db, LoggingInterface logging)
        {
            _db=db;
            _logging=logging;
        }
        [HttpGet("dropdown")]
        public ActionResult Get()
        {
            var authors=_db.Author.Where(x=> x.Status==1).ToList();
            var categories= _db.Category.Where(x => x.Status==1).ToList();
            var data = new
            {
                category = categories,
                authors = authors,
            };
           var response = _logging.Success("Dropdown Fetched Successfully", 200, data);
            return Content(response, "application/json");

        }
        [HttpGet("bookTitle")]
        public IActionResult GetBookTitle()
        {
            var data = _db.Transaction
                  .Join(_db.Book,
                      trans => trans.TransBookId,
                      book => book.BookId,
                      (trans, book) => new
                      {
                          TransBookId=trans.TransBookId,
                          Title=book.Title,
                      }
                    )
                  .Distinct()
                    .ToList();
            var response = _logging.Success("Books Title Dropdown Fetched Successfully", 200, data);
            return Content(response, "application/json");

        }
    }
}
