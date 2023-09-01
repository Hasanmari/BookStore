using BookStore.Models;

namespace BookStore.ViewModels
{
    public class BookAuthorViewModel
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookDescription { get; set; }
        public int AuthorId { get; set; }

        public List<Author>? Authors { get; set; }
    }
}