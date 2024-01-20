using System;
using System.Collections.Generic;

namespace CodeChallenge.Models.Entities
{
    public class Employee
    {
        public String EmployeeId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Position { get; set; }
        public String Department { get; set; }
        public List<Employee> DirectReports { get; set; } = new();
        public Compensation Compensation { get; set; }
    }
}
