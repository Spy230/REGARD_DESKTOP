using System;
using System;
using System.Data.SqlClient; 
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace regard
{
    public partial class addSotrudniki : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True";

        public addSotrudniki()
        {
            this.Paint += YourForm_Paint; // Подписать на событие Paint формы
            this.MouseDown += YourForm_MouseDown; // Подписать на событие нажатия кнопки мыши
            this.MouseMove += YourForm_MouseMove;
            this.MouseUp += YourForm_MouseUp;
            InitializeComponent();
            FillComboBox(); //  комбобокс с должностями при инициализации формы
        }

        private void FillComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT vid_dolgnost FROM dolgnost;";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    guna2ComboBox1.Items.Clear();

                    while (reader.Read())
                    {
                        string vid_dolgnost = reader["vid_dolgnost"].ToString();
                        guna2ComboBox1.Items.Add(vid_dolgnost);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при заполнении комбобокса: " + ex.Message);
            }
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


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO sotrudniki (FIO, number, id_dolgnost)
                             VALUES (@FIO, @Number, (SELECT id_dolgnost FROM dolgnost WHERE vid_dolgnost = @VidDolgnost));";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FIO", guna2TextBox2.Text);
                    command.Parameters.AddWithValue("@Number", guna2TextBox1.Text);
                    command.Parameters.AddWithValue("@VidDolgnost", guna2ComboBox1.SelectedItem.ToString());
                    int rowsAffected = command.ExecuteNonQuery();

                    MessageBox.Show("Данные успешно сохранены. Количество затронутых строк: " + rowsAffected);

                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
            }
        }



    }
}



