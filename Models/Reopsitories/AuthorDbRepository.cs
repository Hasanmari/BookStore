﻿namespace BookStore.Models.Reopsitories
{
    public class AuthorDbRepository : IBookStoreRepository<Author>
    {
        private readonly BookStoreDbContext DB;

        public AuthorDbRepository(BookStoreDbContext _bookStoreDbContext)
        {
            DB = _bookStoreDbContext;
        }

        public void Add(Author author)
        {
            DB.Authors.Add(author);
            DB.SaveChanges();
        }

        public void Delete(int id)
        {
            var author = Find(id);
            DB.Authors.Remove(author);
            DB.SaveChanges();
        }

        public Author Find(int id)
        {
            var author = DB.Authors.SingleOrDefault(a => a.Id == id);
            return author;
        }

        public IList<Author> List()
        {
            return DB.Authors.ToList();
        }

        public IList<Author> Search(string term)
        {
            var result = DB.Authors.Where(a => a.FullName.Contains(term)).ToList();
            return result;
        }

        public void Update(int id, Author newAuthor)
        {
            DB.Update(newAuthor);
            DB.SaveChanges();
        }
    }
}