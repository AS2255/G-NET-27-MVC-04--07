using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.CategoryName)
                .HasColumnType("VARCHAR")
                .HasMaxLength(20);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.HasData(
                  new Category { Id = 1, CategoryName = "GeneralFitness"},
                  new Category { Id = 2, CategoryName = "Cardio" },
                  new Category { Id = 3, CategoryName = "Strength" },
                  new Category { Id = 4, CategoryName = "Yoga" },
                  new Category { Id = 5, CategoryName = "Boxing" },
                  new Category { Id = 6, CategoryName = "CrossFit" }
                  );

        }
    }
}

