using System; 
using System.Data.SqlClient;
using BCrypt.Net;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
 
 
namespace regard
{
    public partial class authorization : Form
    {

        public static string CurrentUserLogin { get; set; }
        DataBase database = new DataBase();



        public authorization()
        {
            InitializeComponent();

            textBox1.MouseEnter += textBox1_MouseEnter;
            textBox1.MouseLeave += textBox1_MouseLeave;
            textBox2.MouseEnter += textBox2_MouseEnter;
            textBox2.MouseLeave += textBox2_MouseLeave;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {
        }



        private void btnEnter_Click_Click(object sender, EventArgs e)
        {
            var loginUser = textBox1.Text;
            var passUser = textBox2.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            // Создаем запрос с использованием параметризации для безопасности
            string querystring = "SELECT id_User, login_user, password_user, is_admin, role FROM register WHERE login_user = @LoginUser";
            SqlCommand command = new SqlCommand(querystring, database.getConnection());
            command.Parameters.AddWithValue("@LoginUser", loginUser); // Добавляем параметр

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                DataRow userRow = table.Rows[0];
                bool isAdmin = Convert.ToBoolean(userRow["is_admin"]);

                if (isAdmin)
                {
                    // Вход для администратора без проверки пароля
                    MessageBox.Show("Вход выполнен успешно как администратор", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CurrentUserLogin = loginUser;
                    string role = userRow["role"].ToString();
                    checkUser currentUser = new checkUser(loginUser, true, role);
                    order ord = new order(currentUser);
                    this.Hide();
                    ord.ShowDialog();
                    this.Show();
                }
                else
                {
                    // Обычная проверка пароля для пользователей
                    string storedHashedPassword = userRow["password_user"].ToString();
                    if (BCrypt.Net.BCrypt.Verify(passUser, storedHashedPassword))
                    {
                        MessageBox.Show("Вход выполнен успешно", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CurrentUserLogin = loginUser;
                        string role = userRow["role"].ToString();
                        checkUser currentUser = new checkUser(loginUser, false, role);
                        order ord = new order(currentUser);
                        this.Hide();
                        ord.ShowDialog();
                        this.Show();
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Аккаунт не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private bool CheckUserRole(DataRow userRow)
        {

            string userRole = userRow.ItemArray[4].ToString();


            return userRole.Equals("admin", StringComparison.OrdinalIgnoreCase);
        }

        private checkUser GetCurrentUser(DataRow userRow)
        {

            string username = userRow.IsNull(1) ? null : userRow.ItemArray[1].ToString();
            bool isAdmin = userRow.IsNull(3) ? false : Convert.ToBoolean(userRow.ItemArray[3]);
            string userRole = userRow.IsNull(4) ? null : userRow.ItemArray[4].ToString();


            return new checkUser(username, isAdmin, userRole);
        }






        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }

      

        private void textBox2_MouseEnter(object sender, EventArgs e)
        {

            if (textBox2.Text == "password")
            {

                textBox2.Text = "";
            }
        }
        private void textBox2_MouseLeave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                // Если текстовое поле пустое,  текст "password"
                textBox2.Text = "password";
            }
        }


        private void textBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {

            if (textBox1.Text == "login")
            {

                textBox1.Text = "";
            }
        }
        private void textBox1_MouseLeave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {

                textBox1.Text = "login";
            }
        }


      

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = guna2CheckBox1.Checked ? '\0' : '*';
        }

        
    }
}





    






                
