using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.PlanViewModels
{
    public class PlanToUpdateViewModel
    {
        public string name { get; set; } = default!;

        [Required(ErrorMessage = "Price is required!")]
        [Range(0.01,10000, ErrorMessage = "Price must be greater than 0")]
        public decimal price { get; set; }

        [Required(ErrorMessage = "Duration is required!")]
        [Range(1,365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int duration { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 200 characters")]
        public string description { get; set; } = default!;
    }
}
