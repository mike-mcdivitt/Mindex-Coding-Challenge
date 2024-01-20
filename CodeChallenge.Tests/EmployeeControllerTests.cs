
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using CodeChallenge.Models.Contracts;
using CodeChallenge.Models.Entities;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [TestMethod]
        public void GetEmployeeReportingStructure_Returns_Ok()
        {
            // Arrange
            const string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            const int expectedNumberOfReports = 4;
            
            // Act
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reporting-structure");
            var response = getRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructureContract>();
            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(employeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(expectedNumberOfReports, reportingStructure.NumberOfReports);
        }
        
        [TestMethod]
        public void GetEmployeeReportingStructure_Returns_NotFound()
        {
            // Arrange
            var employeeId = Guid.NewGuid().ToString();
        
            // Act
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reporting-structure");
            var response = getRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var compensation = new Compensation
            {
                EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309",
                Salary = 123456.78M,
                EffectiveDate = new DateTime(2024, 1, 1)
            };
        
            var requestContent = new JsonSerialization().ToJson(compensation);
        
            // Act
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        
            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.IsNotNull(newCompensation.CompensationId);
            Assert.AreEqual(compensation.EmployeeId, newCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }
        
        [TestMethod]
        public void CreateCompensation_Returns_BadRequest_When_EmployeeId_Is_Empty()
        {
            // Arrange
            var compensation = new Compensation
            {
                EmployeeId = string.Empty,
                Salary = 123456.78m,
                EffectiveDate = DateTime.Now
            };

            const string expectedErrorMessage = "EmployeeId is required.";
        
            var requestContent = new JsonSerialization().ToJson(compensation);
        
            // Act
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var validationProblemDetails = response.DeserializeContent<ValidationProblemDetails>();
            Assert.IsNotNull(validationProblemDetails);
            Assert.That.SingleValidationProblemDetailsErrorIsValid(validationProblemDetails, nameof(Compensation.EmployeeId), expectedErrorMessage);
        }

        [TestMethod]
        public void CreateCompensation_Returns_BadRequest_When_Employee_Does_Not_Exist()
        {
            // Arrange
            var compensation = new Compensation
            {
                EmployeeId = Guid.NewGuid().ToString(),
                Salary = 123456.78m,
                EffectiveDate = DateTime.Now
            };

            var expectedErrorMessage = $"Employee '{compensation.EmployeeId}' does not exist.";
        
            var requestContent = new JsonSerialization().ToJson(compensation);
        
            // Act
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var validationProblemDetails = response.DeserializeContent<ValidationProblemDetails>();
            Assert.IsNotNull(validationProblemDetails);
            Assert.That.SingleValidationProblemDetailsErrorIsValid(validationProblemDetails, nameof(Compensation.EmployeeId), expectedErrorMessage);
        }
        
        [TestMethod]
        public void CreateCompensation_Returns_BadRequest_When_Compensation_Already_Exists()
        {
            // Arrange
            var compensation = new Compensation
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 123456.78m,
                EffectiveDate = DateTime.Now
            };

            var expectedErrorMessage =
                $"Compensation for employee '{compensation.EmployeeId}' already exists.";
        
            var requestContent = new JsonSerialization().ToJson(compensation);
        
            // Act
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var validationProblemDetails = response.DeserializeContent<ValidationProblemDetails>();
            Assert.IsNotNull(validationProblemDetails);
            Assert.That.SingleValidationProblemDetailsErrorIsValid(validationProblemDetails, nameof(Compensation.EmployeeId), expectedErrorMessage);
        }
        
        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange
            const string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            const decimal expectedSalary = 205000;
            var expectedEffectiveDate = new DateTime(2024, 1, 1);
                
            // Act
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/compensation");
            var response = getRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(employeeId, compensation.EmployeeId);
            Assert.AreEqual(expectedSalary, compensation.Salary);
            Assert.AreEqual(expectedEffectiveDate.Date, compensation.EffectiveDate.Date);
        }
        
        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_NotFound()
        {
            // Arrange
            var employeeId = Guid.NewGuid().ToString();
        
            // Act
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/compensation");
            var response = getRequestTask.Result;
        
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
