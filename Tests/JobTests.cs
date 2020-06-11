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
    public class JobTests
    {
        [Fact]
        public void Get_WhenCalled_ReturnOkResult_WithResources()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobs = GetFakeData();
            service.Setup(x => x.All()).Returns(jobs);
            var controller = new JobsController(logger.Object, service.Object);

            int page = 1;
            //Act
            var results = controller.Get(page);

            //Assert
            var response = Assert.IsType<ActionResult<JobResponse>>(results);
            var actionResult = response.Result as OkObjectResult;
            var statusCode = actionResult.StatusCode;
            var responseValues = actionResult.Value as IEnumerable<JobResponse>;
            Assert.Equal(StatusCodes.Status200OK, statusCode);
            Assert.Equal(JobsController.NUM_OF_RESULT_PER_PAGE, responseValues.Count());
        }
        [Fact]
        public void Get_WhenCalled_ReturnNoContentResult()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobs = GetFakeData();
            service.Setup(x => x.All()).Returns(jobs);
            var controller = new JobsController(logger.Object, service.Object);

            int page = 3;
            //Act
            var results = controller.Get(page);

            //Assert
            var response = Assert.IsType<ActionResult<JobResponse>>(results);
            var actionResult = response.Result as NoContentResult;
            var statusCode = actionResult.StatusCode;
            Assert.Equal(StatusCodes.Status204NoContent, statusCode);
        }

        [Fact]
        public void Post_WhenCalled_PostAJob_ReturnCreatedResult()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobToAdd = buildJobRequest();
            Job jobNull = null;
            service.Setup(x => x.FindByNameOrDefault(jobToAdd.Name)).Returns(jobNull);
            var controller = new JobsController(logger.Object, service.Object);
            var expectedResponse = buildJobResponse();
            //Act
            var results = controller.Post(jobToAdd);

            //Assert
            var response = Assert.IsType<Task<ActionResult<JobResponse>>>(results);
            var actionResult = response.Result.Result as CreatedAtActionResult;
            var statusCode = actionResult.StatusCode;
            var responseValue = actionResult.Value as JobResponse;
            Assert.Equal(StatusCodes.Status201Created, statusCode);
            Assert.Equal(expectedResponse.Name, responseValue.Name);
        }

        [Fact]
        public void Post_WhenCalled_PostAJobThatExists_ReturnConflictResult()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobToAdd = buildJobRequest();
            Job job = new Job();
            service.Setup(x => x.FindByNameOrDefault(jobToAdd.Name)).Returns(job);
            var controller = new JobsController(logger.Object, service.Object);
            //Act
            var results = controller.Post(jobToAdd);

            //Assert
            var response = Assert.IsType<Task<ActionResult<JobResponse>>>(results);
            var actionResult = response.Result.Result as ConflictObjectResult;
            var statusCode = actionResult.StatusCode;
            Assert.Equal(StatusCodes.Status409Conflict, statusCode);
        }

        [Fact]
        public void Put_WhenCalled_PutAJob_ReturnConflictResult()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobToPut = buildJobRequest();
            Job job = new Job();
            Task completedTask = Task.CompletedTask;
            service.Setup(x => x.FindByNameOrDefault(jobToPut.Name)).Returns(job);
            service.Setup(x => x.UpdateAsync(It.Ref<Job>.IsAny)).Returns(completedTask);
            var controller = new JobsController(logger.Object, service.Object);
            //Act
            var results = controller.Put(jobToPut.Id, jobToPut);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as ConflictObjectResult;
            Assert.Equal(StatusCodes.Status409Conflict, statusCode.StatusCode);
        }

        [Fact]
        public void Put_WhenCalled_PutAJob_ReturnNoContentResult()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobToPut = buildJobRequest();
            Task t = Task.CompletedTask;
            Job jobNull = null;
            service.Setup(x => x.FindByNameOrDefault(jobToPut.Name)).Returns(jobNull);
            service.Setup(x => x.UpdateAsync(It.Ref<Job>.IsAny)).Returns(t);
            var controller = new JobsController(logger.Object, service.Object);
            //Act
            var results = controller.Put(jobToPut.Id, jobToPut);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as NoContentResult;
            Assert.Equal(StatusCodes.Status204NoContent, statusCode.StatusCode);
        }

        [Fact]
        public void Put_WhenCalled_PutAJob_ReturnBadRequestResult()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobToPut = buildJobRequest();
            var controller = new JobsController(logger.Object, service.Object);
            var badId = jobToPut.Id + 1;
            //Act
            var results = controller.Put(badId, jobToPut);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as BadRequestResult;
            Assert.Equal(StatusCodes.Status400BadRequest, statusCode.StatusCode);
        }

        [Fact]
        public void Delete_WhenCalled_ReturnNoContentResult()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobToDelete = buildJobRequest();
            var controller = new JobsController(logger.Object, service.Object);
            var jobId = jobToDelete.Id;
            Job job = new Job { };
            service.Setup(x => x.Find(jobId)).Returns(job);
            service.Setup(x => x.RemoveAsync(It.Ref<Job>.IsAny)).Returns(Task.CompletedTask);
            //Act
            var results = controller.Delete(jobId);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as NoContentResult;
            Assert.Equal(StatusCodes.Status204NoContent, statusCode.StatusCode);
        }

        [Fact]
        public void Delete_WhenCalled_ReturnNotFoundResult()
        {
            //Arrange
            var service = new Mock<IJobService>();
            var logger = new Mock<ILogger<JobsController>>();

            var jobToDelete = buildJobRequest();
            var controller = new JobsController(logger.Object, service.Object);
            var jobId = jobToDelete.Id;
            Job job = null;
            service.Setup(x => x.Find(jobId)).Returns(job);
            //Act
            var results = controller.Delete(jobId);

            //Assert
            var response = Assert.IsType<Task<IActionResult>>(results);
            var statusCode = response.Result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, statusCode.StatusCode);
        }

        private IEnumerable<Job> GetFakeData()
        {
            var jobs = new List<Job>()
            {
                 new Job { Name = "Engineer" },
                    new Job { Name = "Police man/woman" },
                    new Job { Name = "Teacher" },
                    new Job { Name = "Waiter" },
                    new Job { Name = "Recruiter" },
                    new Job { Name = "Bus Driver" }
            };
            return jobs;
        }

        private JobRequest buildJobRequest()
        {
            return new JobRequest
            {
                Id = 0,
                Name = "Wood Craftman",
            };
        }
        private JobRequest buildJobResponse()
        {
            return new JobRequest
            {
                Id = 0, // It doesn't care beacuse there is no connection to any db
                Name = "Wood Craftman",
            };
        }

    }
}