using Microsoft.EntityFrameworkCore;
using SecureEmployeeManagement.Models;

namespace SecureEmployeeManagement.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
