using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    internal class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable(tb => tb.HasCheckConstraint("SessionCapacityCheck", "Capacity BETWEEN 1 AND 25"));
            builder.ToTable(tb => tb.HasCheckConstraint("SessionEndDateCheck", "EndDate > StartDate"));
        }

    }
}
