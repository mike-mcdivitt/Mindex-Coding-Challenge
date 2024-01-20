using CodeChallenge.Models;
using System;
using CodeChallenge.Models.Contracts;
using CodeChallenge.Models.Entities;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
        Employee Update(Employee newEmployee);
        ReportingStructureContract GetReportingStructure(string employeeId);
        Compensation CreateCompensation(Compensation compensation);
        Compensation GetCompensationByEmployeeId(string employeeId);
        bool AnyEmployee(string id);
        bool AnyCompensation(string id);
    }
}
