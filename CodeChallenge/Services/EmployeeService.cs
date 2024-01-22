using System;
using CodeChallenge.Models.Contracts;
using CodeChallenge.Models.Entities;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        [Obsolete("Replaced by the Update method which makes only 1 call to the database instead of 2.")]
        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists -- this is solved by making the GetById no tracking.
                    _employeeRepository.SaveAsync().Wait();
            
                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
        
        /// <summary>
        /// Updates a single employee.
        /// </summary>
        /// <param name="newEmployee">The employee to update.</param>
        /// <returns>The employee that has been updated.</returns>
        public Employee Update(Employee newEmployee)
        {
            _logger.LogDebug("Updating employee '{EmployeeId}'.", newEmployee.EmployeeId);
            
            _employeeRepository.Update(newEmployee);
            _employeeRepository.SaveAsync().Wait();

            return newEmployee;
        }
        
        /// <summary>
        /// Checks if an employee exists in the system.
        /// </summary>
        /// <param name="id">The EmployeeId.</param>
        /// <returns>True if the employee exists. False if the employee does not.</returns>
        public bool EmployeeExists(string id)
        {
            return _employeeRepository.EmployeeExists(id);
        }
        
        /// <summary>
        /// Retrieves the employee reporting structure which includes direct and indirect reports.
        /// </summary>
        /// <param name="employeeId">The EmployeeId.</param>
        /// <returns>The employee reporting structure.</returns>
        public ReportingStructureContract GetReportingStructure(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId)) return null;
            
            _logger.LogDebug("Retrieving reporting structure for employee '{EmployeeId}'.", employeeId);

            var employee = _employeeRepository.GetReportingStructure(employeeId);

            if (employee == null) return null;

            var numberOfReports = CalculateNumberOfReports(employee);
            
            return new ReportingStructureContract
            {
                Employee = employee,
                NumberOfReports = numberOfReports
            };
        }
        
        // I would argue Compensation should live under the Employee Aggregate instead of having its own Controller/Service..
        // ..at least until the project grows and more business rules are presented.
        #region Compensation
        
        /// <summary>
        /// Creates a new employee compensation.
        /// </summary>
        /// <param name="compensation">The employees compensation.</param>
        /// <returns></returns>
        public Compensation CreateCompensation(Compensation compensation)
        {
            if (compensation != null)
            {
                _logger.LogDebug("Creating compensation for employee '{EmployeeId}'.", compensation.EmployeeId);

                _employeeRepository.AddCompensation(compensation);
                _employeeRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        /// <summary>
        /// Retrieves the compensation of a single employee.
        /// </summary>
        /// <param name="employeeId">The EmployeeId</param>
        /// <returns>The employees compensation.</returns>
        public Compensation GetCompensationByEmployeeId(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId)) return null;
            
            _logger.LogDebug("Retrieving compensation for employee '{EmployeeId}'.", employeeId);

            return _employeeRepository.GetCompensationByEmployeeId(employeeId);
        }
        
        /// <summary>
        /// Checks if an employee already has a compensation.
        /// </summary>
        /// <param name="employeeId">The EmployeeId.</param>
        /// <returns>True if the employee compensation already exists. False if it does not.</returns>
        public bool CompensationExists(string employeeId)
        {
            return _employeeRepository.CompensationExists(employeeId);
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Recursively calculates the total number of direct and indirect reports for an employee.
        /// </summary>
        /// <param name="employee">The employee for whom to calculate the number of reports.</param>
        /// <returns>The total number of direct and indirect reports for the employee.</returns>
        private int CalculateNumberOfReports(Employee employee)
        {
            _logger.LogDebug("Calculating number of reports for employee '{EmployeeId}'.", employee.EmployeeId);

            // Count the number of direct reports for the current employee, or 0 if there are none
            var directReportsCount = employee.DirectReports?.Count ?? 0;

            // Initialize the count for indirect reports
            var indirectReportsCount = 0;

            // Check if the current employee has direct reports
            if (employee.DirectReports != null)
            {
                // Iterate through each direct report
                foreach (var directReport in employee.DirectReports)
                {
                    // Recursively calculate the number of reports for each direct report
                    indirectReportsCount += CalculateNumberOfReports(directReport);
                }
            }

            // Return the sum of direct and indirect reports
            return directReportsCount + indirectReportsCount;
        }
        
        #endregion
    }
}
