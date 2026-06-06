
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


async Task WriteResponse(HttpContext context)
{
    var path = context.Request.Path;
    var method = context.Request.Method;
    
    if(path.StartsWithSegments("/employees") && method.Equals("GET"))
    {
        EmployeeRepository.AddDefaultInMemoryEmployees();
        var employees = EmployeeRepository.GetAllEmployees();
        var json = JsonSerializer.Serialize(employees);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync(json);
    }

    if(path.StartsWithSegments("/employees") && method.Equals("POST"))
    {
        var reader =new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        var employee = JsonSerializer.Deserialize<Employee>(body);
        EmployeeRepository.AddEmployee(employee);
        context.Response.StatusCode = 201;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Employee added successfully");
    }

}

app.Run(WriteResponse);
app.Run();



class EmployeeRepository
{

    private static List<Employee> _employees = new List<Employee>();

    private static bool inMemoryDataAdded = false;
   
    public static List<Employee> GetAllEmployees()
    {
        return _employees;
    }

    public static void AddDefaultInMemoryEmployees()
    {
        if(inMemoryDataAdded)
        {
            return;
        }

        Employee emp1 = new Employee();
        emp1.Id = 1;
        emp1.Name = "John Doe";
        Employee emp2 = new Employee();
        emp2.Id = 2;
        emp2.Name = "Jane Smith";
        _employees.Add(emp1);
        _employees.Add(emp2);
        inMemoryDataAdded = true;
    }

    public static void AddEmployee(Employee employee)
    {
        _employees.Add(employee);
    }
    
}

public class Employee
{
    public int Id { get; set; }
    public  string Name { get; set; }
    
}