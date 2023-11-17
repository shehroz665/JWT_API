using Azure;
using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using JWT_API.Repository_Pattern.Category.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Linq;

namespace JWT_API.Repository_Pattern.Category
{
    public class CategoryRepository:ICategory
    {
        private readonly ApplicationContext _db;
        public CategoryRepository(ApplicationContext db)
        {
            _db = db;
        }
       public CatDto GetAllCategories(int from, int to, string searchTerm)
        {

            var query = from category in _db.Category
                        where category.Status != 2 && (string.IsNullOrEmpty(searchTerm) || category.CatName.Contains(searchTerm))
                        select category;

            var result = query.Skip(from-1).Take(10).ToList();
            var count= query.Count();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                count = result.Count();
                
            }
            var res = new CatDto
            {
                data = result,
                count = count,
            };
            return res;

        }
       public IEnumerable GetBookCatData(int id)
        {
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
                                Status = b.Status,
                                BookAuthName = _db.Author.FirstOrDefault(x => x.AuthId == b.BookAuthId).AuthName,
                                BookCatName = category.CatName,
                            })
                        };
            var result = query.ToList();
            return result;
        }

       public Categories GetCategory(int id)
        {
            var categoryData = _db.Category.FirstOrDefault(x => x.CatId == id && x.Status!=2);
            return categoryData;
        }

        public Categories DeleteCategory(int id)
        {
            var categoryData = _db.Category.FirstOrDefault(x => x.CatId == id);
            if (categoryData != null)
            {
                categoryData.Status = 2;
                _db.Category.Update(categoryData);
                _db.SaveChanges();
              
            }
            return categoryData;
        }

        public Categories UpdateCategory(int id, Categories category)
        {
            var categoryData = _db.Category.FirstOrDefault(x => x.CatId == id);
            if (categoryData != null)
            {
                categoryData.CatName = category.CatName;
                categoryData.Status = category.Status;
                _db.Category.Update(categoryData);
                _db.SaveChanges();
            }
            return categoryData;
        }

        public Categories ChangeStatusCategory(int id, StatusDto statusDto)
        {
            var categoryData = _db.Category.FirstOrDefault(x => x.CatId == id);
            if (categoryData != null)
            {


                categoryData.Status = statusDto.status;

                _db.Category.Update(categoryData);
                _db.SaveChanges();
            }
            return categoryData;
        }
        public Categories CreateCategory(Categories categoriesObj)
        {

            var categoryData = _db.Category.FirstOrDefault(x => x.CatName == categoriesObj.CatName && x.Status != 2);
            if (categoryData != null)
            {
                var res = new Categories()
                {
                    CatId = -1,
                    CatName = "",
                    Status = -1
                };
                return res;

            }
            Categories category = new()
            {
                CatName = categoriesObj.CatName,
                Status = categoriesObj.Status,
            };
            _db.Category.Add(category);
            _db.SaveChanges();
            return category;
        }
    }

}
