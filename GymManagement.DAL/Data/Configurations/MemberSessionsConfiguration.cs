using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Configurations
{
    internal class MemberSessionsConfiguration : IEntityTypeConfiguration<MemberSessions>
    {
        public void Configure(EntityTypeBuilder<MemberSessions> builder)
        {
            builder.Ignore(x => x.Id);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("BookingDate")
                .HasDefaultValueSql("GETDATE()");

            builder.HasKey(x => new { x.MemberId, x.SessionId });
        }
    }
}
