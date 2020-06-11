using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace Interfaces
{
    public interface IJobService
    {
        IEnumerable<Job> All();
        Task FindAsync(long id);
        Job Find(long id);
        Job Find(string name);
        Job FindByNameOrDefault(string name);
        Task AddAsync(Job job);
        Task UpdateAsync(Job job);
        Task RemoveAsync(Job job);
    }
}