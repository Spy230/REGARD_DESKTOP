using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace regard
{
    public partial class Adminis : Form
    {
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA\\SQLEXPRESS;Initial Catalog=regard;Integrated Security=True";
        private DataTable dataTable;
        private DataBase database = new DataBase();

        public Adminis()
        {
            InitializeComponent();
            LoadDataIntoDataGridView();
        }

        private void LoadDataIntoDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT [id_User] AS 'ID', 
                         [login_user] AS 'Логин', 
                         [password_user] AS 'Пароль', 
                         [is_admin] AS 'Администратор', 
                         [role] AS 'Должность',  -- Изменено на 'Должность'
                         [name] AS 'Имя', 
                         [last_name] AS 'Фамилия', 
                         [number] AS 'Телефон' 
                         FROM [regard].[dbo].[register]";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                guna2DataGridView1.DataSource = dataTable;


                if (guna2DataGridView1.Columns.Contains("ID"))
                {
                    guna2DataGridView1.Columns["ID"].Visible = false;
                }

                //  ComboBox для столбца "Должность"
                DataGridViewComboBoxColumn roleColumn = new DataGridViewComboBoxColumn();
                roleColumn.HeaderText = "";
                roleColumn.Name = "RoleComboBox";
                roleColumn.DataSource = new string[] { "AdminVendors", "AdminOrder", "admin", "Manager_Client", "AdminTovar", "HR-manager" };
                guna2DataGridView1.Columns.Add(roleColumn);
            }
        }

        private void UpdateDataInDatabase(int idSotrudnika, string selectedRole)
        {
            try
            {
                database.openConnection();

                // Обновление данных в базе данных
                string updateQuery = "UPDATE register SET role = @role WHERE id_User = @id_User";

                SqlCommand updateCommand = new SqlCommand(updateQuery, database.getConnection());
                updateCommand.Parameters.AddWithValue("@role", selectedRole);
                updateCommand.Parameters.AddWithValue("@id_User", idSotrudnika);

                int rowsAffected = updateCommand.ExecuteNonQuery();

                database.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при обновлении данных: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (guna2DataGridView1.SelectedCells.Count > 0)
                {
                    int selectedRowIndex = guna2DataGridView1.CurrentCell.RowIndex;
                    object idSotrudnikaObject = guna2DataGridView1.Rows[selectedRowIndex].Cells["ID"].Value;

                    //  является ли значение ячейки DBNull
                    if (idSotrudnikaObject != DBNull.Value && idSotrudnikaObject != null)
                    {
                        int idSotrudnika = Convert.ToInt32(idSotrudnikaObject);

                        string selectedRole = guna2DataGridView1.Rows[selectedRowIndex].Cells["RoleComboBox"].Value.ToString();

                        //  метод для сохранения изменений в базе данных
                        UpdateDataInDatabase(idSotrudnika, selectedRole);

                        // Обновление столбца "Role" в DataTable
                        dataTable.Rows[selectedRowIndex]["Должность"] = selectedRole;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: ID сотрудника не может быть DBNull", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}");
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedCells.Count > 0)
            {
                int selectedRowIndex = guna2DataGridView1.CurrentCell.RowIndex;
                int id = Convert.ToInt32(guna2DataGridView1.Rows[selectedRowIndex].Cells["id_user"].Value);

                database.openConnection();
                string deleteQuery = $"DELETE FROM register WHERE id_User = {id}";
                SqlCommand command = new SqlCommand(deleteQuery, database.getConnection());
                command.ExecuteNonQuery();
                database.closeConnection();
            }
        }




        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

