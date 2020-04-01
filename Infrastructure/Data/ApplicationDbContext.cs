using Core.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyIdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions):base(dbContextOptions)
        {
        }

        public  DbSet<Meeting> Meeting { get; set; }
        public  DbSet<User_Meeting> User_Meeting { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User_Meeting>().HasOne(x => x.Meeting).WithMany(x => x.User_Meetings).HasForeignKey(x => x.MeetingId).IsRequired();

            modelBuilder.Entity<User_Meeting>().HasOne(x => x.User).WithMany(x => x.user_Meetings).HasForeignKey(x => x.UserId).IsRequired();

            modelBuilder.Entity<Meeting>().Property(x => x.CreaterId).HasMaxLength(450).IsRequired();

            modelBuilder.Entity<Meeting>().Property(x => x.MeetingState).HasMaxLength(20).IsRequired();


            base.OnModelCreating(modelBuilder);

        }
    }
}
