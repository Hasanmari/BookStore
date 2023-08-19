namespace BookStore.Models.Reopsitories
{
    public class BookRepository : IBookStoreRepository<Book>
    {
        private List<Book> books;

        public BookRepository()
        {
            books = new List<Book>()
            {
                new Book
                {
                    Id = 1,
                    Title = "C# Programming",
                    Description = "No Description",
                },
                new Book
                {
                    Id = 2,
                    Title = "Java Programming",
                    Description = "Nothing",
                },
                new Book
                {
                    Id = 3,
                    Title = "Python Programming",
                    Description = "No Description",
                }
            };
        }

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

        public void Update(int id, Book NewBook)
        {
            var book = Find(id);
            book.Title = NewBook.Title;
            book.Description = NewBook.Description;
            book.Author = NewBook.Author;
        }
    }
}