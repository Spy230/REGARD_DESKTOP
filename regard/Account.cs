using System;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace regard.Resources

{
    public partial class Account : Form
    {
        private string currentUserLogin;
        public static string CurrentUserLogin { get; set; }
        DataBase database = new DataBase();
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA\\SQLEXPRESS;Initial Catalog=regard;Integrated Security=True";
        public Account()
        {


            InitializeComponent();
        }


        private void Account_Load(object sender, EventArgs e)
        {
            //  ID текущего пользователя
            int userId = GetUserIdByLogin(authorization.CurrentUserLogin);

           
            UserData userData = GetUserDataById(userId);

            
            if (userData != null)
            {
                guna2TextBox3.Text = userData.Name;
                guna2TextBox4.Text = userData.LastName;
                guna2TextBox6.Text = userData.Number.ToString();
                guna2TextBox1.Text = userData.Login_user;
                guna2TextBox2.Text = userData.Password_user;
                guna2TextBox5.Text = userData.Role;
            }
        }


        private int GetUserIdByLogin(string loginUser)
        {
            int userId = -1;  

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = $"SELECT id_User FROM [register] WHERE login_user = @LoginUser";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@LoginUser", loginUser);

                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        userId = (int)result;  
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении ID пользователя: {ex.Message}");
                }
            }

            return userId;
        }

        private UserData GetUserDataById(int userId)
        {
            UserData userData = null;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = $"SELECT [name], [last_name], [number] , [login_user], [password_user] , [role] , [photo] FROM [register] WHERE id_User = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    { 
                        string name = reader.GetString(0);
                        string lastName = reader.GetString(1);
                        decimal number = reader.GetDecimal(2);  
                        string login_user = reader.GetString(3);
                        string password_user = reader.GetString(4);
                        string role = reader.GetString(5);
                       
                        userData = new UserData(name, lastName, number, login_user, password_user, role);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении данных пользователя: {ex.Message}");
                }
            }

            return userData;
        }

        public class UserData
        {
            public string Name { get; }
            public string LastName { get; }
            public decimal Number { get; }
            public string Login_user { get; }
            public string Password_user { get; }
            public string Role { get; }
            public byte[] Photo { get; set; }
            public UserData(string name, string lastName, decimal number, string login_user, string password_user, string role)
            {
                Name = name;
                LastName = lastName;
                Number = number;
                Login_user = login_user;
                Password_user = password_user;
                Role = role;
            }
        }

        private void GetUserByLogin(string loginUser)
        {



        }

 
        private void button5_Click(object sender, EventArgs e)
        {
            
            int userId = GetUserIdByLogin(authorization.CurrentUserLogin);

            // Обновление данных пользователя в базе данных 
            UpdateUserData(userId, guna2TextBox3.Text, guna2TextBox4.Text, decimal.Parse(guna2TextBox6.Text), guna2TextBox5.Text);

            MessageBox.Show("Данные успешно обновлены.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
             
            int userId = GetUserIdByLogin(authorization.CurrentUserLogin);

           
            UpdateUserData(userId, guna2TextBox1.Text, guna2TextBox2.Text);

            MessageBox.Show("Данные успешно обновлены.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateUserData(int userId, string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "UPDATE [register] SET login_user = @Login, password_user = @Password WHERE id_User = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении данных пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

            private void UpdateUserData(int userId, string name, string lastName, decimal number, string role)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "UPDATE [register] SET [name] = @Name, [last_name] = @LastName, [number] = @Number, [role] = @Role WHERE id_User = @UserId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Number", number);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@UserId", userId);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении данных пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }

    }


