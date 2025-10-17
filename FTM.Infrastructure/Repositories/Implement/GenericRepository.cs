using FTM.Domain.Entities;
using FTM.Domain.Specification;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly FTMDbContext _context;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver)
        {
            _context = context;
            _currentUserResolver = currentUserResolver;
            _dbSet = _context.Set<TEntity>();
        }

        protected FTMDbContext Context => _context;

        protected DbSet<TEntity> DbSet => _dbSet;

        public DbSet<TEntity> GetQuery()
        {
            return DbSet;
        }

        public async Task AddAsync(TEntity entity)
        {
            entity.CreatedOn = DateTimeOffset.Now;
            entity.LastModifiedOn = DateTimeOffset.Now;

            entity.CreatedBy = _currentUserResolver.Username ?? "FTMsystem";
            entity.CreatedByUserId = _currentUserResolver.UserId;
            entity.LastModifiedBy = _currentUserResolver.Username ?? "FTMsystem";

            await _dbSet.AddAsync(entity);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public void Delete(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            _dbSet.Remove(entity);
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);

            entity.LastModifiedOn = DateTimeOffset.Now;

            entity.LastModifiedBy = _currentUserResolver.Username ?? "FTMSystem";

            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public async Task AddAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                await AddAsync(entity);
            }
        }

        public async Task<TEntity> GetEntityWithSpec(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
        {
            var query = ApplySpecification(spec);

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<TEntity>> ListAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<TEntity> spec)
        {
            var query = ApplySpecification(spec);
            return await query.CountAsync();
        }

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
        {
            return SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), spec);
            //return null;
        }

        public void Delete(List<TEntity> entities)
        {
            entities.ForEach(x => Delete(x.Id));
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            entities = entities.Select(x =>
            {
                x.LastModifiedOn = DateTimeOffset.Now;
                return x;
            });

            _context.UpdateRange(entities);
        }
    }
}
