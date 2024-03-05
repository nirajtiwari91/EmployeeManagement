
// EmployeesController.cs
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")] // Enable CORS for the entire controller

    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeesController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            try
            {
                _context.Employee.Add(employee);
                await _context.SaveChangesAsync();

                // Retrieve the newly added employee with the role name
                var addedEmployee = await _context.Employee
                    .Include(e => e.Role) // Include Role navigation property
                    .OrderByDescending(e => e.EmployeeID)
                    .Where(e => e.EmployeeID == employee.EmployeeID)
                    .Select(e => new
                    {
                        e.EmployeeID,
                        e.EmployeeNumber,
                        e.FirstName,
                        e.LastName,
                        e.DateJoined,
                        e.Extension,
                        e.RoleID,
                        RoleName = e.Role != null ? e.Role.RoleName : null // Get RoleName from the Role navigation property
                    })
                    .FirstOrDefaultAsync();

                if (addedEmployee == null)
                    return NotFound("Employee not found after adding");

                return Ok(addedEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error adding employee: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetEmployees()
        {
            try
            {
                var employeesWithRoles = _context.Employee
                    .Include(e => e.Role) // Include Role navigation property
                    .OrderByDescending(e => e.EmployeeID)
                    .Select(e => new
                    {
                        e.EmployeeID,
                        e.EmployeeNumber,
                        e.FirstName,
                        e.LastName,
                        e.DateJoined,
                        e.Extension,
                        e.RoleID,
                        RoleName = e.Role != null ? e.Role.RoleName : null // Get RoleName from the Role navigation property
                    })
                    .ToList();

                return Ok(employeesWithRoles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error fetching employees: " + ex.Message);
            }
        }


        [HttpGet("search")]
        public IActionResult SearchEmployees(string searchTerm)
        {
            try
            {
                var employees = _context.Employee
                    .OrderByDescending(e => e.EmployeeID)
                    .Where(e => e.FirstName.Contains(searchTerm) || e.LastName.Contains(searchTerm))
                    .ToList();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error searching employees: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            try
            {
                var employee = await _context.Employee.FindAsync(id);
                if (employee == null)
                    return NotFound("Employee not found");

                _context.Employee.Remove(employee);
                await _context.SaveChangesAsync();
                return Ok("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error deleting employee: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(long id, Employee employee)
        {
            try
            {
                if (id != employee.EmployeeID)
                    return BadRequest("Employee ID mismatch");

                var existingEmployee = await _context.Employee.FindAsync(id);
                if (existingEmployee == null)
                    return NotFound("Employee not found");

                // Update existing employee properties
                existingEmployee.FirstName = employee.FirstName;
                existingEmployee.LastName = employee.LastName;
                existingEmployee.EmployeeNumber = employee.EmployeeNumber;
                existingEmployee.DateJoined = employee.DateJoined;
                existingEmployee.Extension = employee.Extension;
                existingEmployee.RoleID = employee.RoleID;

                await _context.SaveChangesAsync();

                // Retrieve the updated employee with the role name
                var updatedEmployee = _context.Employee
                    .Include(e => e.Role) // Include Role navigation property
                    .OrderByDescending(e => e.EmployeeID)
                    .Where(e => e.EmployeeID == id)
                    .Select(e => new
                    {
                        e.EmployeeID,
                        e.EmployeeNumber,
                        e.FirstName,
                        e.LastName,
                        e.DateJoined,
                        e.Extension,
                        e.RoleID,
                        RoleName = e.Role != null ? e.Role.RoleName : null // Get RoleName from the Role navigation property
                    })
                    .FirstOrDefault();

                if (updatedEmployee == null)
                    return NotFound("Employee not found after update");

                return Ok(updatedEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error updating employee: " + ex.Message);
            }
        }

        [HttpGet("getroles")]
        public IActionResult GetRoles()
        {
            try
            {
                var roles = _context.Role.ToList();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error fetching roles: " + ex.Message);
            }
        }
    }
}

