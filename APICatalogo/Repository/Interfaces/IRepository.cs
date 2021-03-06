using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APICatalogo.Repository.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> Get();

        Task<T> GetById(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        List<T> LocalizaPagina<Tipo>(int numeroPagina, int quantidadeRegistros) where Tipo : class;

        int GetTotalRegistros();
    }
}
