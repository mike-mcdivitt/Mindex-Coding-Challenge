using CodeChallenge.Models.Entities;

namespace CodeChallenge.Models.Contracts;

/// <summary>
/// Api contract that holds the employees reporting structure which includes the Employee information
/// and the total number of direct and indirect reports.
/// </summary>
public class ReportingStructureContract
{
    /// <summary>
    /// The Employee for whom the reporting structure is being retrieved for.
    /// </summary>
    public Employee Employee { get; set; }
    /// <summary>
    /// The total number of direct and indirect reports for the employee.
    /// </summary>
    public int NumberOfReports { get; set; }
}