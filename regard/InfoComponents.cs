using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace regard
{
    public partial class InfoComponents : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private order _orderForm;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True";
        private int id_tovar;
        private string id_statusa;
        public InfoComponents(int id_tovar, string name, string model, decimal price, string id_statusa)
        {
            InitializeComponent();
            this.id_tovar = id_tovar;
            this.id_statusa = id_statusa;
            this.Paint += YourForm_Paint; // Подписать на событие Paint формы
            this.MouseDown += YourForm_MouseDown; // Подписать на событие нажатия кнопки мыши
            this.MouseMove += YourForm_MouseMove;
            this.MouseUp += YourForm_MouseUp;
            FillComboBox();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT name, model, price, id_statusa, photo FROM tovar WHERE id_tovar = @id_tovar";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_tovar", id_tovar);

                byte[] photo = null; // Переменная для хранения данных изображения

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    guna2TextBox1.Text = reader["name"].ToString();
                    guna2TextBox2.Text = reader["model"].ToString();
                    guna2TextBox3.Text = reader["price"].ToString();

                    guna2TextBox5.Text = id_tovar.ToString();

                    // Сохраняем данные о фотографии
                    if (reader["photo"] != DBNull.Value)
                    {
                        photo = (byte[])reader["photo"];
                    }
                }
                else
                {
                    MessageBox.Show("Компонент с указанным идентификатором не найден.");
                    this.Close(); // Закрыть форму, так как данные не были загружены
                }

                // Закрываем reader
                reader.Close();

                // Отображаем изображение, если оно было загружено из базы данных
                if (photo != null)
                {
                    using (MemoryStream ms = new MemoryStream(photo))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
            }
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



        private void InfoComponents_Load(object sender, EventArgs e)
        {
            guna2ComboBox1.SelectedItem = id_statusa;
           
        }



        private void FillComboBox()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT status_tovara FROM status_tovara";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string status_tovara = reader["status_tovara"].ToString();
                    guna2ComboBox1.Items.Add(status_tovara); // Добавляем только статус, без id
                }
                reader.Close();
            }
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

        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Получение новых значений из текстовых полей
            string name = guna2TextBox1.Text;
            string model = guna2TextBox2.Text;
            decimal price = decimal.Parse(guna2TextBox3.Text); // Явное преобразование строки в decimal

            // Получение выбранного значения из комбобокса
            string selectedStatus = guna2ComboBox1.SelectedItem.ToString();

            // Получение соответствующего id_statusa
            string id_statusa = GetIdStatusa(selectedStatus);

            // Проверка на null перед использованием id_vendors
            if (id_tovar != 0) // Или любое другое подходящее значение по умолчанию для id_vendors
            {
                // Обновление данных в базе данных
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE tovar SET name = @name, model = @model, price = @price, id_statusa = @id_statusa WHERE id_tovar = @id_tovar";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@model", model);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@id_statusa", id_statusa);
                    command.Parameters.AddWithValue("@id_tovar", id_tovar);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные успешно обновлены.");
                        // Перезагрузка данных в DataGridView
                        if (this.Owner != null)
                        {
                            ((order)this.Owner).Showtovar();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении данных.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Ошибка: id_components не был инициализирован.");
            }
        }


        private string GetIdStatusa(string selectedStatus)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT id_statusa FROM status_tovara WHERE status_tovara = @status_tovara";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status_tovara", selectedStatus);
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : string.Empty;
            }
        }


        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void SaveImageToDatabase(string imagePath)
        {
            try
            {
                // Читаем содержимое изображения в байтовый массив
                byte[] imageBytes = File.ReadAllBytes(imagePath);

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE tovar SET photo = @photo WHERE id_tovar = @id_tovar";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@photo", imageBytes);
                    command.Parameters.AddWithValue("@id_tovar", id_tovar);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Изображение сохранено в базе данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при сохранении изображения в базе данных.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении изображения в базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            // Отображаем диалоговое окно для выбора файла изображения
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Получаем путь к выбранному файлу
                string imagePath = openFileDialog.FileName;

                // Отображаем выбранное изображение в pictureBox1
                pictureBox1.Image = Image.FromFile(imagePath);

                // Сохраняем выбранное изображение в базе данных
                SaveImageToDatabase(imagePath);
            }
        }
    }
}