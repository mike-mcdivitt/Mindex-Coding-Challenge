using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models.Entities;

public class Compensation
{
    public string CompensationId { get; set; }
    public string EmployeeId { get; set; }
    public decimal Salary { get; set; }
    public DateTime EffectiveDate { get; set; }
}