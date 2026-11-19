using System;

namespace School_Clinic_Patient_Record_System.Classes
{
    public static class SessionManager
    {
        public static int UserId { get; private set; }
        public static string FullName { get; private set; }
        public static string Role { get; private set; }
        public static bool IsAdmin { get { return String.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase); } }
        public static void Start(int id, string fullName, string role) { UserId = id; FullName = fullName; Role = role; }
        public static void End() { UserId = 0; FullName = null; Role = null; }
    }
}
