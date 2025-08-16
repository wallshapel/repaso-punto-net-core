using api.Mapping;
using api.Models;
using api.Dtos.Employee;

namespace Api.Tests;

public class ReflectionObjectMapperTests
{
    private readonly IObjectMapper _mapper = new ReflectionObjectMapper();

    // Auxiliares para probar conversiones y enums
    private enum Role { User = 0, Admin = 1 }

    private class ConvSource
    {
        public string? Name { get; set; }
        public string? Age { get; set; }          // string -> int?
        public string? Role { get; set; }         // string -> enum
    }

    private class ConvDest
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public Role Role { get; set; }
    }

    [Fact]
    public void Map_CreatesNewInstance_Maps_ByName_CaseInsensitive()
    {
        var src = new EmployeeCreateDto
        {
            Name = "Ada",
            LastName = "Lovelace",
            Age = 28,
            Address = "Somewhere",
            Cel = "3110000000",
            Email = "ada@x.com"
        };

        // Ignoramos IdEmployee en el destino Employee (no existe en CreateDto).
        var dest = _mapper.Map<EmployeeCreateDto, Employee>(src, "IdEmployee");

        Assert.Equal(src.Name, dest.Name);
        Assert.Equal(src.LastName, dest.LastName);
        Assert.Equal(src.Age, dest.Age);
        Assert.Equal(src.Address, dest.Address);
        Assert.Equal(src.Cel, dest.Cel);
        Assert.Equal(src.Email, dest.Email);
        // IdEmployee no se toca (default 0)
        Assert.Equal(0, dest.IdEmployee);
    }

    [Fact]
    public void MapInto_IgnoreNullsTrue_DoesNotOverwrite_WithNulls()
    {
        var patch = new EmployeePatchDto
        {
            Name = "NewName",
            Email = null // no debe sobrescribir si ignoreNulls = true
        };

        var entity = new Employee
        {
            IdEmployee = 10,
            Name = "OldName",
            Email = "old@x.com"
        };

        _mapper.MapInto(patch, entity, ignoreNulls: true, "IdEmployee");

        Assert.Equal("NewName", entity.Name);
        // email permanece igual porque la fuente es null y ignoreNulls = true
        Assert.Equal("old@x.com", entity.Email);
        // Id ignorado
        Assert.Equal(10, entity.IdEmployee);
    }

    [Fact]
    public void MapInto_IgnoreDestProps_DoesNotTouch_IgnoredMembers()
    {
        var dto = new EmployeeUpdateDto
        {
            IdEmployee = 99,            // debería ignorarse
            Name = "N",
            LastName = "L",
            Email = "n@x.com",
            Age = 20
        };

        var entity = new Employee { IdEmployee = 5 };

        _mapper.MapInto(dto, entity, ignoreNulls: false, "IdEmployee");

        // Id debe conservarse
        Assert.Equal(5, entity.IdEmployee);
        // Resto sí debe mapearse
        Assert.Equal("N", entity.Name);
        Assert.Equal("L", entity.LastName);
        Assert.Equal(20, entity.Age);
        Assert.Equal("n@x.com", entity.Email);
    }

    [Fact]
    public void Map_Converts_StringToNullableInt_And_StringToEnum()
    {
        var src = new ConvSource
        {
            Name = "Alice",
            Age = "42",         // debe convertirse a int?
            Role = "Admin"      // debe convertirse a enum (case-insensitive)
        };

        var dest = _mapper.Map<ConvSource, ConvDest>(src);

        Assert.Equal("Alice", dest.Name);
        Assert.Equal(42, dest.Age);
        Assert.Equal(Role.Admin, dest.Role);
    }

    [Fact]
    public void MapInto_AllowNullAssignment_WhenIgnoreNullsFalse()
    {
        // Si ignoreNulls = false, un null de la fuente debe sobreescribir a destino
        var src = new EmployeePatchDto
        {
            Address = null, // explícitamente null
            Name = "X"
        };

        var dest = new Employee
        {
            Name = "Old",
            Address = "WillBeNull"
        };

        _mapper.MapInto(src, dest, ignoreNulls: false, "IdEmployee");

        Assert.Equal("X", dest.Name);
        Assert.Null(dest.Address); // fue sobreescrito por null
    }
}
