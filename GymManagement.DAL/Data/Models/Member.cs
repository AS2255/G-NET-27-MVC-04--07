namespace GymManagement.DAL.Data.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        //change CreatedAt column name to JoinDate

        // Relationships
        public HealthRecord HealthRecord { get; set; } = default!;
        public ICollection<MemberPlans> MemberPlans { get; set; } = default!;
        public ICollection<MemberSessions> MemberSessions { get; set; } = default!;

    }
}
