using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.PlanViewModels
{
    public class PlanDetailsViewModel
    {
        public string name { get; set; } = default!;
        public decimal price { get; set; }
        public int duration { get; set; }
        public string description { get; set; } = default!;
        public bool isActive { get; set; }
    }
}
