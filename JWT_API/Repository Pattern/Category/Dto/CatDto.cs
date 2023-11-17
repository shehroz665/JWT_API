using JWT_API.Models;

namespace JWT_API.Repository_Pattern.Category.Dto
{
    public class CatDto
    {
        public int count { get; set; }
        public List<Categories> data { get; set; }
    }
}
