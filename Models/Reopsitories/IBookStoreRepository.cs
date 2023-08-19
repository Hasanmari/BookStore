namespace BookStore.Models.Reopsitories
{
	public interface IBookStoreRepository<TEntity>
	{
		IList<TEntity> List();
		TEntity Find(int id);
		void Add(TEntity entity);
		void Update(int id, TEntity entity);
		void Delete(int id);
      

    }
}
