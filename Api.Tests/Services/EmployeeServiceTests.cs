using System.Linq.Expressions;
using Moq;
using api.Services.Employees;
using api.Repositories.Employee;
using api.Mapping;
using api.Dtos.Employee;
using api.Models;
using api.Exceptions;

namespace Api.Tests;

public class EmployeeServiceTests
{
    private static (EmployeeService svc, Mock<IEmployeeRepository> repo, Mock<IObjectMapper> mapper)
        MakeService()
    {
        var repo = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var mapper = new Mock<IObjectMapper>(MockBehavior.Strict);
        var svc = new EmployeeService(repo.Object, mapper.Object);
        return (svc, repo, mapper);
    }

    [Fact]
    public async Task AllEmployees_DelegatesToProjectAsync_AndReturnsList()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();
        var expected = new List<EmployeeOutputDto>
        {
            new EmployeeOutputDto { Name = "Ada", LastName = "Lovelace", Age = 28, Email = "ada@x.com" },
            new EmployeeOutputDto { Name = "Alan", LastName = "Turing",   Age = 30, Email = "alan@x.com" }
        };

        repo.Setup(r => r.ProjectAsync(
                It.IsAny<Expression<Func<Employee, EmployeeOutputDto>>>(),
                false))
            .ReturnsAsync(expected);

        // Act
        var result = await svc.AllEmployees();

        // Assert
        Assert.Equal(expected.Count, result.Count);
        repo.Verify(r => r.ProjectAsync(
            It.IsAny<Expression<Func<Employee, EmployeeOutputDto>>>(),
            false), Times.Once);
        mapper.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AddEmployee_Maps_Adds_Saves_AndReturnsOutputDto()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();

        var create = new EmployeeCreateDto
        {
            Name = "Ada",
            LastName = "Lovelace",
            Age = 28,
            Email = "ada@x.com"
        };
        var entity = new Employee
        {
            IdEmployee = 1,
            Name = "Ada",
            LastName = "Lovelace",
            Age = 28,
            Email = "ada@x.com"
        };
        var output = new EmployeeOutputDto
        {
            Name = entity.Name,
            LastName = entity.LastName,
            Age = entity.Age,
            Email = entity.Email
        };

        mapper.Setup(m => m.Map<EmployeeCreateDto, Employee>(create)).Returns(entity);
        repo.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        mapper.Setup(m => m.Map<Employee, EmployeeOutputDto>(entity)).Returns(output);

        // Act
        var result = await svc.AddEmployee(create);

        // Assert
        Assert.Equal(output.Email, result.Email);
        mapper.Verify(m => m.Map<EmployeeCreateDto, Employee>(create), Times.Once);
        repo.Verify(r => r.AddAsync(entity), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        mapper.Verify(m => m.Map<Employee, EmployeeOutputDto>(entity), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployee_WhenRouteAndBodyIdMismatch_ThrowsArgumentException()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();
        var idRoute = 2;
        var dto = new EmployeeUpdateDto { IdEmployee = 1, Name = "N", LastName = "L", Email = "n@x.com" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => svc.UpdateEmployee(idRoute, dto));

        repo.VerifyNoOtherCalls();
        mapper.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateEmployee_WhenFound_MapsIntoAndSaves()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();
        var id = 5;
        var dto = new EmployeeUpdateDto { IdEmployee = id, Name = "N", LastName = "L", Email = "n@x.com", Age = 20 };
        var entity = new Employee { IdEmployee = id, Name = "Old", LastName = "Old", Email = "old@x.com", Age = 10 };

        repo.Setup(r => r.GetByIdAsync(id, true)).ReturnsAsync(entity);
        mapper.Setup(m => m.MapInto(dto, entity, false, "IdEmployee"));
        repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await svc.UpdateEmployee(id, dto);

        // Assert
        repo.Verify(r => r.GetByIdAsync(id, true), Times.Once);
        mapper.Verify(m => m.MapInto(dto, entity, false, "IdEmployee"), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task UpdateEmployee_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();
        var id = 10;
        var dto = new EmployeeUpdateDto { IdEmployee = id, Name = "N", LastName = "L", Email = "n@x.com" };

        repo.Setup(r => r.GetByIdAsync(id, true)).ReturnsAsync((Employee?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => svc.UpdateEmployee(id, dto));

        repo.Verify(r => r.GetByIdAsync(id, true), Times.Once);
        mapper.VerifyNoOtherCalls();
        repo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateEmployeePartial_WhenFound_MapsIntoIgnoreNullsTrue_AndSaves()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();
        var id = 7;
        var patch = new EmployeePatchDto { Name = "OnlyName" };
        var entity = new Employee { IdEmployee = id, Name = "Old", Email = "old@x.com" };

        repo.Setup(r => r.GetByIdAsync(id, true)).ReturnsAsync(entity);
        mapper.Setup(m => m.MapInto(patch, entity, true, "IdEmployee"));
        repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await svc.UpdateEmployeePartial(id, patch);

        // Assert
        repo.Verify(r => r.GetByIdAsync(id, true), Times.Once);
        mapper.Verify(m => m.MapInto(patch, entity, true, "IdEmployee"), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployeePartial_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();
        var id = 8;
        var patch = new EmployeePatchDto { Name = "X" };

        repo.Setup(r => r.GetByIdAsync(id, true)).ReturnsAsync((Employee?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => svc.UpdateEmployeePartial(id, patch));

        repo.Verify(r => r.GetByIdAsync(id, true), Times.Once);
        mapper.VerifyNoOtherCalls();
        repo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteEmployee_WhenFound_RemovesAndSaves()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();
        var id = 9;
        var entity = new Employee { IdEmployee = id };

        repo.Setup(r => r.GetByIdAsync(id, true)).ReturnsAsync(entity);
        repo.Setup(r => r.Remove(entity));
        repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await svc.DeleteEmployee(id);

        // Assert
        repo.Verify(r => r.GetByIdAsync(id, true), Times.Once);
        repo.Verify(r => r.Remove(entity), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        mapper.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteEmployee_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var (svc, repo, mapper) = MakeService();
        var id = 11;

        repo.Setup(r => r.GetByIdAsync(id, true)).ReturnsAsync((Employee?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => svc.DeleteEmployee(id));

        repo.Verify(r => r.GetByIdAsync(id, true), Times.Once);
        repo.Verify(r => r.Remove(It.IsAny<Employee>()), Times.Never);
        repo.Verify(r => r.SaveChangesAsync(), Times.Never);
        mapper.VerifyNoOtherCalls();
    }
}
