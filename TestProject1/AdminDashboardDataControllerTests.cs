using Moq;
using NUnit.Framework;
using RoadReady.Controllers;
using RoadReady.Models;
using RoadReady.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using RoadReady.Exceptions;

namespace RoadReady.Tests.Controllers
{
    [TestFixture]
    public class AdminDashboardDataControllerTests
    {
        private Mock<IAdminDashboardDataRepository> _mockRepo;
        private AdminDashboardDataController _controller;

        [SetUp]
        public void SetUp()
        {
            // Mock the repository
            _mockRepo = new Mock<IAdminDashboardDataRepository>();
            _controller = new AdminDashboardDataController(_mockRepo.Object);
        }

        // Test: GetAllAdminDashboardData (GET)
        [Test]
        public async Task GetAllAdminDashboardData_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<AdminDashboardData>
    {
        new AdminDashboardData { DashboardId = 1, TotalReservations = 100, TotalRevenue = 5000.50m, TotalUsers = 50 },
        new AdminDashboardData { DashboardId = 2, TotalReservations = 150, TotalRevenue = 7500.75m, TotalUsers = 60 }
    };

            _mockRepo.Setup(repo => repo.GetAllAdminDashboardData()).Returns(mockData);

            // Act
            var result = _controller.GetAllAdminDashboardData(); // Removed `await` here

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode); // HTTP 200 OK
            Assert.AreEqual(mockData, okResult.Value);
        }



        [Test]
        public async Task GetAllAdminDashboardData_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllAdminDashboardData()).Throws(new Exception("Internal error"));

            // Act
            var result = _controller.GetAllAdminDashboardData(); // Removed await here

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode); // HTTP 500 Internal Server Error
        }

        // Test: AddDashboardData (POST)
        [Test]
        public async Task AddDashboardData_ShouldReturnCreatedResult_WhenDataIsValid()
        {
            // Arrange
            var newDashboardData = new AdminDashboardData
            {
                DashboardId = 3,
                TotalReservations = 120,
                TotalRevenue = 6000.60m,
                TotalUsers = 55
            };

            _mockRepo.Setup(repo => repo.AddDashboardDataAsync(It.IsAny<AdminDashboardData>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddDashboardData(newDashboardData);

            // Assert
            var createdAtResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.AreEqual(201, createdAtResult.StatusCode); // HTTP 201 Created
            Assert.AreEqual(newDashboardData, createdAtResult.Value);
        }

        [Test]
        public async Task AddDashboardData_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var invalidDashboardData = new AdminDashboardData { DashboardId = 0 }; // Invalid object

            _controller.ModelState.AddModelError("TotalReservations", "Required");

            // Act
            var result = await _controller.AddDashboardData(invalidDashboardData);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode); // HTTP 400 Bad Request
        }

        [Test]
        public async Task AddDashboardData_ShouldReturnConflict_WhenDuplicateDataOccurs()
        {
            // Arrange
            var duplicateDashboardData = new AdminDashboardData { DashboardId = 1 };

            _mockRepo.Setup(repo => repo.AddDashboardDataAsync(It.IsAny<AdminDashboardData>()))
                .Throws(new DuplicateResourceException("Duplicate resource"));

            // Act
            var result = await _controller.AddDashboardData(duplicateDashboardData);

            // Assert
            var conflictResult = result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
            Assert.AreEqual(409, conflictResult.StatusCode); // HTTP 409 Conflict
        }

        // Test: UpdateDashboardData (PUT)
        [Test]
        public async Task UpdateDashboardData_ShouldReturnOkResult_WhenDataIsUpdated()
        {
            // Arrange
            var updateData = new AdminDashboardData { DashboardId = 1, TotalReservations = 200 };

            _mockRepo.Setup(repo => repo.UpdateDashboardDataAsync(It.IsAny<AdminDashboardData>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateDashboardData(1, updateData);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode); // HTTP 200 OK
            Assert.AreEqual("ID 1 has been updated.", okResult.Value.ToString());
        }

        [Test]
        public async Task UpdateDashboardData_ShouldReturnNotFound_WhenDataDoesNotExist()
        {
            // Arrange
            var nonExistingId = 999; // ID that does not exist in the database
            var dashboardData = new AdminDashboardData { DashboardId = nonExistingId, TotalReservations = 100 };

            // Simulate that the data does not exist by returning null
            _mockRepo.Setup(repo => repo.GetDashboardDataByIdAsync(nonExistingId)).ReturnsAsync((AdminDashboardData)null);

            // Act
            var result = await _controller.UpdateDashboardData(nonExistingId, dashboardData);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNull(statusCodeResult);
            Assert.AreEqual(404, statusCodeResult.StatusCode); // HTTP 404 Not Found

            // Ensure the response body is not null and contains the 'message' property
            var response = statusCodeResult.Value as dynamic;
            Assert.IsNull(response);
            Assert.AreEqual("Data not found", response.message);
        }





    }
}
