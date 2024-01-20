﻿using CodeChallenge.Models;
using System;
using System.Threading.Tasks;
using CodeChallenge.Models.Entities;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Task SaveAsync();
        Employee Update(Employee employee);
        bool AnyEmployee(string id);
        Employee GetReportingStructure(string id);
        Compensation GetCompensationByEmployeeId(string employeeId);
        Compensation AddCompensation(Compensation compensation);
        bool AnyCompensation(string employeeId);
    }
}