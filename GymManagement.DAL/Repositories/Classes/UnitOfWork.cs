using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;
       
        private readonly Dictionary<string, object> _repositories = [];

        public ISessionRepository SessionRepository { get; }
        public IMembershipRepository MembershipRepository { get; }
        public IBookingRepository BookingRepository { get; }
        public UnitOfWork(
            GymDbContext dbContext, 
            ISessionRepository sessionRepository, 
            IMembershipRepository membershipRepository,
            IBookingRepository bookingRepository
            )
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
            MembershipRepository = membershipRepository;
            BookingRepository = bookingRepository;
        }
    

        //This is NOT the best implementation, but it is still commonly used
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            //check if _repositories dictionary already contains the repo or not and return it
            var typeName = typeof(TEntity).Name;
            if (_repositories.TryGetValue(typeName, out object? val))
            {
                //explicit cast from Object to IGenericRepository
                return (IGenericRepository<TEntity>) val;
            }
            else
            {
                //if it doesn't exist create a new repo and save it in the dictionary
                var repo = new GenericRepository<TEntity>(_dbContext);
                _repositories[typeName] = repo;
                return (IGenericRepository<TEntity>) _repositories[typeName];
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            await _dbContext.SaveChangesAsync(ct);

    }
}
