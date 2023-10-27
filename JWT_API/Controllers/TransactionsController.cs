using Azure;
using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata;
using System.Transactions;

namespace JWT_API.Controllers
{
    [Route("api/transaction")]
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
        [HttpPost]
        [Authorize]
        public ActionResult<Transactions> CreateTransaction([FromBody] Transactions transObj)
        {
            var response = " ";
            if (transObj == null)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var student = _db.Student.FirstOrDefault(x=>x.UserId == transObj.UserId);
            if(student == null)
            {
                response = _logging.Failure("Before applying for Book,Please add your details on Profile", 404, null);
                return Content(response, "application/json");
            }
            Transactions transaction = new()
            {
                TransBookId= transObj.TransBookId,
                TransStuId= student.StuId,
                BorrowedDate=transObj.BorrowedDate,
                DueDate=transObj.DueDate,
                ReturnedDate=transObj.ReturnedDate,
                Status=transObj.Status,
                UserId=transObj.UserId,
            };
            _db.Transaction.Add(transaction);
            _db.SaveChanges();
            response = _logging.Success("Please wait until the admin approve your request", 201, transaction);
            return Content(response, "application/json");
        }
        [HttpGet]
   
        public ActionResult<TransactionsDto> GetAll(string searchTerm = "",int from=1,int to=10,int status=0)
        {
            var fromParam = new SqlParameter("fromParam", from-1);
            var toParam= new SqlParameter("to", to);
            var searchParam = new SqlParameter("SearchTerm", "%" + searchTerm + "%");
            var statusParam = new SqlParameter("statusParam", status);
            var sqlQuery = "SELECT [Book].Title, [User].Email, [Transaction].TransBookId, [Transaction].TransID, [Transaction].TransStuId, [Transaction].BorrowedDate, [Transaction].DueDate, [Transaction].ReturnedDate, " +
                    "[Transaction].Status, [Transaction].UserId " +
                    "FROM [Book] " +
                    "INNER JOIN [Transaction] " +
                    "ON [Book].BookId = [Transaction].TransBookId " +
                    "INNER JOIN [User] " +
                    "ON [Transaction].UserId = [User].Id";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                
                sqlQuery += " WHERE [Book].Title LIKE @SearchTerm OR [User].Email LIKE @SearchTerm";
               var count = _db.TransactionDto.FromSqlRaw(sqlQuery,searchParam).Count();
                sqlQuery += $" ORDER BY [Transaction].TransID OFFSET @fromParam ROWS FETCH NEXT @to ROWS ONLY";
                var data = _db.TransactionDto.FromSqlRaw(sqlQuery, searchParam,fromParam,toParam).ToList();
                var res = new
                {
                    Data= data,
                    count=count
                };
                var responses = _logging.Success("Transaction Fetched Successfully", 200, res);
                return Content(responses, "application/json");
            }
            else
            {
                
                if (status!=0)
                {
                    sqlQuery += " WHERE [Transaction].Status = @statusParam ";
                    var count = _db.TransactionDto.FromSqlRaw(sqlQuery, statusParam).Count();
                    sqlQuery += $" ORDER BY [Transaction].TransID OFFSET @fromParam ROWS FETCH NEXT @to ROWS ONLY";
                    var data = _db.TransactionDto.FromSqlRaw(sqlQuery, fromParam, toParam,statusParam).ToList();
                    var res = new
                    {
                        Data = data,
                        count = count
                    };
                    var response = _logging.Success("Transaction Fetched Successfully", 200, res);
                    return Content(response, "application/json");
                }
                else
                {
                    var count = _db.TransactionDto.FromSqlRaw(sqlQuery).Count();
                    sqlQuery += $" ORDER BY [Transaction].TransID OFFSET @fromParam ROWS FETCH NEXT @to ROWS ONLY";
                    var data = _db.TransactionDto.FromSqlRaw(sqlQuery, fromParam, toParam).ToList();
                    var res = new
                    {
                        Data = data,
                        count = count
                    };
                    var response = _logging.Success("Transaction Fetched Successfully", 200, res);
                    return Content(response, "application/json");
                }
            }

        }
        [HttpGet("{id}")]


        public ActionResult<Transactions> GetById(int id,string searchTerm="",int from = 1, int to = 10,int status=0)
        {
            //this api is for student,id=UserId
            var response = " ";
            var count = 0;
            var UserId = new SqlParameter("UserId", id);
            var fromParam = new SqlParameter("fromParam", from-1);
            var toParam= new SqlParameter("toParam", to);
            var SearchTerm = new SqlParameter("SearchTerm", "%" + searchTerm + "%");
            var statusParam = new SqlParameter("statusParam", status);
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var sqlQuery = "SELECT [Book].Title, [User].Email, [Transaction].TransBookId, [Transaction].TransID, [Transaction].TransStuId, [Transaction].BorrowedDate, [Transaction].DueDate, [Transaction].ReturnedDate, " +
                                                  "[Transaction].Status, [Transaction].UserId " +
                                                  "FROM [Book] " +
                                                  "INNER JOIN [Transaction] " +
                                                  "ON [Book].BookId = [Transaction].TransBookId "+
                                                  "INNER JOIN [User] " +
                                                  "ON [Transaction].UserId = [User].Id "+
                                                  "WHERE [Transaction].UserId= @UserId ";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                sqlQuery += " AND ([User].Email LIKE @SearchTerm OR [Book].Title LIKE @SearchTerm) ";
                count = _db.Transaction.FromSqlRaw(sqlQuery, UserId, SearchTerm).Count();
                sqlQuery += $" ORDER BY [Transaction].TransID OFFSET @fromParam ROWS FETCH NEXT @toParam ROWS ONLY ";
                var data = _db.TransactionDto.FromSqlRaw(sqlQuery, UserId, SearchTerm, fromParam, toParam).ToList();
                if (data!=null)
                {
                    var obj = new
                    {
                        Data = data,
                        count = count,
                    };
                    response = _logging.Success("Student Transaction Fetched Successfully", 200, obj);
                    return Content(response, "application/json");
                }
                response = _logging.Failure("Not found", 404, null);
                return Content(response, "application/json");
            }
            else
            {
                if (status!=0)
                {
                    
                    sqlQuery+="AND [Transaction].Status = @statusParam";
                    sqlQuery += $" ORDER BY [Transaction].TransID OFFSET @fromParam ROWS FETCH NEXT @toParam ROWS ONLY ";
                    count = _db.Transaction.FromSqlRaw(sqlQuery, UserId, fromParam, toParam, statusParam).Count();
                    var data = _db.TransactionDto.FromSqlRaw(sqlQuery, UserId, fromParam, toParam,statusParam).ToList();
                    if (data!=null)
                    {
                        var obj = new
                        {
                            Data = data,
                            count = count,
                        };
                        response = _logging.Success("Student Transaction Fetched Successfully", 200, obj);
                        return Content(response, "application/json");
                    }
                    response = _logging.Failure("Not found", 404, null);
                    return Content(response, "application/json");
                }
                else
                {
                    count = _db.Transaction.FromSqlRaw(sqlQuery, UserId).Count();
                    sqlQuery += $" ORDER BY [Transaction].TransID OFFSET @fromParam ROWS FETCH NEXT @toParam ROWS ONLY ";
                    var data = _db.TransactionDto.FromSqlRaw(sqlQuery, UserId, fromParam, toParam).ToList();
                    if (data!=null)
                    {
                        var obj = new
                        {
                            Data = data,
                            count = count,
                        };
                        response = _logging.Success("Student Transaction Fetched Successfully", 200, obj);
                        return Content(response, "application/json");
                    }
                    response = _logging.Failure("Not found", 404, null);
                    return Content(response, "application/json");
                }

            }
        }

        [HttpPut("update/{id}")]
        public ActionResult<Transactions> UpdateStatus(int id,[FromBody] Transactions transactionsObj) 
        {
            var response = " ";
            var bookStatus = "";
            if (transactionsObj==null || id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var transaction= _db.Transaction.FirstOrDefault(x=>x.TransID==id);
            var book = _db.Book.FirstOrDefault(x => x.BookId==transactionsObj.TransBookId);
            if (transaction!=null && book!=null)
            {
                if (transactionsObj.Status==2)  //approved
                {           
                    transaction.Status = 2;
                    book.AvailableQuantity = book.AvailableQuantity-1;
                    bookStatus= "approved";
                }
                else if(transactionsObj.Status==3)  //returned
                {
                    transaction.Status = 3;
                    transaction.ReturnedDate=DateTime.Now;
                    book.AvailableQuantity= book.AvailableQuantity+1;
                    bookStatus= "returned";
                }
                else    //rejected
                {
                    transaction.Status=4;
                    bookStatus= "rejected";
                }
                _db.Transaction.Update(transaction);
                _db.Book.Update(book);
                _db.SaveChanges();
                response = _logging.Success("Book is "+bookStatus, 200, transaction);
                return Content(response, "application/json");
            }
            else
            {
                response = _logging.Failure("Not found", 404, null);
                return Content(response, "application/json");
            }
      
        }
    }
}
