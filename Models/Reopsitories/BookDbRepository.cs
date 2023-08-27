namespace BookStore.Models.Reopsitories
{
    public class BookDbRepository
    {
        private readonly BookStoreDbContext DB;

        public BookDbRepository(BookStoreDbContext _bookStoreDbContext)
        {
            DB = _bookStoreDbContext;
        }

        public void Add(Book book)
        {
            DB.Books.Add(book);
            DB.SaveChanges();
        }

        public void Delete(int id)
        {
            var book = Find(id);
            DB.Books.Remove(book);
            DB.SaveChanges();
        }

        public Book Find(int id)
        {
            var book = DB.Books.SingleOrDefault(b => b.Id == id);
            return book;
        }

        public IList<Book> List()
        {
            return DB.Books.ToList();
        }

        public void Update(int id, Book newBook)
        {
            DB.Update(newBook);
            DB.SaveChanges();
        }
    }
}