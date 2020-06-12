using System.Collections.Generic;
using System.Threading.Tasks;
using Model;
using Models;

namespace Interfaces
{
    public interface IJobEmployeeService
    {
        IEnumerable<JobEmployee> All();
        IEnumerable<JobEmployee> FindByEmployeeId(long employeeId);
        Task FindAsync(long id);
        JobEmployee Find(long id);
        IEnumerable<Job> EmployeeJobs(long employeeId);
        IEnumerable<Job> EmployeeInProgressJobs(long employeeId);
        JobEmployee FindOrDefault(long id);
        Task AddAsync(JobEmployee jobEmployee);
        Task UpdateAsync(JobEmployee jobEmployee);
        Task RemoveAsync(JobEmployee jobEmployee);
    }
}