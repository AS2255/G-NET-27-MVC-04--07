using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.Analytics;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default)
        {
            
            var upcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > DateTime.Now);
            var ongoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate <= DateTime.Now && s.EndDate > DateTime.Now);
            var completedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.EndDate <= DateTime.Now);
            var totalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(ct: ct);
            var totalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var totalActiveMembers = await _unitOfWork.GetRepository<MemberPlans>().CountAsync(ct: ct);

            return new AnalyticsViewModel
            {
                TotalMember = totalMembers,
                TotalTrainer = totalTrainers,
                ActiveMember = totalActiveMembers,
                UpcomingSession = upcomingSessions,
                OngoingSession = ongoingSessions,
                CompletedSession = completedSessions
            };
        }
    }
}
