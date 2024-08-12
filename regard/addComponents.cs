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
    public partial class addComponents : Form
    {


        DataBase database = new DataBase();

        private bool mouseDown;
        private Point lastLocation;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True ";

        private order parentForm;
        public addComponents(order parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;
            this.FormClosing += addComponents_FormClosing;
            LoadStatuses();
            this.Paint += YourForm_Paint; // Подписать на событие Paint формы
            this.MouseDown += YourForm_MouseDown; // Подписать на событие нажатия кнопки мыши
            this.MouseMove += YourForm_MouseMove;
            this.MouseUp += YourForm_MouseUp;

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
            int curveSize = 20; // Размер закругления 

            path.AddArc(new Rectangle(0, 0, curveSize * 2, curveSize * 2), 180, 90);
            path.AddArc(new Rectangle(this.Width - curveSize * 2, 0, curveSize * 2, curveSize * 2), -90, 90);
            path.AddArc(new Rectangle(this.Width - curveSize * 2, this.Height - curveSize * 2, curveSize * 2, curveSize * 2), 0, 90);
            path.AddArc(new Rectangle(0, this.Height - curveSize * 2, curveSize * 2, curveSize * 2), 90, 90);
            this.Region = new Region(path);
        }

        private void LoadStatuses()
        {
            try
            {
                database.openConnection();
                string query = "SELECT status_tovara FROM status_tovara";
                SqlCommand command = new SqlCommand(query, database.getConnection());
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string status = reader["status_tovara"].ToString();
                    guna2ComboBox1.Items.Add(status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке статусов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.closeConnection();
            }
        }



        private void addComponents_FormClosing(object sender, FormClosingEventArgs e)
        {
            parentForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                database.openConnection();

                var name = guna2TextBox2.Text;
                var model = guna2TextBox3.Text;
                var price = guna2TextBox4.Text;
                var statusName = guna2ComboBox1.SelectedItem?.ToString();

                // идентификатор статуса по его имени
                int statusId = GetStatusId(statusName);

                var addQuerry = $"INSERT INTO [tovar] (name, model, price, id_statusa) VALUES ('{name}', '{model}', '{price}', '{statusId}')";

                var command = new SqlCommand(addQuerry, database.getConnection());

                command.ExecuteNonQuery();
                MessageBox.Show("Запись создана");

                parentForm.Showtovar();

                guna2TextBox2.Clear();
                guna2TextBox3.Clear();
                guna2TextBox4.Clear();
                guna2ComboBox1.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.closeConnection();
            }
        }

        private int GetStatusId(string statusName)
        {
            int statusId = -1;  

            try
            {
                database.openConnection();
                string query = "SELECT id_statusa FROM status_tovara WHERE status_tovara = @statusName";
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

        private void button2_Click(object sender, EventArgs e)
        {

            this.Close();
            parentForm.Show();
        }

     
        private void SaveImageToDatabase(string imagePath)
        {
            try
            {
                database.openConnection();

               
                byte[] imageBytes = File.ReadAllBytes(imagePath);

                
                SqlParameter imageParam = new SqlParameter("@photo", SqlDbType.VarBinary);
                imageParam.Value = imageBytes;

                // запрос на вставку изображения в базу данных
                string query = "INSERT INTO tovar (photo) VALUES (@photo)";
                SqlCommand command = new SqlCommand(query, database.getConnection());
                command.Parameters.Add(imageParam);

              
                command.ExecuteNonQuery();

              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении изображения в базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.closeConnection();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // диалоговое окно для выбора файла изображения
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //  путь к файлу
                string imagePath = openFileDialog.FileName;

                pictureBox1.Image = Image.FromFile(imagePath);

                
                SaveImageToDatabase(imagePath);
            }
        }
    }
}
