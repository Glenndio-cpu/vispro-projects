using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Database___CRUD___Crystal_Report
{
    public class Database
    {
        private MySqlConnection connection;
        private string connectionString;

        // Constructor: Set the connection string
        public Database(string server, string database, string username, string password)
        {
            connectionString = $"Server={server};Database={database};User={username};Password={password};";
            connection = new MySqlConnection(connectionString);
        }

        // Open the connection
        public void OpenConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        // Close the connection
        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        // Execute a SELECT query and return DataTable
        public DataTable ExecuteSelectQuery(string query)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OpenConnection();
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
            return dataTable;
        }

        // Execute a non-SELECT query (INSERT, UPDATE, DELETE)
        public int ExecuteNonQuery(string query)
        {
            int rowsAffected = 0;
            try
            {
                OpenConnection();
                MySqlCommand command = new MySqlCommand(query, connection);
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
            return rowsAffected;
        }
    }
}
