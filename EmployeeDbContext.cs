using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EmployeeManagement
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}
