using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class MembershipRepository : GenericRepository<MemberPlans>, IMembershipRepository
    {
        private readonly GymDbContext _context;
        public MembershipRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<MemberPlans> GetAllWithMemberAndPlans(Func<MemberPlans, bool>? condition = null)
            => _context.MemberPlans.Include(x => x.Member).Include(x => x.Plan).Where(condition ?? (_ => true));

    }

}
