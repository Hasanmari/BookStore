using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a title")]
        [StringLength(20, MinimumLength = 5)]
        public string? Title { get; set; }

        public string? Description { get; set; }

       
        public Author Author { get; set; }
    }
}