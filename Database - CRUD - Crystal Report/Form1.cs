using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Database___CRUD___Crystal_Report
{
    public partial class Form1 : Form
    {
        private Database db;
        public Form1()
        {
            InitializeComponent();
            db = new Database("localhost", "user_management", "root", "");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRegist_Click(object sender, EventArgs e)
        {
            Register registerForm = new Register();
            registerForm.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Query to validate user credentials
            string query = $"SELECT * FROM users WHERE email = '{email}' AND password = '{password}'";

            DataTable result = db.ExecuteSelectQuery(query);

            if (result.Rows.Count > 0)
            {
                // Login successful
                string name = result.Rows[0]["name"].ToString();
                string roleId = result.Rows[0]["role_id"].ToString();
                MessageBox.Show($"Welcome, {name}!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open main form or dashboard
                Form1 mainForm = new Form1();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                // Login failed
                MessageBox.Show("Invalid email or password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
