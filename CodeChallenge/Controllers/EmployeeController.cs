using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models.Entities;
using Microsoft.AspNetCore.Http;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEmployeeReportingStructure(String id)
        {
            _logger.LogDebug($"Received employee reporting structure get request for '{id}'");

            var reportingStructure = _employeeService.GetReportingStructure(id);

            if (reportingStructure == null)
                return NotFound();

            return Ok(reportingStructure);
        }
        
        [HttpPost("compensation", Name = "createCompensation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            // Start by validating the EmployeeId.
            if (string.IsNullOrEmpty(compensation.EmployeeId))
            {
                ModelState.AddModelError(nameof(Compensation.EmployeeId), "EmployeeId is required.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            
            _logger.LogDebug($"Received compensation create request for employee '{compensation.EmployeeId}'");

            // Check if this employee exists.
            var employeeExists = _employeeService.AnyEmployee(compensation.EmployeeId);

            if (employeeExists == false)
            {
                ModelState.AddModelError(nameof(Compensation.EmployeeId),
                    $"Employee '{compensation.EmployeeId}' does not exist.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Now that we know the EmployeeId is "valid", check if this employee already has compensation.
            var compensationExists = _employeeService.AnyCompensation(compensation.EmployeeId);
            
            if (compensationExists)
            {
                ModelState.AddModelError(nameof(Compensation.EmployeeId),
                    $"Compensation for employee '{compensation.EmployeeId}' already exists.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
    
            _employeeService.CreateCompensation(compensation);
           
            return CreatedAtRoute("createCompensation", new { id = compensation.CompensationId }, compensation);
        }
    
        [HttpGet("{id}/compensation", Name = "getCompensationByEmployeeId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
