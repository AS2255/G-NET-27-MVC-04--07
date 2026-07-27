namespace GymManagement.DAL.Data.Models
{
    public class HealthRecord : BaseEntity
    {
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string? Note { get; set; }
        public string BloodType { get; set; } = default!;

        //change UpdatedAt column name to LastUpdated 

        // Relationships
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }
    }
}
