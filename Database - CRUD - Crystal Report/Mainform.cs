using System;
using System.Data;
using System.Windows.Forms;

namespace Database___CRUD___Crystal_Report
{
    public partial class Mainform : Form
    {
        private Database db;
        private int editingUserId = -1; // Variable to track if we're editing an existing user

        public Mainform()
        {
            InitializeComponent();
            db = new Database("localhost", "user_management", "root", "");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            // Load user data into the DataGridView
            LoadUserData();
            // Load roles into ComboBox
            LoadRoles();
            // Disable editing on DataGridView cells
            dataGridViewUsers.ReadOnly = true; // Set the DataGridView as read-only
        }

        private void LoadUserData()
        {
            string query = "SELECT u.id, u.name, u.email, u.age, u.role_id, r.role_name, u.password FROM users u JOIN roles r ON u.role_id = r.id";
            DataTable users = db.ExecuteSelectQuery(query);

            if (users == null || users.Rows.Count == 0)
            {
                MessageBox.Show("No data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridViewUsers.DataSource = users;
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            if (dataGridViewUsers.Columns.Contains("id"))
            {
                dataGridViewUsers.Columns["id"].HeaderText = "User ID";
                dataGridViewUsers.Columns["id"].Visible = false;
            }

            if (dataGridViewUsers.Columns.Contains("name"))
            {
                dataGridViewUsers.Columns["name"].HeaderText = "Name";
            }

            if (dataGridViewUsers.Columns.Contains("email"))
            {
                dataGridViewUsers.Columns["email"].HeaderText = "Email";
            }

            if (dataGridViewUsers.Columns.Contains("age"))
            {
                dataGridViewUsers.Columns["age"].HeaderText = "Age";
            }

            if (dataGridViewUsers.Columns.Contains("role_name"))
            {
                dataGridViewUsers.Columns["role_name"].HeaderText = "Role";
            }

            if (dataGridViewUsers.Columns.Contains("password"))
            {
                dataGridViewUsers.Columns["password"].Visible = false; // Hide password column
            }
        }

        private void LoadRoles()
        {
            // Load roles into ComboBox
            string query = "SELECT id, role_name FROM roles";
            DataTable roles = db.ExecuteSelectQuery(query);

            if (roles != null && roles.Rows.Count > 0)
            {
                cmbRole.DataSource = roles;
                cmbRole.DisplayMember = "role_name"; // Show role name in ComboBox
                cmbRole.ValueMember = "id"; // Store role ID in ComboBox
            }
        }

        private void dataGridViewUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewUsers.Rows[e.RowIndex];

                // Populate TextBox values from DataGridView row, but don't allow edits in DataGridView
                txtName.Text = row.Cells["name"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                txtAge.Text = row.Cells["age"].Value.ToString();

                // Ensure the role ID is being set correctly for ComboBox
                if (row.Cells["role_id"].Value != DBNull.Value)
                {
                    cmbRole.SelectedValue = row.Cells["role_id"].Value;
                }

                txtPassword.Text = row.Cells["password"].Value != DBNull.Value ? row.Cells["password"].Value.ToString() : string.Empty;
                editingUserId = row.Cells["id"].Value != DBNull.Value ? Convert.ToInt32(row.Cells["id"].Value) : -1;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtAge.Text) || cmbRole.SelectedIndex == -1 || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            string name = txtName.Text;
            string email = txtEmail.Text;

            // Validate age input before parsing
            int age;
            if (!int.TryParse(txtAge.Text, out age))
            {
                MessageBox.Show("Please enter a valid age.");
                return;
            }

            int roleId = (int)cmbRole.SelectedValue;
            string password = txtPassword.Text;

            if (editingUserId == -1)
            {
                string query = $"INSERT INTO users (name, email, age, role_id, password) VALUES ('{name}', '{email}', {age}, {roleId}, '{password}')";
                db.ExecuteNonQuery(query);
                MessageBox.Show("User added successfully.");
            }
            else
            {
                string query = $"UPDATE users SET name = '{name}', email = '{email}', age = {age}, role_id = {roleId}, password = '{password}' WHERE id = {editingUserId}";
                db.ExecuteNonQuery(query);
                MessageBox.Show("User updated successfully.");
            }

            LoadUserData();
            ClearInputs();
        }

        private void ClearInputs()
        {
            txtName.Clear();
            txtEmail.Clear();
            txtAge.Clear();
            cmbRole.SelectedIndex = -1; // Reset the ComboBox
            editingUserId = -1; // Reset editing mode
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dataGridViewUsers.SelectedRows[0].Cells["id"].Value);
                var confirmResult = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    string query = $"DELETE FROM users WHERE id={userId}";
                    db.ExecuteNonQuery(query);
                    LoadUserData(); // Reload user data after deletion
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.");
            }
        }
    }
}
