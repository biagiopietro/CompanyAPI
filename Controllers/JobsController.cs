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
        public async Task<ActionResult<JobRequest>> GetItem(int id)
        {
            var todoItem = await _dbContext.Jobs.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(ConvertJobToJobResponse(todoItem));
        }

        [HttpPost]
        public async Task<ActionResult<JobRequest>> Post(Job job)
        {
            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItem), new { id = job.Id }, ConvertJobToJobResponse(job));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, Job job)
        {
            if (id != job.Id)
            {
                return BadRequest();
            }
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var todoItem = await _dbContext.Jobs.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _dbContext.Jobs.Remove(todoItem);
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
        private bool JobExists(int id)
        {
            return _dbContext.Jobs.FindAsync(id) != null;
        }
        private static IEnumerable<JobResponse> buildResponse(Job[] jobs)
        {
            foreach (var job in jobs)
            {
                yield return ConvertJobToJobResponse(job);
            }
        }
        public static JobResponse ConvertJobToJobResponse(Job job)
        {
            return new JobResponse(job.Id, job.Name);
        }
    }
}