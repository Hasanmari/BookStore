namespace BookStore.Models.Reopsitories
{
    public class AuthorRepository : IBookStoreRepository<Author>
    {
        private List<Author> authors;

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