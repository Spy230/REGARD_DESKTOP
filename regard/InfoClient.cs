using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;

namespace regard
{


    public partial class InfoClient : Form
    {
        public InfoClient()
        {
            this.Paint += YourForm_Paint; // Подписать на событие Paint формы
            this.MouseDown += YourForm_MouseDown; // Подписать на событие нажатия кнопки мыши
            this.MouseMove += YourForm_MouseMove;
            this.MouseUp += YourForm_MouseUp;
            InitializeComponent();
            FillComboBox();
        }
        private bool mouseDown;
        private Point lastLocation;
        private string id_client;
        private string id_status;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA\\SQLEXPRESS;Initial Catalog=regard;Integrated Security=True ";
        public void FillData(string id_client, string name, string contactNumber, string email, string gender, string id_status, string id_type, string address, string birthDate)
        {
            textBox6.Text = id_client;
            textBox1.Text = name;
            textBox2.Text = contactNumber;
            textBox3.Text = email;
            textBox8.Text = gender;
            guna2ComboBox1.Text = id_type;
            textBox4.Text = address;
            guna2ComboBox2.Text = id_status;
            textBox10.Text = birthDate;

            if (!guna2ComboBox1.Items.Contains(id_status))
            {
                guna2ComboBox1.Items.Add(id_status);
            }
            guna2ComboBox1.Text = id_status;



            if (!guna2ComboBox2.Items.Contains(id_type))
            {

                guna2ComboBox2.Items.Add(id_type);
            }
            guna2ComboBox2.Text = id_type;
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
        private void YourForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
        }

        private void YourForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && e.Button == MouseButtons.Left)
            {
                int dx = e.Location.X - lastLocation.X;
                int dy = e.Location.Y - lastLocation.Y;
                this.Location = new Point(this.Location.X + dx, this.Location.Y + dy);
            }
        }

        private void YourForm_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }


        private void FillComboBox()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT DISTINCT cs.vid_statusa
                 FROM Client c
                 INNER JOIN Custumer_Status cs ON c.id_status = c.id_status";


                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                // Очистка всех существующих элементов в комбобоксе перед добавлением новых
                guna2ComboBox1.Items.Clear();

                while (reader.Read())
                {
                    string id_status = reader["vid_statusa"].ToString();
                    guna2ComboBox1.Items.Add(id_status);
                }
                reader.Close();

                string query2 = @"SELECT DISTINCT tc.type
                  FROM Client c
                  INNER JOIN type_Client tc ON c.id_type = c.id_type";

                SqlCommand command2 = new SqlCommand(query2, connection);
                SqlDataReader reader2 = command2.ExecuteReader();


                guna2ComboBox2.Items.Clear();

                while (reader2.Read())
                {
                    string type = reader2["type"].ToString(); // Исправлено на "type"
                    guna2ComboBox2.Items.Add(type);
                }
                reader2.Close();

            }
        }


        public void ShowImage(byte[] imageBytes)
        {
            if (imageBytes != null && imageBytes.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            }
            else
            {

                pictureBox1.Image = null;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

            string id_client = textBox6.Text;
            string name = textBox1.Text;
            string contactNumber = textBox2.Text;
            string email = textBox3.Text;
            string gender = textBox8.Text;
            string id_status = guna2ComboBox1.Text;
            string id_type = guna2ComboBox2.Text;
            string address = textBox4.Text;
            string birthDate = textBox10.Text;

            //  идентификатор статуса и типа клиента
            int statusId = GetStatusId(id_status);
            int typeId = GetTypeId(id_type);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = @"UPDATE Client SET 
                name = @name,
                contact_number = @contact_number,
                email = @email,
                gender = @gender,
                id_status = @statusId,
                id_type = @typeId,
                address = @address,
                birth_date = @birth_date
                WHERE id_client = @id_client";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_client", id_client);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@contact_number", contactNumber);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@gender", gender);
                command.Parameters.AddWithValue("@statusId", statusId);
                command.Parameters.AddWithValue("@typeId", typeId);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@birth_date", birthDate);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Данные успешно сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось сохранить данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int GetStatusId(string selectedStatus)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT id_status FROM Custumer_Status WHERE vid_statusa = @selectedStatus";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@selectedStatus", selectedStatus);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }


        private int GetTypeId(string selectedType)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT id_type FROM Type_Client WHERE type = @selectedType";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@selectedType", selectedType);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
