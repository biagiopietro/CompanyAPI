using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using PagedList;
using Requests;
using Responses;

namespace CompanyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private static readonly int NUM_OF_RESULT_PER_PAGE = 5;

        private CompanyContext _dbContext;
        private readonly ILogger<JobsController> _logger;

        public JobsController(ILogger<JobsController> logger,
                                    CompanyContext context)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "page")] int page)
        {
            var jobs = _dbContext.Jobs.ToArray();
            if (jobs.Count() == 0)
            {
                return NoContent();
            }
            var paginatedJob = paginateResults(page, jobs);
            if (paginatedJob.Count() == 0)
            {
                return NoContent();
            }
            return Ok(buildResponse(paginatedJob.ToArray()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobResponse>> GetItem(long id)
        {
            var job = await _dbContext.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            return Ok(ConvertJobToJobResponse(job));
        }

        [HttpPost]
        public async Task<ActionResult<JobRequest>> Post(JobRequest jobRequest)
        {
            if (!JobExists(jobRequest.Name))
            {
                var job = ConvertJobRequestToJob(jobRequest);
                _dbContext.Jobs.Add(job);
                await _dbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetItem), new { id = job.Id }, ConvertJobToJobResponse(job));
            }
            return new ConflictObjectResult(new { message = $"Already exists a job with name {jobRequest.Name}" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, JobRequest jobRequest)
        {
            var job = ConvertJobRequestToJob(jobRequest);
            if (id != job.Id)
            {
                return BadRequest();
            }
            if (!JobExists(jobRequest.Name))
            {
                _dbContext.Entry(job).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
            else
            {
                return new ConflictObjectResult(new { message = $"Cannot update the requested resource because already exists a job with name '{jobRequest.Name}'" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var job = await _dbContext.Jobs.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            _dbContext.Jobs.Remove(job);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        private static IEnumerable<Job> paginateResults(int page, IEnumerable<Job> jobs)
        {
            return paginateResults(page, NUM_OF_RESULT_PER_PAGE, jobs);
        }
        private static IEnumerable<Job> paginateResults(int page, int pageSize, IEnumerable<Job> jobs)
        {
            if (page <= 0)
            {
                page = 1;
            }
            return jobs.ToPagedList(page, pageSize);

        }
        private bool JobExists(long id)
        {
            return _dbContext.Jobs.FindAsync(id) != null;
        }
        private bool JobExists(string name)
        {
            return _dbContext.Jobs.Where(x => x.Name == name).Count() > 0;
        }

        private static IEnumerable<JobResponse> buildResponse(Job[] jobs)
        {
            foreach (var job in jobs)
            {
                yield return ConvertJobToJobResponse(job);
            }
        }
        private static JobResponse ConvertJobToJobResponse(Job job)
        {
            return new JobResponse
            {
                Id = job.Id,
                Name = job.Name
            };
        }
        private static Job ConvertJobRequestToJob(JobRequest jobRequest)
        {
            return new Job
            {
                Id = jobRequest.Id,
                Name = jobRequest.Name
            };
        }
    }
}