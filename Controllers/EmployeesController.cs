using System;
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
    public class EmployeesController : ControllerBase
    {
        public static readonly int NUM_OF_RESULT_PER_PAGE = 5;

        private IEmployeeService _service;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ILogger<EmployeesController> logger,
                                    IEmployeeService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public ActionResult<EmployeeResponse> Get([FromQuery(Name = "page")] int page)
        {
            var employees = _service.All().ToArray();
            if (employees.Count() == 0)
            {
                return NoContent();
            }
            var paginatedEmployee = paginateResults(page, employees);
            if (paginatedEmployee.Count() == 0)
            {
                return NoContent();
            }
            return Ok(buildResponse(paginatedEmployee.ToArray()));
        }

        [HttpGet("{id}")]
        public ActionResult<EmployeeResponse> GetItem(long id)
        {
            var employee = _service.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(ConvertEmployeeToEmployeeResponse(employee));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> Post(EmployeeRequest employeeRequest)
        {
            if (!EmployeeExists(employeeRequest.SerialNumber))
            {
                var employee = ConvertEmployeeRequestToEmployee(employeeRequest);
                await _service.AddAsync(employee);
                return CreatedAtAction(nameof(GetItem), new { id = employee.Id }, ConvertEmployeeToEmployeeResponse(employee));
            }
            return new ConflictObjectResult(new { message = $"Already exists a job with this serial number {employeeRequest.SerialNumber}" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, EmployeeRequest employeeRequest)
        {
            if (id != employeeRequest.Id)
            {
                return BadRequest();
            }
            var employee = ConvertEmployeeRequestToEmployee(employeeRequest);
            if (!EmployeeExists(employee.SerialNumber))
            {
                try
                {
                    await _service.UpdateAsync(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(id))
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
                return new ConflictObjectResult(new { message = $"Cannot update the requested resource because already exists the same employee." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var employee = _service.Find(id);

            if (employee == null)
            {
                return NotFound();
            }
            await _service.RemoveAsync(employee);
            return NoContent();
        }
        private static IEnumerable<Employee> paginateResults(int page, IEnumerable<Employee> employees)
        {
            return paginateResults(page, NUM_OF_RESULT_PER_PAGE, employees);
        }
        private static IEnumerable<Employee> paginateResults(int page, int pageSize, IEnumerable<Employee> employees)
        {
            if (page <= 0)
            {
                page = 1;
            }
            return employees.ToPagedList(page, pageSize);

        }
        private bool EmployeeExists(long id)
        {
            return _service.FindAsync(id) != null;
        }
        private bool EmployeeExists(string serialNumber)
        {
            var employee = _service.FindBySerialNumberOrDefault(serialNumber);
            return (object.Equals(employee, default (Employee)))?  false : true;
        }

        private static IEnumerable<EmployeeResponse> buildResponse(Employee[] employees)
        {
            foreach (var employee in employees)
            {
                yield return ConvertEmployeeToEmployeeResponse(employee);
            }
        }

        private static Employee ConvertEmployeeRequestToEmployee(EmployeeRequest employeeRequest)
        {
            return new Employee
            {
                Id = employeeRequest.Id,
                Name = employeeRequest.Name,
                Surname = employeeRequest.Surname,
                Gender = (employeeRequest.Gender == "Male") ? Gender.Male : Gender.Female,
                Age = employeeRequest.Age,
                SerialNumber = employeeRequest.SerialNumber
            };
        }
        private static EmployeeResponse ConvertEmployeeToEmployeeResponse(Employee employee)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Surname = employee.Surname,
                FullName = employee.FullName,
                Age = employee.Age,
                SerialNumber = employee.SerialNumber,
                Gender = employee.Gender.ToString()
            };
        }
    }
}
