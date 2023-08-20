using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a title")]
        [StringLength(20, MinimumLength = 5)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [NotMapped]
        public IFormFile? BookImage { get; set; }

        [NotMapped]
        public IFormFile? BookFile { get; set; }

        public string? BookImagePath { get; set; }

        public string? BookFilePath { get; set; }

        public Author Author { get; set; }
    }
}