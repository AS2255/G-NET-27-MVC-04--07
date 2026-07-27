using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymManagement.DAL.Data.DbContexts
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.; Database=GymDb; Trusted_Connection=True; TrustServerCertificate=True");
        //}

        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Plan> Plans { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<MemberPlans> MemberPlans { get; set; }
        public DbSet<MemberSessions> MemberSessions { get; set; }
        public DbSet<Session> Sessions { get; set; }

    }
}
