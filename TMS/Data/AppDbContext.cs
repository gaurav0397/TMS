using Microsoft.EntityFrameworkCore;
using TMS_API.Models;

namespace TMS_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<WorkNote> WorkNotes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<WorkAttachment> WorkAttachments { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=localdatabase.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkItem>(entity =>
            {
                entity.Property(e => e.Description).HasColumnType("TEXT");
            });

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    EmployeeName="Team_A_Emp1",
                    TeamId=1,
                    IsManager=false,
                    IsAdmin=false
                },
                new Employee
                {
                    Id = 2,
                    EmployeeName = "Team_A_Emp2",
                    TeamId = 1,
                    IsManager = false,
                    IsAdmin = false
                },
                new Employee
                {
                    Id = 3,
                    EmployeeName = "Team_A_Manager",
                    TeamId = 1,
                    IsManager = true,
                    IsAdmin = false
                },
                new Employee
                {
                    Id = 4,
                    EmployeeName = "Team_B_Emp1",
                    TeamId = 2,
                    IsManager = false,
                    IsAdmin = false
                },
                new Employee
                {
                    Id = 5,
                    EmployeeName = "Team_B_Emp2",
                    TeamId = 2,
                    IsManager = false,
                    IsAdmin = false
                },
                new Employee
                {
                    Id = 6,
                    EmployeeName = "Team_B_Manager",
                    TeamId = 2,
                    IsManager = true,
                    IsAdmin = false
                },
                new Employee
                {
                    Id = 7,
                    EmployeeName = "Admin",
                    TeamId = null,
                    IsManager = false,
                    IsAdmin = true
                });

            modelBuilder.Entity<Team>().HasData(
                new Team
                {
                    Id=1,
                    TeamName="Team_A",
                    ManagerId=3
                },
                new Team
                {
                    Id = 2,
                    TeamName = "Team_A",
                    ManagerId = 3
                });



        }


    }
}
