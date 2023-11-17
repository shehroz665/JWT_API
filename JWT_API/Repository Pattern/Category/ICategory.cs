using JWT_API.Models;
using JWT_API.Repository_Pattern.Category.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace JWT_API.Repository_Pattern.Category
{
    public interface ICategory
    {
        CatDto GetAllCategories(int from, int to, string searchTerm);
        IEnumerable GetBookCatData(int id);
        Categories GetCategory(int id);
        
        Categories DeleteCategory(int id);

        Categories UpdateCategory(int id, Categories category);

        Categories ChangeStatusCategory(int id, StatusDto statusDto);

        Categories CreateCategory(Categories categoriesObj);
    }
}
