using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Model;
using Models;
using PagedList;
using Requests;
using Responses;

namespace CompanyAPI.Controllers
{
    [ApiController]
    [Route("api/Employee/{employeeId}/Jobs")]
    public class JobsEmployeesController : ControllerBase
    {
        private static readonly int NUM_OF_RESULT_PER_PAGE = 5;

        private CompanyContext _dbContext;
        private readonly ILogger<JobsEmployeesController> _logger;

        public JobsEmployeesController(ILogger<JobsEmployeesController> logger,
                                    CompanyContext context)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "page")] int page, int employeeId)
        {
            var jobEmployees = _dbContext.JobEmployees
            .Include(x => x.Job)
            .Include(x => x.Employee);
            var jobs = GetJobsFromEmployeeId(jobEmployees, employeeId);
            if (jobs.Count() <= 0)
            {
                return NoContent();
            }
            return Ok(jobs);
        }

        [HttpPost]
        public async Task<ActionResult<JobEmployeeResponse>> Post(JobEmployeeRequest jobEmployeeRequest, int employeeId)
        {
            if (validateDataInPostRequest(jobEmployeeRequest))
            {
                var jobEmployees = _dbContext.JobEmployees
                .Include(x => x.Job)
                .Include(x => x.Employee);
                var jobsInProgress = GetInProgressJobsFromEmployeeId(jobEmployees, employeeId);
                bool jobAlreadyExists = jobsInProgress.Select(x => x).Where(x => x.Id == jobEmployeeRequest.JobId).Any();
                if (!jobAlreadyExists)
                {
                    var jobEmployee = ConvertJobEmployeeRequestToJobEmployee(jobEmployeeRequest, employeeId);
                    _dbContext.JobEmployees.Add(jobEmployee);
                    await _dbContext.SaveChangesAsync();
                    return CreatedAtAction(nameof(Get), new { employeeId = jobEmployee.Employee.Id }, ConvertJobEmployeeToJobEmployeeResponse(jobEmployee));
                }
                else
                {
                    return new ConflictObjectResult(new { message = $"The employee with id={employeeId} has already a work in progress."});
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem([FromQuery(Name = "contractCod"), BindRequired] int contractCode, int id, int employeeId)
        {
            var jobEmployee = await _dbContext.JobEmployees.
            Include(x => x.Employee).
            Include(x => x.Job).
            SingleOrDefaultAsync(x => x.Id == contractCode);
            if (jobEmployee == null)
            {
                return NotFound();
            }
            if(jobEmployee.Employee.Id != employeeId)
            {
                return new UnauthorizedObjectResult(new {message="The employeedId specified in the url and the employeeId of the requested resource are not the same."});
            }
            if(jobEmployee.Job.Id != id)
            {
                return new UnauthorizedObjectResult(new {message="The id specified in the url and the jobId of the requested resource are not the same."});
            }
            _dbContext.JobEmployees.Remove(jobEmployee);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private static bool validateDataInPostRequest(JobEmployeeRequest jobEmployeeRequest)
        {
            return jobEmployeeRequest.JobId > 0 &&
                   jobEmployeeRequest.Start != DateTime.MinValue && 
                   jobEmployeeRequest.MonthlySalaryGross > 0;
        }

        private static Job[] GetJobsFromEmployeeId(IIncludableQueryable<Model.JobEmployee, Employee> jobEmployees, int employeeId)
        {
            return jobEmployees.Select(x => x).
                Where(x => x.Employee.Id == employeeId).
                Select(x => x.Job).ToArray();
        }
        private static Job[] GetInProgressJobsFromEmployeeId(IIncludableQueryable<Model.JobEmployee, Employee> jobEmployees, int employeeId)
        {
            return jobEmployees.Select(x => x).
                Where(x => x.Employee.Id == employeeId && x.End == DateTime.MinValue).
                Select(x => x.Job).ToArray();
        }

        private static IEnumerable<JobEmployee> paginateResults(int page, IEnumerable<JobEmployee> jobEmployees)
        {
            return paginateResults(page, NUM_OF_RESULT_PER_PAGE, jobEmployees);
        }
        private static IEnumerable<JobEmployee> paginateResults(int page, int pageSize, IEnumerable<JobEmployee> jobEmployees)
        {
            if (page <= 0)
            {
                page = 1;
            }
            return jobEmployees.ToPagedList(page, pageSize);

        }
        private bool EmployeeExists(int id)
        {
            return _dbContext.Employees.FindAsync(id) != null;
        }

        private static IEnumerable<JobEmployeeResponse> buildResponse(JobEmployee[] jobEmployees)
        {
            foreach (var jobEmployee in jobEmployees)
            {
                yield return ConvertJobEmployeeToJobEmployeeResponse(jobEmployee);
            }
        }

        private static JobEmployeeResponse ConvertJobEmployeeToJobEmployeeResponse(JobEmployee jobEmployee)
        {
            return new JobEmployeeResponse(jobEmployee.Id,
                                                  jobEmployee.Job.Name,
                                                  jobEmployee.Start,
                                                  jobEmployee.End,
                                                  jobEmployee.MonthlySalaryGross,
                                                  jobEmployee.YearlySalaryGross);

        }

        private JobEmployee ConvertJobEmployeeRequestToJobEmployee(JobEmployeeRequest jobEmployeeRequest, int employeeId)
        {
            return new JobEmployee(0,
                                    _dbContext.Employees.Find(employeeId),
                                    _dbContext.Jobs.Find(jobEmployeeRequest.JobId),
                                    jobEmployeeRequest.Start,
                                    jobEmployeeRequest.End,
                                    jobEmployeeRequest.MonthlySalaryGross);

        }
    }
}
