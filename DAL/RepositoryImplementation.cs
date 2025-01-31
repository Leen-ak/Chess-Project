using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DAL
{
    public class RepositoryImplementation<T> : IRepository<T> where T : ChessEntity
    {
        readonly private ChessContext _db;

        public RepositoryImplementation()
        {
            _db = new ChessContext();
        }

        public async Task<T> Add(T entity)
        {
            _db.Set<T>().Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<T>? GetOne(Expression<Func<T, bool>> match)
        {
            return await _db.Set<T>().FirstOrDefaultAsync(match);
        }

        public async Task<int> Delete(int entity)
        {
            T? currentEntity = await GetOne(ent => ent.Id == entity);
            _db.Set<T>().Remove(currentEntity);
            return _db.SaveChanges();
        }


        //It is failing from here 
        public async Task<UpdateStatus> Update(T entity)
        {
            UpdateStatus operationStatus = UpdateStatus.Failed;
            try
            {
                T? currentEntity = await GetOne(ent => ent.Id == entity.Id);
                _db.Entry(entity!).OriginalValues["Timer"] = entity.Timer;
                _db.Entry(entity!).CurrentValues.SetValues(entity);
                if (await _db.SaveChangesAsync() == 1)
                    operationStatus = UpdateStatus.Ok;
            }
            catch(DbUpdateConcurrencyException dbx)
            {
                operationStatus = UpdateStatus.Stale;
                Console.WriteLine("Problem in " + MethodBase.GetCurrentMethod()!.Name + dbx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem in " + MethodBase.GetCurrentMethod()!.Name + ex.Message);
            }
            return operationStatus;
        }
    }
}
