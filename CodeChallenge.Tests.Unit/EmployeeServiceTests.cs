using CodeChallenge.Models;
using CodeChallenge.Models.Entities;
using CodeChallenge.Repositories;
using CodeChallenge.Services;
using Microsoft.Extensions.Logging;
using NSubstitute.ReturnsExtensions;

namespace CodeChallenge.Tests.Unit;

public class EmployeeServiceTests
{
    private readonly IEmployeeRepository _repositorySub;
    private readonly ILogger<EmployeeService> _loggerSub;
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _loggerSub = Substitute.For<ILogger<EmployeeService>>();
        _repositorySub = Substitute.For<IEmployeeRepository>();
        _sut = new EmployeeService(_loggerSub, _repositorySub);
    }
    
    [Fact]
    public void GetReportingStructure_Returns_ReportingStructure()
    {
        // Arrange
        const int expectedNumberOfReports = 3;
        
        var employee = new Employee
        {
            EmployeeId = Guid.NewGuid().ToString(),
            FirstName = "Paul",
            LastName = "McCartney",
            Position = "Manager",
            Department = "Engineering",
            DirectReports = new List<Employee>
            {
                new()
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    Department = "Engineering",
                    FirstName = "Debbie",
                    LastName = "Downer",
                    Position = "Lead Software Engineer",
                    DirectReports = new List<Employee>
                    {
                        new()
                        {
                            EmployeeId = Guid.NewGuid().ToString(),
                            Department = "Engineering",
                            FirstName = "Bart",
                            LastName = "Simpson",
                            Position = "Software Engineer I",
                        }
                    }
                },
                new()
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    Department = "Engineering",
                    FirstName = "Clark",
                    LastName = "Kent",
                    Position = "Senior Software Engineer",
                }
            }
        };
        
        _repositorySub.GetReportingStructure(Arg.Any<string>()).Returns(employee);
        
        // Act
        var reportingStructure = _sut.GetReportingStructure(employee.EmployeeId);

        // Assert
        reportingStructure.Should().NotBeNull();
        reportingStructure.Employee.Should().BeEquivalentTo(employee, options =>
        {
            options.Excluding(e => e.DirectReports);
            return options;
        });
        reportingStructure.NumberOfReports.Should().Be(expectedNumberOfReports);

        _repositorySub.GetReportingStructure(Arg.Is(employee.EmployeeId));
    }

    [Fact]
    public void GetReportingStructure_Returns_Null()
    {
        // Arrange
        var employeeId = Guid.NewGuid().ToString();
        _repositorySub.GetReportingStructure(Arg.Any<string>()).ReturnsNull();

        // Act
        var reportingStructure = _sut.GetReportingStructure(employeeId);

        // Assert
        reportingStructure.Should().BeNull();
        _repositorySub.GetReportingStructure(Arg.Is(employeeId));
    }

    [Fact]
    public void CreateCompensation_Returns_New_Compensation_When_Not_Null()
    {
        // Arrange
        var compensation = new Compensation
        {
            EmployeeId = Guid.NewGuid().ToString(),
            EffectiveDate = DateTime.Now,
            Salary = 123456.78M
        };
        
        var addedCompensation = new Compensation
        {
            CompensationId = Guid.NewGuid().ToString(),
            EmployeeId = compensation.EmployeeId,
            EffectiveDate = compensation.EffectiveDate,
            Salary = compensation.Salary
        };

        _repositorySub.AddCompensation(Arg.Any<Compensation>()).Returns(addedCompensation);
        _repositorySub.SaveAsync().Returns(Task.CompletedTask);
        
        // Act
        var newCompensation = _sut.CreateCompensation(compensation);
        
        // Assert
        newCompensation.Should().NotBeNull();
        _repositorySub.Received(1).AddCompensation(Arg.Is(compensation));
        _repositorySub.Received(1).SaveAsync();
        
        Received.InOrder(() =>
        {
            _repositorySub.AddCompensation(Arg.Is(compensation));
            _repositorySub.SaveAsync();
        });
    }
    
    [Fact]
    public void CreateCompensation_Returns_Null_When_Compensation_Is_Null()
    {
        // Arrange
        _repositorySub.AddCompensation(Arg.Any<Compensation>()).ReturnsNull();
        _repositorySub.SaveAsync().Returns(Task.CompletedTask);
        
        // Act
        var newCompensation = _sut.CreateCompensation(null);
        
        // Assert
        newCompensation.Should().BeNull();
        _repositorySub.DidNotReceive().AddCompensation(Arg.Any<Compensation>());
        _repositorySub.DidNotReceive().SaveAsync();
    }

    [Fact]
    public void GetCompensationByEmployeeId_Returns_Compensation()
    {
        // Arrange
        var employeeId = Guid.NewGuid().ToString();
        
        var expectedCompensation = new Compensation
        {
            CompensationId = Guid.NewGuid().ToString(),
            EffectiveDate = DateTime.Now,
            EmployeeId = employeeId,
            Salary = 12345.67M
        };

        _repositorySub.GetCompensationByEmployeeId(Arg.Any<string>()).Returns(expectedCompensation);
        
        // Act
        var compensation = _sut.GetCompensationByEmployeeId(employeeId);

        // Assert
        compensation.Should().NotBeNull().And.BeEquivalentTo(expectedCompensation);
        _repositorySub.Received(1).GetCompensationByEmployeeId(Arg.Is(employeeId));
    }
    
    [Fact]
    public void GetCompensationByEmployeeId_Returns_Null_When_EmployeeId_Is_Null()
    {
        // Arrange
        _repositorySub.GetCompensationByEmployeeId(Arg.Any<string>());

        // Act
        var compensation = _sut.GetCompensationByEmployeeId(null);
        
        // Assert
        compensation.Should().BeNull();
        _repositorySub.DidNotReceive().GetCompensationByEmployeeId(Arg.Any<string>());

    }
}