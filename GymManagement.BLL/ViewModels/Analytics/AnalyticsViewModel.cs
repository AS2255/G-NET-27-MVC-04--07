using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.Analytics
{
    public class AnalyticsViewModel
    {
        public int TotalMember { get; set; }
        public int TotalTrainer { get; set; }
        public int ActiveMember { get; set; }
        public int UpcomingSession { get; set; }
        public int OngoingSession { get; set; }
        public int CompletedSession { get; set; }
    }
}
