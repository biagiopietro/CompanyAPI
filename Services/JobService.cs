using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services
{
    public class JobService : IJobService
    {
        private readonly CompanyContext _dbContext;

        public JobService(CompanyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Job job)
        {
            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<Job> All()
        {
            return _dbContext.Jobs;
        }

        public Job Find(long id)
        {
            return _dbContext.Jobs.Find(id);
        }

        public Job Find(string name)
        {
            return _dbContext.Jobs.FirstOrDefault(x => x.Name == name);
        }

        public async Task FindAsync(long id)
        {
            await _dbContext.Jobs.FindAsync(id);
        }

        public Job FindByNameOrDefault(string name)
        {
           return _dbContext.Jobs.SingleOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        public async Task RemoveAsync(Job job)
        {
            _dbContext.Jobs.Remove(job);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            _dbContext.Entry(job).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

        }
    }
}