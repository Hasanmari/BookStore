namespace BookStore.Models.Reopsitories
{
    public class AuthorRepository : IBookStoreRepository<Author>
    {
        private List<Author> authors;

        //constroctor to initialize the list of authors
        public AuthorRepository()
        {
            authors = new List<Author>()
            {
                new Author
                {
                    Id = 1,
                    FullName = "Ahmed"
                },
                new Author
                {
                    Id = 2,
                    FullName = "Mohamed"
                },
                new Author
                {
                    Id = 3,
                    FullName = "Ali"
                }
            };
        }

        public void Add(Author entity)
        {
            entity.Id = authors.Max(A => A.Id) + 1;
            authors.Add(entity);
        }

        public void Delete(int id)
        {
            authors.Remove(Find(id));
        }

        public Author Find(int id)
        {
            var author = authors.SingleOrDefault(A => A.Id == id);
            return author;
        }

        public IList<Author> List()
        {
            return authors;
        }

        public IList<Author> Search(string term)
        {
            var result = authors.Where(A => A.FullName.Contains(term)).ToList();
            return result;
        }

        public void Update(int id, Author NewAuthor)
        {
            var author = Find(id);
            author.FullName = NewAuthor.FullName;
        }
    }
}