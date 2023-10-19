using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JWT_API.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly LoggingInterface _logging;
        public BooksController(ApplicationContext db, LoggingInterface logging)
        {
            _db=db;
            _logging=logging;
        }
        [HttpGet]
        public ActionResult<IEnumerable<BookDto>> getBooks()
        {
            //using raw query
            var books = _db.Bookdto
                .FromSqlRaw("SELECT Book.BookId, Book.Title, Book.BookAuthId, Book.BookCatId, Book.Isbn, Book.ActualQuantity, Book.AvailableQuantity, Book.Price, Book.Status, " +
                            "Category.CatId, Category.CatName, " +
                            "Author.AuthId, Author.AuthName " +
                            "FROM Book " +
                            "JOIN Category ON Book.BookCatId = Category.CatId " +
                            "JOIN Author ON Book.BookAuthId = Author.AuthId " +
                            "WHERE Book.Status IN (0, 1)")
                            .ToList();
            //using stored procedure
            //var books = _db.Bookdto.FromSqlRaw("exec getBooks").ToList();
            var response = _logging.Success("Books Fetched Successfully", 200, books);
            return Content(response, "application/json");
        }


        [HttpPost]
        public ActionResult<Books> createBook([FromBody] Books booksObj)
        {
            var response = "";
            if (booksObj == null)
            {
                    response = _logging.Failure("Bad Request", 400, null);
                    return Content(response, "application/json");
            }
            var book= _db.Book.FirstOrDefault(x=>x.Title == booksObj.Title);
            if(book != null)
            {
                response = _logging.Failure("Book Already exists", 400, null);
                return Content(response, "application/json");
            }
            Books books = new Books()
            {
                  Title=booksObj.Title,
                  BookCatId=booksObj.BookCatId,
                  BookAuthId=booksObj.BookCatId,
                  Isbn=booksObj.Isbn,
                  ActualQuantity=booksObj.ActualQuantity,
                  AvailableQuantity=booksObj.AvailableQuantity,
                  Price=booksObj.Price,
                  Status=1,
            };
            _db.Book.Add(books);
            _db.SaveChanges();
            response = _logging.Success("Book Created Successfully", 201, books);
            return Content(response, "application/json");

        }

    }
}
