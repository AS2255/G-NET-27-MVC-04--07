using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IMembershipRepository
    {
        IEnumerable<MemberPlans> GetAllWithMemberAndPlans(Func<MemberPlans, bool>? condition = null);

    }

}
