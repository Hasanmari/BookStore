using System.Linq.Expressions;

namespace BookStore.Models.Reopsitories
{
    public class BookRepository : IBookStoreRepository<Book>
    {
        private List<Book> books;

        public void Add(Book entity)
        {
            entity.Id = books.Max(b => b.Id) + 1;
            books.Add(entity);
        }

        public void Delete(int id)
        {
            books.Remove(Find(id));
        }

        public Book Find(int id)
        {
            var book = books.SingleOrDefault(b => b.Id == id);
            return book;
        }

        public IList<Book> List()
        {
            return books;
        }

        public IList<Book> Search(string term)
        {
            var result = books.Where(b => b.Title.Contains(term)
            || b.Description.Contains(term)
            || b.Author.FullName.Contains(term)).ToList();
            return result;
        }

        public void Update(int id, Book NewBook)
        {
            var book = Find(id);
            book.Title = NewBook.Title;
            book.Description = NewBook.Description;
            book.Author = NewBook.Author;
            book.BookImagePath = NewBook.BookImagePath;  // Update the image path
            book.BookFilePath = NewBook.BookFilePath;    // Update the file path
        }
    }
}