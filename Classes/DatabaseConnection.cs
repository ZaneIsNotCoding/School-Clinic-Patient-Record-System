using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace School_Clinic_Patient_Record_System.Classes
{
    public static class DatabaseConnection
    {
        public static MySqlConnection Open()
        {
            var setting = ConfigurationManager.ConnectionStrings["ClinicDb"];
            if (setting == null) throw new InvalidOperationException("Database connection is missing. Set it in App.config.");
            var connection = new MySqlConnection(setting.ConnectionString);
            connection.Open();
            return connection;
        }
        public static bool CanConnect(out string message)
        {
            try { using (var connection = Open()) { message = "Connected to school_clinic_db."; return true; } }
            catch (Exception ex) { message = ex.Message; return false; }
        }
        public static int Execute(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = Open()) using (var command = new MySqlCommand(sql, connection))
            { if (parameters != null) command.Parameters.AddRange(parameters); return command.ExecuteNonQuery(); }
        }
        public static object Scalar(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = Open()) using (var command = new MySqlCommand(sql, connection))
            { if (parameters != null) command.Parameters.AddRange(parameters); return command.ExecuteScalar(); }
        }
        public static System.Data.DataTable Query(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = Open()) using (var command = new MySqlCommand(sql, connection))
            {
                if (parameters != null) command.Parameters.AddRange(parameters);
                using (var adapter = new MySqlDataAdapter(command)) { var table = new System.Data.DataTable(); adapter.Fill(table); return table; }
            }
        }
        public static MySqlParameter P(string name, object value) { return new MySqlParameter(name, value ?? DBNull.Value); }
    }
}
