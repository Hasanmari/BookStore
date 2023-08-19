namespace BookStore.Models.Reopsitories
{
	public class AuthorRepository : IBookStoreRepository<Author>
	{
		List<Author> authors;

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

		public void Update(int id, Author NewAuthor)
		{
			var author= Find(id);
			author.FullName = NewAuthor.FullName;
			
		}
	}
}
