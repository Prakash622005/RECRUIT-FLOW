using Microsoft.EntityFrameworkCore;
using RecruitFlow.Models;

namespace RecruitFlow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicantUser> ApplicantUsers => Set<ApplicantUser>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicantUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Company>()
                .HasIndex(c => c.Username)
                .IsUnique();

            modelBuilder.Entity<JobApplication>()
                .Property(j => j.CurrentCTC)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<JobApplication>()
                .Property(j => j.ExpectedCTC)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<JobApplication>()
                .HasOne(j => j.ApplicantUser)
                .WithMany(u => u.JobApplications)
                .HasForeignKey(j => j.ApplicantUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
