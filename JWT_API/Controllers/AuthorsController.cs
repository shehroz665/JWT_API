using Azure;
using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JWT_API.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly LoggingInterface _logging;
        public AuthorsController(ApplicationContext db, LoggingInterface logging)
        {
            _db=db;
            _logging=logging;
        }


        [HttpPost]
        [Authorize(Policy = "Admin")]
        public ActionResult<Authors> CreateAuthor([FromBody] Authors authorsObj)
        {
            var response = " ";
            if(authorsObj == null)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var authorData= _db.Author.FirstOrDefault(x=>x.AuthName == authorsObj.AuthName && x.Status!=2);
            if(authorData != null)
            {
                response = _logging.Failure("Author Already exists", 400, null);
                return Content(response, "application/json");

            }
            Authors author = new()
            {
                AuthName = authorsObj.AuthName,
                Status = authorsObj.Status,
            };
            _db.Author.Add(author);
            _db.SaveChanges();
            response = _logging.Success("Author Created Successfully", 201, author);
            return Content(response, "application/json");
        }

        [HttpGet]

        public ActionResult<AuthorDto> GetAuthors(int from = 1, int to = 10, string searchTerm = "")
        {
            var response = " ";
                var fromParam = new SqlParameter("fromParam", from - 1);
                var toParam = new SqlParameter("toParam", to);
                var searchParam = new SqlParameter("SearchTerm", "%" + searchTerm + "%");
            var sqlQuery = "SELECT [Author].AuthId, [Author].AuthName, [Author].Status, "+
                                "ISNULL(STRING_AGG([Book].Title, ', '), '') AS Titles " +
                                "FROM [Author] " +
                                "LEFT JOIN [Book] ON [Author].AuthId = [Book].BookAuthId " +
                                "WHERE [Author].Status IN (0,1) " +
                                "GROUP BY [Author].AuthId, [Author].AuthName, [Author].Status ";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sqlQuery += "HAVING [Author].AuthName LIKE @SearchTerm ";
                var count = _db.AuthorDto.FromSqlRaw(sqlQuery, searchParam).Count();
                sqlQuery +="ORDER BY [Author].AuthId OFFSET @fromParam ROWS FETCH NEXT @toParam ROWS ONLY";
                var authorData = _db.AuthorDto.FromSqlRaw(sqlQuery, searchParam, fromParam, toParam).ToList();


                if (authorData != null)
                {
   
                    var obj = new
                    {
                        data = authorData,
                        count = count,
                    };
                    response = _logging.Success("Authors Fetched Successfully", 200, obj);
                    return Content(response, "application/json");
                }
                else
                {
                    response = _logging.Failure("Not found", 404, null);
                    return Content(response, "application/json");
                }
            }
            else
            {
                var count = _db.AuthorDto.FromSqlRaw(sqlQuery).Count();
                sqlQuery += $"ORDER BY [Author].AuthId OFFSET @fromParam ROWS FETCH NEXT @toParam ROWS ONLY ";
                var authorData = _db.AuthorDto.FromSqlRaw(sqlQuery, fromParam, toParam).ToList();
                if (authorData != null)
                {
                    var obj = new
                    {
                        data = authorData,
                        count = count,
                    };
                    response = _logging.Success("Authors Fetched Successfully", 200, obj);
                    return Content(response, "application/json");
                }
                else
                {
                    response = _logging.Failure("Not found", 404, null);
                    return Content(response, "application/json");
                }
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Authors> GetAuthor(int id)
        {
            var response = " ";
            if(id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var authorData = _db.Author.FirstOrDefault(x => x.AuthId==id);
            if (authorData!=null)
            {
                response = _logging.Success("Authors Fetched Successfully", 200, authorData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpPut("delete/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Authors> DeleteAuthor(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var authorData = _db.Author.FirstOrDefault(x => x.AuthId==id);
            if (authorData!=null)
            {
                authorData.Status=2;
                _db.Author.Update(authorData);
                _db.SaveChanges();
                response = _logging.Success("Author Deleted Successfully", 200, authorData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpPut("update/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Authors> UpdateAuthor(int id, [FromBody] Authors authorsObj)
        {
            var response = " ";
            if (id==0 || authorsObj==null)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var authorData = _db.Author.FirstOrDefault(x => x.AuthId==id);
            if (authorData!=null)
            {
                authorData.AuthName=authorsObj.AuthName;
                authorData.Status=authorsObj.Status;
                _db.Author.Update(authorData);
                _db.SaveChanges();
                response = _logging.Success("Author Updated Successfully", 200, authorData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpPut("changeStatus/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Authors> ChangeStatusAuthor(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var authorData = _db.Author.FirstOrDefault(x => x.AuthId==id);
            if (authorData!=null)
            {
                if(authorData.Status==0)
                {
                    authorData.Status=1;
                }
                else
                {
                    authorData.Status=0;
                }
                _db.Author.Update(authorData);
                _db.SaveChanges();
                response = _logging.Success("Author Status Updated Successfully", 200, authorData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }

    }
}
