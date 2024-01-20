using CodeChallenge.Models;
using System;
using CodeChallenge.Models.Dtos;
using CodeChallenge.Models.Entities;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
        Employee Update(Employee newEmployee);
        ReportingStructure GetReportingStructure(string employeeId);
        Compensation CreateCompensation(Compensation compensation);
        Compensation GetCompensationByEmployeeId(String employeeId);
    }
}
