using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
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
        public static readonly int NUM_OF_RESULT_PER_PAGE = 5;


        private IJobService _service;

        private readonly ILogger<JobsController> _logger;

        public JobsController(ILogger<JobsController> logger,
                                    IJobService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public ActionResult<JobResponse> Get([FromQuery(Name = "page")] int page)
        {
            var jobs = _service.All().ToArray();
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
        public ActionResult<JobResponse> GetItem(long id)
        {
            var job = _service.Find(id);
            if (job == null)
            {
                return NotFound();
            }

            return Ok(ConvertJobToJobResponse(job));
        }

        [HttpPost]
        public async Task<ActionResult<JobResponse>> Post(JobRequest jobRequest)
        {
            if (!JobExists(jobRequest.Name))
            {
                var job = ConvertJobRequestToJob(jobRequest);
                await _service.AddAsync(job);
                return CreatedAtAction(nameof(GetItem), new { id = job.Id }, ConvertJobToJobResponse(job));
            }
            return new ConflictObjectResult(new { message = $"Already exists a job with name {jobRequest.Name}" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, JobRequest jobRequest)
        {
            if (id != jobRequest.Id)
            {
                return BadRequest();
            }
            var job = ConvertJobRequestToJob(jobRequest);
            if (!JobExists(jobRequest.Name))
            {
                try
                {
                    await _service.UpdateAsync(job);
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
            var job = _service.Find(id);
            if (job == null)
            {
                return NotFound();
            }
            await _service.RemoveAsync(job);
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
            return _service.FindAsync(id) != null;
        }
        private bool JobExists(string name)
        {
            var job = _service.FindByNameOrDefault(name);
            
            return (object.Equals(job, default (Job)))?  false : true;
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