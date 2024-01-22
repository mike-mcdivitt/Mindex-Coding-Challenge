using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;
using CodeChallenge.Models.Entities;

namespace CodeChallenge.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRepository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        /// <summary>
        /// Retrieves a single employee by id.
        /// <remarks>This is a readonly query and uses AsNoTracking to disable change tracking. This helps to avoid
        /// issues where an instance of an Employee with the same key is already being tracked, specifically when
        /// calling this method prior to calling the Update method.</remarks>
        /// </summary>
        /// <param name="id">The EmployeeId.</param>
        /// <returns>A single employee.</returns>
        public Employee GetById(String id)
        {
            // Adding AsNoTracking here to avoid issues where an instance of an Employee with the same key is already being tracked.
            // Specifically when calling this method prior to calling the Update method.
            return _employeeContext.Employees.AsNoTracking() // <-- added
                .SingleOrDefault(e => e.EmployeeId == id);
        }
        
        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
        
        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        /// <summary>
        /// Updates a single employee.
        /// <remarks>Begins tracking the employee updating its state to Modified since the EmployeeId (PK) is being kept the same.
        /// No database interaction will be performed until SaveChanges() is called.</remarks>
        /// </summary>
        /// <param name="employee">The employee to be updated.</param>
        /// <returns>The now tracked updated employee.</returns>
        public Employee Update(Employee employee)
        {
            return _employeeContext.Update(employee).Entity;
        }
        
        /// <summary>
        /// Checks if an employee exists.
        /// </summary>
        /// <param name="id">The EmployeeId.</param>
        /// <returns>True if the employee exists. False if the employee does not.</returns>
        public bool EmployeeExists(string id)
        {
            return _employeeContext.Employees.AsNoTracking().Any(e => e.EmployeeId == id);
        }
        
        /// <summary>
        /// Returns a single employee including their direct and indirect reports.
        /// </summary>
        /// <remarks>This is a readonly query and uses AsNoTracking to disable change tracking while
        /// utilizing Eager loading to load all related employees in one query.</remarks>
        /// <param name="id">The EmployeeId.</param>
        /// <returns>The employee for whom to get the reporting structure for.</returns>
        public Employee GetReportingStructure(string id)
        {
            return _employeeContext.Employees.AsNoTracking()
                .Where(e => e.EmployeeId == id)
                .Include(e => e.DirectReports)
                .ThenInclude(e => e.DirectReports)
                .SingleOrDefault();
        }
        
        /// <summary>
        /// Retrieves a single employees compensation.
        /// </summary>
        /// <remarks>This is a readonly query and uses AsNoTracking to disable change tracking.</remarks>
        /// <param name="employeeId">The EmployeeId.</param>
        /// <returns>The employees compensation.</returns>
        public Compensation GetCompensationByEmployeeId(string employeeId)
        {
            return _employeeContext.Compensations.AsNoTracking()
                .SingleOrDefault(e => e.EmployeeId == employeeId);
        }

        /// <summary>
        /// Adds a new compensation for a single employee.
        /// </summary>
        /// <param name="compensation">The compensation to add.</param>
        /// <returns>The new compensation.</returns>
        public Compensation AddCompensation(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }

        /// <summary>
        /// Checks if an existing employee compensation exists.
        /// </summary>
        /// <remarks>This is a readonly query and uses AsNoTracking to disable change tracking.</remarks>
        /// <param name="employeeId">The EmployeeId.</param>
        /// <returns>True if the employee compensation exists. False if it does not.</returns>
        public bool CompensationExists(string employeeId)
        {
            return _employeeContext.Compensations.AsNoTracking().Any(e => e.EmployeeId == employeeId);
        }
    }
}
