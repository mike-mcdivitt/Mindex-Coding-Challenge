using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Models.Entities;

namespace CodeChallenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);

                // Configure the Employee entity to represent self referential relationships.
                entity.HasMany(e => e.DirectReports);

                // Configure one to one relationship with Compensation
                entity.HasOne(e => e.Compensation)
                    .WithOne()
                    .HasForeignKey<Compensation>(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Compensation>(entity =>
            {
                entity.HasKey(e => e.CompensationId);
                
                // This generally has no affect on an In Memory database but should be added to
                // not violate referential integrity constraints in a real database scenario.
                // Adding a new compensation could benefit from this if an employee already has compensation.
                // Currently the api checks this condition so theres no need for concern.
                entity.HasIndex(u => u.EmployeeId).IsUnique();
            });
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Compensation> Compensations { get; set; }
    }
}
