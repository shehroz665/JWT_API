using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly LoggingInterface _logging;
        public CategoriesController( ApplicationContext db, LoggingInterface logging)
        {
            _db=db;
            _logging=logging;
        }
        [HttpPost]
        [Authorize(Policy = "Admin")]
        public ActionResult<Categories> CreateCategory([FromBody] Categories categoriesObj)
        {
            var response = " ";
            if (categoriesObj == null)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var categoryData = _db.Category.FirstOrDefault(x => x.CatName == categoriesObj.CatName);
            if (categoryData != null)
            {
                response = _logging.Failure("Category Already exists", 400, null);
                return Content(response, "application/json");

            }
            Categories category = new()
            {
                CatName = categoriesObj.CatName,
                Status = categoriesObj.Status,
            };
            _db.Category.Add(category);
            _db.SaveChanges();
            response = _logging.Success("Category Created Successfully", 201, category);
            return Content(response, "application/json");
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public ActionResult<Categories> GetCategories()
        {
            var response = " ";
            var categoryData = _db.Category.Where(x => x.Status==1 |x.Status==0).ToList();
            response = _logging.Success("Categories Fetched Successfully", 200, categoryData);
            return Content(response, "application/json");
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Categories> GetCategory(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var categoryData = _db.Category.FirstOrDefault(x => x.CatId==id);
            if (categoryData!=null)
            {
                response = _logging.Success("Category Fetched Successfully", 200, categoryData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpPut("delete/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Categories> DeleteCategory(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var categoryData = _db.Category.FirstOrDefault(x => x.CatId==id);
            if (categoryData!=null)
            {
                categoryData.Status=2;
                _db.Category.Update(categoryData);
                _db.SaveChanges();
                response = _logging.Success("Category Deleted Successfully", 200, categoryData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpPut("update/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Categories> UpdateCategory(int id, [FromBody] Categories categoryObj)
        {
            var response = " ";
            if (id==0 || categoryObj==null)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var categoryData = _db.Category.FirstOrDefault(x => x.CatId==id);
            if (categoryData!=null)
            {
                categoryData.CatName=categoryObj.CatName;
                categoryData.Status=categoryObj.Status;
                _db.Category.Update(categoryData);
                _db.SaveChanges();
                response = _logging.Success("Category Updated Successfully", 200, categoryData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }
        [HttpPut("changeStatus/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<Categories> ChangeStatusCategory(int id)
        {
            var response = " ";
            if (id==0)
            {
                response = _logging.Failure("Bad Request", 400, null);
                return Content(response, "application/json");
            }
            var categoryData = _db.Category.FirstOrDefault(x => x.CatId==id);
            if (categoryData!=null)
            {
                if (categoryData.Status==0)
                {
                    categoryData.Status=1;
                }
                else
                {
                    categoryData.Status=0;
                }
                _db.Category.Update(categoryData);
                _db.SaveChanges();
                response = _logging.Success("category Status Updated Successfully", 200, categoryData);
                return Content(response, "application/json");
            }
            response = _logging.Failure("Not found", 404, null);
            return Content(response, "application/json");
        }

    }
}
