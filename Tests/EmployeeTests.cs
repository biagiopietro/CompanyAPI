using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyAPI.Controllers;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using Requests;
using Responses;
using Xunit;

namespace Tests
{
    public class EmployeeTests
    {
        [Fact]
        public void Get_WhenCalled_ReturnOkResult_WithResources()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employees = GetFakeData();
            service.Setup(x => x.All()).Returns(employees);
            var controller = new EmployeesController(logger.Object, service.Object);

            int page = 1;
            //Act
            var results = controller.Get(page);

            //Assert
            var response = Assert.IsType<ActionResult<EmployeeResponse>>(results);
            var actionResult = response.Result as OkObjectResult;
            var statusCode = actionResult.StatusCode;
            var responseValues = actionResult.Value as IEnumerable<EmployeeResponse>;
            Assert.Equal(StatusCodes.Status200OK, statusCode);
            Assert.Equal(EmployeesController.NUM_OF_RESULT_PER_PAGE, responseValues.Count());
        }

        [Fact]
        public void Get_WhenCalled_ReturnNoContentResult()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employees = GetFakeData();
            service.Setup(x => x.All()).Returns(employees);
            var controller = new EmployeesController(logger.Object, service.Object);

            int page = 3;
            //Act
            var results = controller.Get(page);

            //Assert
            var response = Assert.IsType<ActionResult<EmployeeResponse>>(results);
            var actionResult = response.Result as NoContentResult;
            var statusCode = actionResult.StatusCode;
            Assert.Equal(StatusCodes.Status204NoContent, statusCode);
        }

        [Fact]
        public void Post_WhenCalled_PostAnEmployee_ReturnCreatedResult()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employeeToAdd = buildEmployeeRequest();
            Employee employee = null;
            service.Setup(x => x.FindBySerialNumberOrDefault(employeeToAdd.SerialNumber)).Returns(employee);
            var controller = new EmployeesController(logger.Object, service.Object);
            var expectedResponse = buildEmployeeResponse();
            //Act
            var results = controller.Post(employeeToAdd);

            //Assert
            var response = Assert.IsType<Task<ActionResult<EmployeeResponse>>>(results);
            var actionResult = response.Result.Result as CreatedAtActionResult;
            var statusCode = actionResult.StatusCode;
            var responseValue = actionResult.Value as EmployeeResponse;
            Assert.Equal(StatusCodes.Status201Created, statusCode);
            Assert.Equal(expectedResponse.SerialNumber, responseValue.SerialNumber);
        }

        [Fact]
        public void Post_WhenCalled_PostAnEmployeeThatExists_ReturnConflictResult()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employeeToAdd = buildEmployeeRequest();
            Employee employee = new Employee();
            service.Setup(x => x.FindBySerialNumberOrDefault(employeeToAdd.SerialNumber)).Returns(employee);
            var controller = new EmployeesController(logger.Object, service.Object);
            //Act
            var results = controller.Post(employeeToAdd);

            //Assert
            var response = Assert.IsType<Task<ActionResult<EmployeeResponse>>>(results);
            var actionResult = response.Result.Result as ConflictObjectResult;
            var statusCode = actionResult.StatusCode;
            Assert.Equal(StatusCodes.Status409Conflict, statusCode);
        }

        [Fact]
        public void Put_WhenCalled_PutAnEmployee_ReturnConflictResult()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employeeToPut = buildEmployeeRequest();
            Employee employee = new Employee();
            Task taskCompleted = Task.CompletedTask;
            service.Setup(x => x.FindBySerialNumberOrDefault(employeeToPut.Name)).Returns(employee);
            service.Setup(x => x.UpdateAsync(It.Ref<Employee>.IsAny)).Returns(taskCompleted);
            var controller = new EmployeesController(logger.Object, service.Object);
            //Act
            var results = controller.Put(employeeToPut.Id, employeeToPut);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as ConflictObjectResult;
            Assert.Equal(StatusCodes.Status409Conflict, statusCode.StatusCode);
        }

        [Fact]
        public void Put_WhenCalled_PutAnEmployee_ReturnNoContentResult()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employeeToPut = buildEmployeeRequest();
            Employee employeeNull = null;
            Task taskCompleted = Task.CompletedTask;
            service.Setup(x => x.FindBySerialNumberOrDefault(employeeToPut.SerialNumber)).Returns(employeeNull);
            service.Setup(x => x.UpdateAsync(It.Ref<Employee>.IsAny)).Returns(taskCompleted);
            var controller = new EmployeesController(logger.Object, service.Object);
            //Act
            var results = controller.Put(employeeToPut.Id, employeeToPut);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as NoContentResult;
            Assert.Equal(StatusCodes.Status204NoContent, statusCode.StatusCode);
        }

        [Fact]
        public void Put_WhenCalled_PutAnEmployee_ReturnBadRequestResult()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employeeToPut = buildEmployeeRequest();
            var controller = new EmployeesController(logger.Object, service.Object);
            var badId = employeeToPut.Id + 1;
            //Act
            var results = controller.Put(badId, employeeToPut);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as BadRequestResult;
            Assert.Equal(StatusCodes.Status400BadRequest, statusCode.StatusCode);
        }

        [Fact]
        public void Delete_WhenCalled_ReturnNoContentResult()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employeeToDelete = buildEmployeeRequest();
            var controller = new EmployeesController(logger.Object, service.Object);
            var employeeId = employeeToDelete.Id;
            Employee employee = new Employee();
            service.Setup(x => x.Find(employeeId)).Returns(employee);
            service.Setup(x => x.RemoveAsync(It.Ref<Employee>.IsAny)).Returns(Task.CompletedTask);
            //Act
            var results = controller.Delete(employeeId);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as NoContentResult;
            Assert.Equal(StatusCodes.Status204NoContent, statusCode.StatusCode);
        }

        [Fact]
        public void Delete_WhenCalled_ReturnNotFoundResult()
        {
            //Arrange
            var service = new Mock<IEmployeeService>();
            var logger = new Mock<ILogger<EmployeesController>>();

            var employeeToDelete = buildEmployeeRequest();
            var controller = new EmployeesController(logger.Object, service.Object);
            var employeeId = employeeToDelete.Id;
            Employee employee = null;
            service.Setup(x => x.Find(employeeId)).Returns(employee);
            //Act
            var results = controller.Delete(employeeId);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, statusCode.StatusCode);
        }

        private IEnumerable<Employee> GetFakeData()
        {
            var employee = new List<Employee>()
            {
                    new Employee
                    {
                        Name = "Phil",
                        Surname = "Rossi",
                        Age = 18,
                        Gender = Gender.Male,
                        SerialNumber = "QAWSEDRFT12"
                    },
                    new Employee
                    {
                        Name = "Jonathan",
                        Surname = "Markov",
                        Age = 25,
                        Gender = Gender.Male,
                        SerialNumber = "LAKSM15WRS"
                    },
                    new Employee
                    {
                        Name = "Lucas",
                        Surname = "Buffalo",
                        Age = 16,
                        Gender = Gender.Male,
                        SerialNumber = "UHASCB25SG"
                    },
                    new Employee
                    {
                        Name = "Jessica",
                        Surname = "Apple",
                        Age = 35,
                        Gender = Gender.Female,
                        SerialNumber = "PLMAVSC1D5"
                    },
                    new Employee
                    {
                        Name = "Francesca",
                        Surname = "Mc Grey",
                        Age = 16,
                        Gender = Gender.Female,
                        SerialNumber = "PLASTEFBN9"
                    }
            };
            return employee;
        }

        private EmployeeRequest buildEmployeeRequest()
        {
            return new EmployeeRequest
                    {
                        Name = "Paolo",
                        Surname = "Panna",
                        Age = 28,
                        Gender = "Male",
                        SerialNumber = "MNVBFG25DR"
                    };
        }
        private EmployeeResponse buildEmployeeResponse()
        {
            return new EmployeeResponse
            {
                Id = 0, // It doesn't care beacuse there is no connection to any db
                SerialNumber = "MNVBFG25DR"
            };
        }

    }
}