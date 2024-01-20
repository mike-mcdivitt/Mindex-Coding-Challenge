using CodeChallenge.Models;
using System;
using System.Threading.Tasks;
using CodeChallenge.Models.Entities;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee GetReportingStructure(string id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Employee Update(Employee employee);
        Compensation GetCompensationByEmployeeId(string employeeId);
        Compensation AddCompensation(Compensation compensation);
        Task SaveAsync();
    }
}