using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Employee
{
    public int IdEmployee { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public int? Age { get; set; }

    public string? Address { get; set; }

    public string? Cel { get; set; }

    public string? Email { get; set; }
}
