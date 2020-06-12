using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Model;
using Models;

namespace Services
{
    public class JobEmployeeService : IJobEmployeeService
    {
        private readonly CompanyContext _dbContext;

        public JobEmployeeService(CompanyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(JobEmployee jobEmployee)
        {
            _dbContext.JobEmployees.Add(jobEmployee);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<JobEmployee> All()
        {
            return _dbContext.JobEmployees;
        }

        public IEnumerable<Job> EmployeeInProgressJobs(long employeeId)
        {
            return _dbContext.JobEmployees.
                Include(x => x.Job).
                Include(x => x.Employee).Select(x => x).
                Where(x => x.Employee.Id == employeeId && x.End == DateTime.MinValue).
                Select(x => x.Job);
        }

        public IEnumerable<Job> EmployeeJobs(long employeeId)
        {
            throw new System.NotImplementedException();
        }

        public JobEmployee Find(long id)
        {
            return _dbContext.JobEmployees.Find(id);
        }

        public async Task FindAsync(long id)
        {
            await _dbContext.JobEmployees.FindAsync(id);
        }

        public IEnumerable<JobEmployee> FindByEmployeeId(long employeeId)
        {
            return _dbContext.JobEmployees
            .Include(x => x.Job)
            .Include(x => x.Employee)
            .Where(x => x.Employee.Id == employeeId);
        }

        public JobEmployee FindOrDefault(long id)
        {
            return _dbContext.JobEmployees.
            Include(x => x.Job).
            Include(x => x.Employee).
            SingleOrDefault(x => x.Id == id);
        }

        public async Task RemoveAsync(JobEmployee jobEmployee)
        {
            _dbContext.JobEmployees.Remove(jobEmployee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobEmployee jobEmployee)
        {
            _dbContext.Entry(jobEmployee).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

        }
    }
}