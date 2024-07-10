using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Drawing.Drawing2D;

namespace regard
{
    public partial class AddClient : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True";
        private string selectedStatus;
        DataBase database = new DataBase();
        private Image selectedImage;

        public AddClient()
        {
            this.Paint += YourForm_Paint; // Подписать на событие Paint формы
            this.MouseDown += YourForm_MouseDown; // Подписать на событие нажатия кнопки мыши
            this.MouseMove += YourForm_MouseMove;
            this.MouseUp += YourForm_MouseUp;
            InitializeComponent();
            guna2ComboBox1.Items.Add("Мужской");
            guna2ComboBox1.Items.Add("Женский");
            LoadClientTypes();
            LoadCustomerStatus();
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



        private void cansel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void LoadCustomerStatus()
        {
            try
            {
                database.openConnection();
                string query = "SELECT vid_statusa FROM Custumer_Status";
                SqlCommand command = new SqlCommand(query, database.getConnection());
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string status = reader["vid_statusa"].ToString();
                    guna2ComboBox2.Items.Add(status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке статусов клиентов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.closeConnection();
            }
        }

        private void LoadClientTypes()
        {
            try
            {
                database.openConnection();
                string query = "SELECT type FROM type_client";
                SqlCommand command = new SqlCommand(query, database.getConnection());
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader["type"].ToString();
                    guna2ComboBox3.Items.Add(type);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке типов клиентов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.closeConnection();
            }
        }



        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                database.openConnection();

                var name = guna2TextBox1.Text;
                var contact_number = guna2TextBox2.Text;
                var email = guna2TextBox3.Text;
                var address = guna2TextBox4.Text;
                var gender = guna2ComboBox1.SelectedItem.ToString();
                var customerStatus = guna2ComboBox2.SelectedItem.ToString();
                var type = guna2ComboBox3.SelectedItem.ToString(); // Получаем выбранный тип клиента
                var birthDate = guna2TextBox5.Text;

                string addQuery;
                SqlCommand command;

                if (selectedImage != null)
                {
                    addQuery = "INSERT INTO [Client] (name, contact_number, email, address, photo, gender, id_status, id_type, birth_date) VALUES (@name, @contact_number, @email, @address, @photo, @gender, @id_status, @id_type, @birth_date)";
                    command = new SqlCommand(addQuery, database.getConnection());
                    command.Parameters.AddWithValue("@photo", ImageToByteArray(selectedImage));
                }
                else
                {
                    addQuery = "INSERT INTO [Client] (name, contact_number, email, address, gender, id_status, id_type, birth_date) VALUES (@name, @contact_number, @email, @address, @gender, @id_status, @id_type, @birth_date)";
                    command = new SqlCommand(addQuery, database.getConnection());
                }

                int statusId = GetStatusId(customerStatus);

                int typeId = GetTypeId(type); // Получаем идентификатор выбранного типа клиента

                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@contact_number", contact_number);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@gender", gender);
                command.Parameters.AddWithValue("@id_status", statusId);
                command.Parameters.AddWithValue("@id_type", typeId); // Используем полученный идентификатор типа клиента
                command.Parameters.AddWithValue("@birth_date", birthDate);

                command.ExecuteNonQuery();
                MessageBox.Show("Запись создана");

                guna2TextBox1.Text = "";
                guna2TextBox2.Text = "";
                guna2TextBox3.Text = "";
                guna2TextBox4.Text = "";
                selectedImage = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить информацию о клиенте: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (database.getConnection().State == ConnectionState.Open)
                {
                    database.closeConnection();
                }
            }
        }


        private int GetStatusId(string statusName)
        {
            int statusId = -1; // Значение по умолчанию, если не удастся найти соответствующий id_status

            try
            {
                database.openConnection();
                string query = "SELECT id_status FROM Custumer_Status WHERE vid_statusa = @statusName";
                SqlCommand command = new SqlCommand(query, database.getConnection());
                command.Parameters.AddWithValue("@statusName", statusName);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    statusId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении id_status: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return statusId;
        }
        private int GetTypeId(string typeName)
        {
            int typeId = -1; // Значение по умолчанию, если не удастся найти соответствующий id_type

            try
            {
                database.openConnection();
                string query = "SELECT id_type FROM type_client WHERE type = @typeName";
                SqlCommand command = new SqlCommand(query, database.getConnection());
                command.Parameters.AddWithValue("@typeName", typeName);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    typeId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении id_type: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return typeId;
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //метод загрузки изображения 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*GIF;*.PNG) |*.BMP;*.JPG;*.GIF;*.PNG | All files(*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    selectedImage = Image.FromFile(openFileDialog.FileName);
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedStatus = guna2ComboBox1.SelectedItem?.ToString();
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedStatus = guna2ComboBox2.SelectedItem?.ToString();
        }

        private void guna2ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedStatus = guna2ComboBox3.SelectedItem?.ToString();
        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void AddClient_Load(object sender, EventArgs e)
        {

        }
    }
}
