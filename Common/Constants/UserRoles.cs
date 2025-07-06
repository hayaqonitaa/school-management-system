namespace SchoolManagementSystem.Common.Constants
{
    public static class UserRoles
    {
        public const int Admin = 1;
        public const int Teacher = 2;
        public const int Student = 3;
        
        public static string GetRoleName(int roleId)
        {
            return roleId switch
            {
                Admin => "Admin",
                Teacher => "Teacher", 
                Student => "Student",
                _ => "Unknown"
            };
        }
        
        public static bool IsValidRole(int roleId)
        {
            return roleId >= Admin && roleId <= Student;
        }
    }
}
