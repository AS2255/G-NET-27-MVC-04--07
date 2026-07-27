using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.PlanViewModels
{
    public class PlanViewModel
    {
        public int id { get; set; }
        public string name { get; set; } = default!;
        public decimal price { get; set; }
        public int duration { get; set; }
        public string description { get; set; } = default!;
        public bool isActive { get; set; }
    }
}
