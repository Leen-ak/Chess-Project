using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace DAL
{
    public class RepositoryImplementation<T> : IRepository<T> where T : ChessEntity
    {
        readonly private SomeSchoolContext _db;

        public RepositoryImplementation()
        {
            _db = new SomeSchoolContext();  
        }

        public Task<T> Add(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete(int entity)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateStatus> Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
