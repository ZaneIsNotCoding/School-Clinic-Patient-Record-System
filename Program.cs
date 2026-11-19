using System;
using System.Windows.Forms;
using School_Clinic_Patient_Record_System.Classes;
using School_Clinic_Patient_Record_System.Forms;

namespace School_Clinic_Patient_Record_System
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
