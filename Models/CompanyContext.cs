
using Microsoft.EntityFrameworkCore;
using Model;

namespace Models
{
    public class CompanyContext : DbContext
    {
        public CompanyContext(
                 DbContextOptions<CompanyContext> options)
                 : base(options)    
                 {
                 }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobEmployee> JobEmployees { get; set; }
    }
}