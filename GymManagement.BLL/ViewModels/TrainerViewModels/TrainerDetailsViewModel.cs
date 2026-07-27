using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.TrainerViewModels
{
    public class TrainerDetailsViewModel
    {
        public string Name { get; set; } = default!; 
        public string Speciality { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string DateOfBirth { get; set; } = default!;
        public string Address { get; set; } = default!;
    }
}
