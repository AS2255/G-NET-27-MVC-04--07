using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    internal class MemberPlansConfiguration : IEntityTypeConfiguration<MemberPlans>
    {
        public void Configure(EntityTypeBuilder<MemberPlans> builder)
        {
            builder.HasKey(mp => mp.Id);

            builder.Property(mp => mp.CreatedAt).HasColumnName("StartDate").HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd(); 

            builder.ToTable(tb => tb.HasCheckConstraint("CheckEndDateValid", "EndDate > StartDate"));

        }
    }
}
