using System.Data;

namespace EmployeeManagement
{
    public class Employee
    {
        public long EmployeeID { get; set; }
        public int EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateJoined { get; set; }
        public short? Extension { get; set; }
        public int? RoleID { get; set; }

        // Navigation property
        public Role? Role { get; set; }
    }

    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
