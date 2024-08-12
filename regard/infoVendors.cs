using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms; 
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace regard
{
    public partial class infoVendors : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private order _orderForm;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True";
        private int id_vendors;  
        public infoVendors(int id_vendors, string name, string number, string contact_name, string email)
        {
            InitializeComponent();
            this.id_vendors = id_vendors;
            this.FormBorderStyle = FormBorderStyle.None; // Установить стиль рамки None
            this.BackColor = Color.White; // Задать цвет фона формы (можно изменить на нужный)
            this.Paint += YourForm_Paint; // Подписать на событие Paint формы
            this.MouseDown += InfoVendors_MouseDown; // Подписать на событие нажатия кнопки мыши
            this.MouseMove += InfoVendors_MouseMove;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT name, number, contact_name, email FROM vendors WHERE id_vendors = @id_vendors";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_vendors", id_vendors);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    guna2TextBox1.Text = reader["name"].ToString();
                    guna2TextBox2.Text = reader["number"].ToString();
                    guna2TextBox3.Text = reader["contact_name"].ToString();
                    guna2TextBox4.Text = reader["email"].ToString();
                    guna2TextBox5.Text = id_vendors.ToString();
                    reader.Close();
                }
            }


        }
        private void InfoVendors_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
        }

        private void InfoVendors_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && e.Button == MouseButtons.Left)
            {
                int dx = e.Location.X - lastLocation.X;
                int dy = e.Location.Y - lastLocation.Y;
                this.Location = new Point(this.Location.X + dx, this.Location.Y + dy);
            }
        }

        private void InfoVendors_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }


        private void YourForm_Paint(object sender, PaintEventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            int curveSize = 20; // Размер закругления (можно изменить на нужный)

            path.AddArc(new Rectangle(0, 0, curveSize * 2, curveSize * 2), 180, 90);
            path.AddArc(new Rectangle(this.Width - curveSize * 2, 0, curveSize * 2, curveSize * 2), -90, 90);
            path.AddArc(new Rectangle(this.Width - curveSize * 2, this.Height - curveSize * 2, curveSize * 2, curveSize * 2), 0, 90);
            path.AddArc(new Rectangle(0, this.Height - curveSize * 2, curveSize * 2, curveSize * 2), 90, 90);
            this.Region = new Region(path);
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            string name = guna2TextBox1.Text;
            string number = guna2TextBox2.Text;
            string contact_name = guna2TextBox3.Text;
            string email = guna2TextBox4.Text;

            
            if (id_vendors != 0)  
            {
                // Обновление данных в базе данных
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE vendors SET name = @name, number = @number, contact_name = @contact_name, email = @email WHERE id_vendors = @id_vendors";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@number", number);
                    command.Parameters.AddWithValue("@contact_name", contact_name);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@id_vendors", id_vendors);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные успешно обновлены.");
                        // Перезагрузка данных в DataGridView
                        ((order)this.Owner).ShowVendors();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении данных.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Ошибка: id_vendors не был инициализирован.");
            }
        }
        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
