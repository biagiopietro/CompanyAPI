using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
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

        private IJobService _jobService;
        private IEmployeeService _employeeService;
        private IJobEmployeeService _service;
        private readonly ILogger<JobsEmployeesController> _logger;

        public JobsEmployeesController(ILogger<JobsEmployeesController> logger,
                                    IJobService jobService,
                                    IEmployeeService employeeService,
                                    IJobEmployeeService service)
        {
            _logger = logger;
            _jobService = jobService;
            _employeeService = employeeService;
            _service = service;
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "page")] int page, long employeeId)
        {
            if (_employeeService.Find(employeeId) == null)
            {
                return NotFound();
            }
            var jobEmployees = _service.FindByEmployeeId(employeeId);
            if (jobEmployees.Count() <= 0)
            {
                return NoContent();
            }
            var paginatedJobEmployee = paginateResults(page, jobEmployees);
            if (paginatedJobEmployee.Count() == 0)
            {
                return NoContent();
            }
            return Ok(buildResponse(paginatedJobEmployee.ToArray()));
        }

        [HttpPost]
        public async Task<ActionResult<JobEmployeeResponse>> Post(JobEmployeeRequest jobEmployeeRequest, long employeeId)
        {
            if (validateDataInPostRequest(jobEmployeeRequest))
            {
                var jobsInProgress = _service.EmployeeInProgressJobs(employeeId);
                bool jobAlreadyExists = jobsInProgress.Select(x => x).Where(x => x.Id == jobEmployeeRequest.JobId).Any();
                if (!jobAlreadyExists)
                {
                    var jobEmployee = ConvertJobEmployeeRequestToJobEmployee(jobEmployeeRequest, employeeId);
                    await _service.AddAsync(jobEmployee);
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
        public async Task<IActionResult> DeleteItem([FromQuery(Name = "contractCode"), BindRequired] long contractCode, long id, long employeeId)
        {
            var jobEmployee = _service.FindOrDefault(contractCode);
            if (object.Equals(jobEmployee, default(JobEmployee)))
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
            await _service.RemoveAsync(jobEmployee);
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
        private bool EmployeeExists(long id)
        {
            return _employeeService.FindAsync(id) != null;
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

        private JobEmployee ConvertJobEmployeeRequestToJobEmployee(JobEmployeeRequest jobEmployeeRequest, long employeeId)
        {
            return new JobEmployee(0,
                                    _employeeService.Find(employeeId),
                                    _jobService.Find(jobEmployeeRequest.JobId),
                                    jobEmployeeRequest.Start,
                                    jobEmployeeRequest.End,
                                    jobEmployeeRequest.MonthlySalaryGross);

        }
    }
}
