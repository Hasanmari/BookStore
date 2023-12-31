﻿using Microsoft.EntityFrameworkCore;

namespace BookStore.Models.Reopsitories
{
    public class BookDbRepository : IBookStoreRepository<Book>

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
            var book = DB.Books.Include(a => a.Author).SingleOrDefault(b => b.Id == id);
            return book;
        }

        public IList<Book> List()
        {
            return DB.Books.Include(a => a.Author).ToList();
        }

        public IList<Book> Search(string term)
        {
            var result = DB.Books.Include(a => a.Author)
             .Where(b => b.Title.Contains(term)
                        || b.Description.Contains(term)
                                   || b.Author.FullName.Contains(term)).ToList();
            return result;
        }

        public void Update(int id, Book newBook)
        {
            DB.Update(newBook);
            DB.SaveChanges();
        }
    }
}