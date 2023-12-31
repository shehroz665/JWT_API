﻿using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
       
        public ActionResult<IEnumerable<BookDto>> getBooks(int from=1,int to=10,string searchTerm="")
        {
            //using raw query
            var fromParam = new SqlParameter("fromParam", from-1);
            var toParam = new SqlParameter("toParam", to);
            var searchParam = new SqlParameter("searchTerm", "%" +searchTerm+"%");
            var sqlQuery = "SELECT Book.BookId, Book.Title, Book.BookAuthId, Book.BookCatId, Book.Isbn, Book.ActualQuantity, Book.AvailableQuantity, Book.Price, Book.Status, " +
                            "Category.CatId, Category.CatName, " +
                            "Author.AuthId, Author.AuthName " +
                            "FROM Book " +
                            "JOIN Category ON Book.BookCatId = Category.CatId " +
                            "JOIN Author ON Book.BookAuthId = Author.AuthId " +
                            "WHERE Book.Status IN (0, 1)";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sqlQuery+="AND (Book.Title LIKE @searchTerm OR Category.CatName LIKE @searchTerm OR Author.AuthName LIKE @searchTerm OR Book.Isbn LIKE @searchTerm) ";
                var count = _db.Bookdto.FromSqlRaw(sqlQuery,searchParam).Count();
                sqlQuery+="ORDER BY Book.BookId OFFSET @fromParam ROWS FETCH NEXT @toParam ROWS ONLY ";
                var books = _db.Bookdto.FromSqlRaw(sqlQuery, fromParam, toParam,searchParam).ToList();
                //using stored procedure
                //var books = _db.Bookdto.FromSqlRaw("exec getBooks").ToList();
                var data = new
                {
                    data = books,
                    count = count,
                };
                var response = _logging.Success("Books Fetched Successfully", 200, data);
                return Content(response, "application/json");
            }
            else
            {
               var count = _db.Bookdto.FromSqlRaw(sqlQuery).Count();
                sqlQuery+="ORDER BY Book.BookId OFFSET @fromParam ROWS FETCH NEXT @toParam ROWS ONLY ";
                var books = _db.Bookdto.FromSqlRaw(sqlQuery, fromParam, toParam).ToList();
                //using stored procedure
                //var books = _db.Bookdto.FromSqlRaw("exec getBooks").ToList();
                var data = new
                {
                    data = books,
                    count = count,
                };
                var response = _logging.Success("Books Fetched Successfully", 200, data);
                return Content(response, "application/json");
            }

        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "Admin")]
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
        [HttpPut("changeStatus/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Authors> ChangeStatus(int id, [FromBody] StatusDto statusDto)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var data = _db.Book.FirstOrDefault(x => x.BookId==id);
            if (data!=null)
            {
                data.Status= statusDto.status;
                _db.Book.Update(data);
                _db.SaveChanges();
                response = _logging.Success("Book Status Updated Successfully", 200, data);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }

        [HttpGet("dropdown")]
        [Authorize]
        public ActionResult<IEnumerable<BookDto>> Getdropdown()
        {
            //using raw query
            var books = _db.Bookdto
                .FromSqlRaw("SELECT Book.BookId, Book.Title, Book.BookAuthId, Book.BookCatId, Book.Isbn, Book.ActualQuantity, Book.AvailableQuantity, Book.Price, Book.Status, " +
                            "Category.CatId, Category.CatName, " +
                            "Author.AuthId, Author.AuthName " +
                            "FROM Book " +
                            "JOIN Category ON Book.BookCatId = Category.CatId " +
                            "JOIN Author ON Book.BookAuthId = Author.AuthId " +
                            "WHERE Book.Status = 1")
                            .ToList();
            //using stored procedure
            //var books = _db.Bookdto.FromSqlRaw("exec getBooks").ToList();
            var response = _logging.Success("Books Fetched Successfully", 200, books);
            return Content(response, "application/json");
        }
    }
}
