using Azure;
using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<Authors> CreateAuthor([FromBody] Authors authorsObj)
        {
            var response = " ";
            if(authorsObj == null)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var authorData= _db.Author.FirstOrDefault(x=>x.AuthName == authorsObj.AuthName);
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
        public ActionResult<Authors> GetAuthors()
        {
            var response = " ";
            var authorData = _db.Author.Where(x=>x.Status==1 | x.Status==0).ToList();
            response = _logging.Success("Authors Fetched Successfully", 200, authorData);
            return Content(response, "application/json");
        }

        [HttpGet("{id}")]
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
