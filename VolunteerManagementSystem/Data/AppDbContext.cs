using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Volunteer> Volunteers => Set<Volunteer>();
        public DbSet<Opportunity> Opportunities => Set<Opportunity>(); 
    }
}
