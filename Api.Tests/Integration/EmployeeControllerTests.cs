using Microsoft.AspNetCore.Mvc;
using Moq;
using api.Controllers;
using api.Services.Employees;
using api.Dtos.Employee;

namespace Api.Tests.Integration;

public class EmployeeControllerTests
{
    private static EmployeeController MakeController(Mock<IEmployeeService> mock)
        => new EmployeeController(mock.Object);

    [Fact]
    public async Task GetAll_ReturnsOk_WithEmployees()
    {
        // Arrange
        var expected = new List<EmployeeOutputDto>
        {
            new EmployeeOutputDto { Name = "Ada", LastName = "Lovelace", Age = 28, Email = "ada@x.com" },
            new EmployeeOutputDto { Name = "Alan", LastName = "Turing",   Age = 30, Email = "alan@x.com" }
        };

        var mock = new Mock<IEmployeeService>();
        mock.Setup(s => s.AllEmployees()).ReturnsAsync(expected);

        var controller = MakeController(mock);

        // Act
        var result = await controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<List<EmployeeOutputDto>>(ok.Value);

        // Assert
        Assert.Equal(expected.Count, body.Count);
        mock.Verify(s => s.AllEmployees(), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithBody()
    {
        // Arrange
        var dto = new EmployeeCreateDto { Name = "Ada", LastName = "Lovelace", Age = 28, Email = "ada@x.com" };
        var created = new EmployeeOutputDto { Name = dto.Name, LastName = dto.LastName, Age = dto.Age, Email = dto.Email };

        var mock = new Mock<IEmployeeService>();
        mock.Setup(s => s.AddEmployee(dto)).ReturnsAsync(created);

        var controller = MakeController(mock);

        // Act
        var result = await controller.Create(dto);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

        // Assert
        Assert.Equal(nameof(EmployeeController.GetAll), createdResult.ActionName);
        var body = Assert.IsType<EmployeeOutputDto>(createdResult.Value);
        Assert.Equal(created.Email, body.Email);
        mock.Verify(s => s.AddEmployee(dto), Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_AndCallsService()
    {
        // Arrange
        var id = 5;
        var dto = new EmployeeUpdateDto { IdEmployee = id, Name = "N", LastName = "L", Age = 20, Email = "n@x.com" };

        var mock = new Mock<IEmployeeService>();
        mock.Setup(s => s.UpdateEmployee(id, dto)).Returns(Task.CompletedTask);

        var controller = MakeController(mock);

        // Act
        var result = await controller.Update(id, dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mock.Verify(s => s.UpdateEmployee(id, dto), Times.Once);
    }

    [Fact]
    public async Task Patch_ReturnsNoContent_AndCallsService()
    {
        // Arrange
        var id = 7;
        var dto = new EmployeePatchDto { Name = "OnlyName" };

        var mock = new Mock<IEmployeeService>();
        mock.Setup(s => s.UpdateEmployeePartial(id, dto)).Returns(Task.CompletedTask);

        var controller = MakeController(mock);

        // Act
        var result = await controller.Patch(id, dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mock.Verify(s => s.UpdateEmployeePartial(id, dto), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_AndCallsService()
    {
        // Arrange
        var id = 9;
        var mock = new Mock<IEmployeeService>();
        mock.Setup(s => s.DeleteEmployee(id)).Returns(Task.CompletedTask);

        var controller = MakeController(mock);

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mock.Verify(s => s.DeleteEmployee(id), Times.Once);
    }
}
