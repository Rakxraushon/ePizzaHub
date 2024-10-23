using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;

namespace ePizzaHub.Services.Implementations
{
    public class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        protected IRepository<TEntity> _repository;
        public Service(IRepository<TEntity> repository)
        {
            _repository = repository;
        }
        public void Add(TEntity entity)
        {
            _repository.Add(entity);
            _repository.SaveChanges();
        }

        public void Delete(object id)
        {
            _repository.Delete(id);
            _repository.SaveChanges();
        }

        public TEntity Find(object id)
        {
            return _repository.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _repository.GetAll();
        }

        public void Remove(TEntity entity)
        {
            _repository.Remove(entity);
            _repository.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _repository.Update(entity);
            _repository.SaveChanges();
        }
    }
}
