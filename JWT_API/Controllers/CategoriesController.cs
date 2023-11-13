using Azure;
using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            var categoryData = _db.Category.FirstOrDefault(x => x.CatName == categoriesObj.CatName && x.Status != 2);
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

        public ActionResult<CategoriesDto> GetCategories(int from = 1, int to = 10, string searchTerm = "")
        {
            var response = " ";
            var query = _db.Category
                .Where(c => EF.Functions.Like(c.CatName, "%" + searchTerm + "%") && (c.Status==0 || c.Status==1))
                .OrderBy(c => c.CatId)
                .Select(c => new CategoriesDto
                {
                    CatId = c.CatId,
                    CatName = c.CatName,
                    Status = c.Status,
                    Books = _db.Book
                        .Where(b => b.BookCatId == c.CatId)
                        .Select(b => new BookDto
                        {
                            BookId = b.BookId,
                            Title = b.Title,
                            BookCatId=b.BookCatId,
                            BookAuthId=b.BookAuthId,
                            Isbn=b.Isbn,
                            ActualQuantity=b.ActualQuantity,
                            AvailableQuantity=b.AvailableQuantity,
                            Price=b.Price,
                            Status=b.Status,
                            CatId=b.BookCatId,
                            CatName=c.CatName,
                            AuthId=b.BookAuthId,
                            AuthName=_db.Author.FirstOrDefault(x=>x.AuthId==b.BookAuthId).AuthName,
                        })
                        .ToList()
                });
            var count = query.Count();
            var result = query.Skip(from-1)
                .Take(to)
                .ToList();
            if (result!=null)
            {
                var res = new
                {
                    data = result,
                    count = count
                };
                response = _logging.Success("Category Fetched Successfully", 200, res);
                return Content(response, "application/json");
            }
            else
            {
                response = _logging.Failure("Not found", 404, null);
                return Content(response, "application/json");
            }

        }
        [HttpGet("getAllCat")]
        public IActionResult GetAllCat(int from = 1, int to = 10, string searchTerm = "")
        {
            try
            {
                var query = from category in _db.Category
                            where category.Status != 2 && (string.IsNullOrEmpty(searchTerm) || category.CatName.Contains(searchTerm))
                            select category;
                var result = query.Skip(from - 1).Take(to).ToList();
                var count = query.Count();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    count = result.Count();
                }
                var res = new
                {
                    data = result,
                    count = count
                };
                var response = _logging.Success("Category Fetched Successfully", 200, res);
                return Content(response, "application/json");
            }
            catch (Exception ex)
            {
                var response = _logging.Failure(ex.Message, 500, null);
                return Content(response, "application/json");
            }

        }
        [HttpGet("getAllCat/{id}")]
        public IActionResult GetCat(int id)
        {
            try
            {
                var response = "";
                if (id == null || id<=0)
                {
                     response = _logging.Failure("Bad Request", 400, null);
                    return Content(response, "application/json");
                }
                var query = from category in _db.Category
                            where category.CatId == id
                            join book in _db.Book
                            on category.CatId equals book.BookCatId into BookData
                            select new
                            {
                                CategoryId = category.CatId,
                                CategoryName = category.CatName,
                                Books = BookData.Select(b => new
                                {
                                    BookId = b.BookId,
                                    Title = b.Title,
                                    BookCatId = b.BookCatId,
                                    BookAuthId = b.BookAuthId,
                                    Isbn = b.Isbn,
                                    ActualQuantity = b.ActualQuantity,
                                    AvailableQuantity = b.AvailableQuantity,
                                    Price = b.Price,
                                    Status = b.Status
                                })
                            };
                var result = query.ToList();
                var res = new
                {
                    data = result,

                };
                response = _logging.Success("Category Fetched Successfully", 200, result);
                return Content(response, "application/json");
            }
            catch (Exception ex)
            {
                var response = _logging.Failure(ex.Message, 500, null);
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
