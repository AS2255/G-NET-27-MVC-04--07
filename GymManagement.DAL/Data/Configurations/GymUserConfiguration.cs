using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasColumnType("VARCHAR")
                .HasMaxLength(11);

            builder.Property(x => x.Gender)
                .HasConversion<string>();

            builder.ToTable(tb => tb.HasCheckConstraint("EmailCheck", "Email Like '_%@_%._%'"));
            builder.ToTable(tb => tb.HasCheckConstraint("PhoneCheck", "Phone Like '01[0125]%'"));


            builder.HasIndex(x => x.Phone).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();

            builder.OwnsOne(x => x.Address, address =>
                {
                    address.Property(a => a.Street).HasColumnType("VARCHAR").HasMaxLength(30);
                    address.Property(a => a.City).HasColumnType("VARCHAR").HasMaxLength(30);
                });

        }
    }
}
