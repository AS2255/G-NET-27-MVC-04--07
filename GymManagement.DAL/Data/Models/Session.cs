using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Models
{
    public class Session : BaseEntity
    {
        public string Description { get; set; } = default!;
        public int Capacity { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }

        // Relationships
        public Trainer Trainer { get; set; } = default!;
        public int TrainerId { get; set; }

        public Category Category { get; set; } = default!;
        public int CategoryId { get; set; }

        public ICollection<MemberSessions> SessionMembers { get; set; } = default!;

    }
}
