using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IRepository<T>
    {
        Task<T> Add(T entity);
        Task<UpdateStatus> Update(T entity);
        Task<int> Delete(int entity);
        Task<T>? GetOne(Expression<Func<T, bool>> match); 
    }
}
