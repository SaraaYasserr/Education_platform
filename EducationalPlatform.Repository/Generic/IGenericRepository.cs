using System.Linq.Expressions;

namespace EducationalPlatform.Repository.Generic
{
    /// <summary>
    /// A generic data-access contract shared by every entity in the system.
    /// Controllers/Services never talk to EF Core directly - they go through
    /// this interface (via IUnitOfWork), which keeps data-access concerns out
    /// of the business logic and makes the project easy to explain in a
    /// graduation defense: "every entity gets CRUD for free from one generic
    /// implementation."
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>Filtered query without tracking, for read-only lists.</summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Loads a single entity together with related navigation
        /// properties, e.g. GetWithIncludeAsync(c => c.Id == id, c => c.Category, c => c.Lessons).
        /// </summary>
        Task<T?> GetWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Loads a list of entities together with related navigation
        /// properties. Useful for course listings that need Category +
        /// Instructor + Reviews loaded in a single query.
        /// </summary>
        Task<IEnumerable<T>> GetAllWithIncludeAsync(
            params Expression<Func<T, object>>[] includes);

        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);

        /// <summary>Exposes IQueryable for advanced scenarios (paging, sorting, projections) the Service layer needs.</summary>
        IQueryable<T> Query();
    }
}
