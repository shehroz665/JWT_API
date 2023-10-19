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

            _db.Book.Add(booksObj);
            _db.SaveChanges();
            response = _logging.Success("Book Created Successfully", 201, booksObj);
            return Content(response, "application/json");

        }
        [HttpGet("{id}")]
        public ActionResult<Books> getBook(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var bookData = _db.Book.FirstOrDefault(x => x.BookId==id);
            if (bookData!=null)
            {
                response = _logging.Success("Book Fetched Successfully", 200, bookData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");

        }
        [HttpPut("delete/{id}")]
        public ActionResult<Books> DeleteBook(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var bookData = _db.Book.FirstOrDefault(x => x.BookId==id);
            if (bookData!=null)
            {
                bookData.Status=2;
                _db.Book.Update(bookData);
                _db.SaveChanges();
                response = _logging.Success("Book Deleted Successfully", 200, bookData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpPut("update/{id}")]
        public ActionResult<Books> UpdateBook(int id, [FromBody] Books booksObj)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var bookData = _db.Book.FirstOrDefault(x => x.BookId==id);
            if (bookData==null)
            {
                response = _logging.Failure("Not found", 404, null);
                return Content(response, "application/json");
            }
            bookData.Title = booksObj.Title;
            bookData.BookCatId = booksObj.BookCatId;
            bookData.BookAuthId = booksObj.BookAuthId;
            bookData.ActualQuantity = booksObj.ActualQuantity;
            bookData.AvailableQuantity = booksObj.AvailableQuantity;
            bookData.Price = booksObj.Price;
            bookData.Isbn = booksObj.Isbn;
            _db.Book.Update(bookData);
            _db.SaveChanges();
            response = _logging.Success("Book Updated Successfully", 200, booksObj);
            return Content(response, "application/json");

        }

    }
}
