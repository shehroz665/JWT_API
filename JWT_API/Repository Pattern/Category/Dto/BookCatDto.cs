using JWT_API.Models;

namespace JWT_API.Repository_Pattern.Category.Dto
{
    public class BookCatDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public List<BookCatDto> Books { get; set; }
    }
    public class CatDtos
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public List<BookCatDto> Books { get; set; }
    }
    public class BookDtos
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int BookCatId { get; set; }
        public int BookAuthId { get; set; }
        public string Isbn { get; set; }
        public int ActualQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public string BookAuthName { get; set; }
        public string BookCatName { get; set; }
    }
}
