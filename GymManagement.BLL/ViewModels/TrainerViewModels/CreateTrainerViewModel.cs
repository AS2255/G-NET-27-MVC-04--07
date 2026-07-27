using GymManagement.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.TrainerViewModels
{
    public class CreateTrainerViewModel
    {
        // Personal Information
        [Required(ErrorMessage = "Name is required!")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Name must be beween 2 and 500 characters")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name must contain only leters or spaces")]
        public string Name { get; set; } = default!;


        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;


        [Required(ErrorMessage = "Phone is required!")]
        [RegularExpression(@"^01[0|1|2|5]\d{8}$", ErrorMessage = "Phone number must be a valid Egyptian mobile number")]
        public string Phone { get; set; } = default!;


        [Required(ErrorMessage = "Date of birth is required!")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; } = default!;


        [Required(ErrorMessage = "Gender is required!")]
        public Gender Gender { get; set; } = default!; 


        // Address Infiormation
        [Required(ErrorMessage = "Building number is required")]
        [Range(1, 9000, ErrorMessage = "Building number must be greater that 0")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 500 characters")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Street must contain only letters or spaces")]
        public string Street { get; set; } = default!;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 500 characters")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "City must contain only letters or spaces")]
        public string City { get; set; } = default!;

        // Professional Information
        [Required(ErrorMessage = "Speciality is required")]
        public Speciality Speciality { get; set; } = default!;
    }
}
