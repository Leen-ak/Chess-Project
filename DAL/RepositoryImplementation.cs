using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        public async Task<List<T>> GetAll()
        {
            return await _db.Set<T>().ToListAsync();
        }

        public async Task<int> Delete(int entity)
        {
            T? currentEntity = await GetOne(ent => ent.Id == entity);
            _db.Set<T>().Remove(currentEntity);
            return _db.SaveChanges();
        }

        public async Task<UpdateStatus> Update(T entity)
        {
            UpdateStatus operationStatus = UpdateStatus.Failed;
            try
            {
                T? currentEntity = await GetOne(ent => ent.Id == entity.Id);
                if (currentEntity == null)
                {
                    Debug.WriteLine($"Entity with ID {entity.Id} not found.");
                    return UpdateStatus.Failed; 
                }

                Debug.WriteLine($"Before update: {currentEntity}");
                Debug.WriteLine($"Incoming data: {entity}");

                _db.Entry(currentEntity).CurrentValues.SetValues(entity);

                var changes = _db.ChangeTracker.Entries<T>().Select(e => e.State).ToList();
                Debug.WriteLine("Entity states before save: " + string.Join(", ", changes));

                _db.Entry(currentEntity).State = EntityState.Modified;

                int rowsAffected = await _db.SaveChangesAsync();

                if (rowsAffected > 0)
                    operationStatus = UpdateStatus.Ok;
                else
                    Debug.WriteLine("No rows were updated. Entity might be unchanged.");
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
