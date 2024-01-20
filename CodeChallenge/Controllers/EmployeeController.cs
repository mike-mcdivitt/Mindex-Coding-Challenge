using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
using CodeChallenge.Models.Entities;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            
            if (existingEmployee == null)
                return NotFound();

            // Using Update here instead to reduce calls to the database and simplify the action. 
            _employeeService.Update(newEmployee);
            // _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }
        
        [HttpGet("{id}/reporting-structure", Name = "getEmployeeReportingStructure")]
        public IActionResult GetEmployeeReportingStructure(String id)
        {
            _logger.LogDebug($"Received employee reporting structure get request for '{id}'");

            var reportingStructure = _employeeService.GetReportingStructure(id);

            if (reportingStructure == null)
                return NotFound();

            return Ok(reportingStructure);
        }
        
        [HttpPost("compensation", Name = "createCompensation")]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            // Check if EmployeeId is included in the compensation.
            if (string.IsNullOrEmpty(compensation.EmployeeId))
            {
                return BadRequest("EmployeeId is required.");
            }
            
            _logger.LogDebug($"Received compensation create request for employee '{compensation.EmployeeId}'");

            // Check if this employee even exists.
            var existingEmployee = _employeeService.GetById(compensation.EmployeeId);

            if (existingEmployee == null)
            {
                return BadRequest($"Employee '{compensation.EmployeeId}' does not exist.");
            }

            // Now that we know the EmployeeId is "valid", check if this employee already has compensation.
            var existingCompensation = _employeeService.GetCompensationByEmployeeId(compensation.EmployeeId);
            
            if (existingCompensation != null)
            {
                return BadRequest($"Compensation for employee '{compensation.EmployeeId}' already exists.");
            }
    
            _employeeService.CreateCompensation(compensation);
    
            return CreatedAtRoute("createCompensation", new { id = compensation.CompensationId }, compensation);
        }
    
        [HttpGet("{id}/compensation", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(string id)
        {
            _logger.LogDebug($"Received compensation get request for employee '{id}'");

            var compensation = _employeeService.GetCompensationByEmployeeId(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }
    }
}
