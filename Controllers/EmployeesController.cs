using System;
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
    public class EmployeesController : ControllerBase
    {
        private static readonly int NUM_OF_RESULT_PER_PAGE = 5;

        private CompanyContext _dbContext;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ILogger<EmployeesController> logger,
                                    CompanyContext context)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "page")] int page)
        {
            var employees = _dbContext.Employees.ToArray();
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
        public async Task<ActionResult<EmployeeRequest>> GetItem(int id)
        {
            var todoItem = await _dbContext.Employees.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(ConvertEmployeeToEmployeeResponse(todoItem));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeRequest>> Post(Employee employee)
        {
            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItem), new { id = employee.Id }, ConvertEmployeeToEmployeeResponse(employee));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }
            _dbContext.Entry(employee).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var todoItem = await _dbContext.Employees.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _dbContext.Employees.Remove(todoItem);
            await _dbContext.SaveChangesAsync();

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
        private bool EmployeeExists(int id)
        {
            return _dbContext.Employees.FindAsync(id) != null;
        }

        private static IEnumerable<EmployeeResponse> buildResponse(Employee[] employees)
        {
            foreach (var employee in employees)
            {
                yield return ConvertEmployeeToEmployeeResponse(employee);
            }
        }

        private static EmployeeResponse ConvertEmployeeToEmployeeResponse(Employee employee)
        {
            return new EmployeeResponse(employee.Id,
                                                  employee.Name,
                                                  employee.Surname,
                                                  employee.FullName,
                                                  employee.Age,
                                                  employee.Gender.ToString());

        }
    }
}
