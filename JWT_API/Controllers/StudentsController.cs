using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_API.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly LoggingInterface _logging;
        public StudentsController( ApplicationContext db, LoggingInterface logging)
        {
            _db=db;
            _logging=logging;
        }
        [HttpPost]
        public ActionResult<Students> Create([FromBody] Students Obj)
        {
            var response = " ";
            if (Obj == null)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var data = _db.Student.FirstOrDefault(x => x.RollNumber == Obj.RollNumber);
            if (data != null)
            {
                response = _logging.Failure("Student Already exists", 400, null);
                return Content(response, "application/json");

            }
            Students student = new()
            {
                RollNumber = Obj.RollNumber,
                Semester = Obj.Semester,
                Name = Obj.Name,
                Phone = Obj.Phone,
                Email = Obj.Email,
                Address = Obj.Address,
                UserId = Obj.UserId,
            };
            _db.Student.Add(student);
            _db.SaveChanges();
            response = _logging.Success("Student Created Successfully", 201, student);
            return Content(response, "application/json");
        }

        [HttpGet]
        public ActionResult<Students> GetAll()
        {
            
            var data = _db.Student.ToList();
            var response = _logging.Success("Students Fetched Successfully", 200, data);
            return Content(response, "application/json");
        }

        [HttpGet("edit/{id}")]
        public ActionResult<Students> edit(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var data = _db.Student.FirstOrDefault(x => x.UserId==id);
            if (data!=null)
            {
                response = _logging.Success("Student Fetched Successfully", 200, data);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }

        [HttpGet("{id}")]
        public ActionResult<Students> Get(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var data = _db.Student.FirstOrDefault(x => x.StuId==id);
            if (data!=null)
            {
                response = _logging.Success("Student Fetched Successfully", 200, data);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpDelete("delete/{id}")]
        public ActionResult<Students> Delete(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var data = _db.Student.FirstOrDefault(x => x.StuId==id);
            if (data!=null)
            {
                _db.Student.Remove(data);
                _db.SaveChanges();
                response = _logging.Success("Student Deleted Successfully", 200, data);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpPut("update/{id}")]
        public ActionResult<Students> Update(int id, [FromBody] Students Obj)
        {
            var response = " ";
            if (id==0 || Obj==null)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var data = _db.Student.FirstOrDefault(x => x.UserId==id);
            if (data!=null)
            {
                data.RollNumber=Obj.RollNumber;
                data.Semester=Obj.Semester;
                data.Name=Obj.Name;
                data.Address=Obj.Address;
                data.Email=Obj.Email;
                data.Phone=Obj.Phone;
                _db.Student.Update(data);
                _db.SaveChanges();
                response = _logging.Success("Student Updated Successfully", 200, data);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }


    }
}
