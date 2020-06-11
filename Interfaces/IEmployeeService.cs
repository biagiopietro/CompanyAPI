using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> All();
        Task FindAsync(long id);
        Employee Find(long id);
        Employee FindBySerialNumberOrDefault(string serialNumber);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task RemoveAsync(Employee employee);
    }
}