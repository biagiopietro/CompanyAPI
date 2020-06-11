using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly CompanyContext _dbContext;

        public EmployeeService(CompanyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Employee employee)
        {
            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<Employee> All()
        {
            return _dbContext.Employees;
        }

        public Employee Find(long id)
        {
            return _dbContext.Employees.Find(id);
        }

        public async Task FindAsync(long id)
        {
            await _dbContext.Employees.FindAsync(id);
        }

        public Employee FindBySerialNumberOrDefault(string serialNumber)
        {
            return _dbContext.Employees.SingleOrDefault(x => x.SerialNumber.ToLower().Equals(serialNumber.ToLower()));
        }

        public async Task RemoveAsync(Employee employee)
        {
            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _dbContext.Entry(employee).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

        }
    }
}