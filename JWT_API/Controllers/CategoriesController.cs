using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
        
        public ActionResult<CategoriesDto> GetCategories(int from=1,int to=10, string searchTerm = "")
        {
            var response = " ";
            var fromParam = new SqlParameter("fromParam", from - 1);
            var toParam = new SqlParameter("toParam", to);
            var searchParam = new SqlParameter("searchParam", "%" + searchTerm + "%");
            var data = _db.CategoryDto.FromSqlRaw("EXEC SearchCategoriesWithBooks @searchParam, @fromParam, @toParam", searchParam,fromParam,toParam).ToList();
            var count = data.Count();
            var DbCount=_db.Category.Where(x=>x.Status== 0 | x.Status==1).Count();
            if (data!=null)
            {
                if (count>9)
                {
                    count=DbCount;
                }
                var obj = new
                {
                    data = data,
                    count = count,
                };
                response = _logging.Success("Category Fetched Successfully", 200, obj);
                return Content(response, "application/json");
            }
            else
            {
                response = _logging.Failure("Not found", 404, null);
                return Content(response, "application/json");
            }

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
