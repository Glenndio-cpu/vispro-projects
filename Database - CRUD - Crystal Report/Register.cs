using System;
using System.Data;
using System.Windows.Forms;

namespace Database___CRUD___Crystal_Report
{
    public partial class Register : Form
    {
        private Database db;
        private int userId;

        public Register()
        {
            InitializeComponent();
            db = new Database("localhost", "user_management", "root", "");
            LoadRoles();
        }

        // Constructor for editing an existing user
        public Register(int userId)
        {
            InitializeComponent();
            db = new Database("localhost", "user_management", "root", "");
            this.userId = userId; // Set userId for editing an existing user
            LoadRoles();
            if (userId > 0)
            {
                LoadUserData(); // Load existing user data
            }
        }

        private void LoadRoles()
        {
            string query = "SELECT id, role_name FROM roles WHERE role_name IN ('User', 'Guest')";
            DataTable roles = db.ExecuteSelectQuery(query);

            cmbRole.DataSource = roles;
            cmbRole.DisplayMember = "role_name";
            cmbRole.ValueMember = "id";
        }

        private void LoadUserData()
        {
            string query = $"SELECT * FROM users WHERE id={userId}";
            DataTable user = db.ExecuteSelectQuery(query);

            if (user.Rows.Count > 0)
            {
                txtName.Text = user.Rows[0]["name"].ToString();
                txtEmail.Text = user.Rows[0]["email"].ToString();
                txtPassword.Text = user.Rows[0]["password"].ToString(); // Display the password
                txtAge.Text = user.Rows[0]["age"].ToString();
                cmbRole.SelectedValue = user.Rows[0]["role_id"];
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide Register form
            Form1 loginForm = new Form1();

            loginForm.FormClosed += (s, args) => this.Close(); // Close Register when Form1 closes
            loginForm.Show();
        }

        private void btnRegist_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim(); // Use plain text password
            int age;

            // Try to parse the age entered by the user
            if (!int.TryParse(txtAge.Text, out age))
            {
                MessageBox.Show("Please enter a valid age.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int roleId = (int)cmbRole.SelectedValue;

            // Validate that required fields are not empty
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Name, email, and password cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Perform the database operation
            if (userId > 0)
            {
                // Update existing user
                string query = $"UPDATE users SET name='{name}', email='{email}', password='{password}', age={age}, role_id={roleId} WHERE id={userId}";
                db.ExecuteNonQuery(query);
                MessageBox.Show("User updated successfully.");
            }
            else
            {
                // Add new user
                string query = $"INSERT INTO users (name, email, password, age, role_id) VALUES ('{name}', '{email}', '{password}', {age}, {roleId})";
                db.ExecuteNonQuery(query);
                MessageBox.Show("User added successfully.");
            }

            DialogResult = DialogResult.OK; // Close the Register form
        }
    }
}
