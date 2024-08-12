using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using GSF.Net.Smtp; 
using Word = Microsoft.Office.Interop.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace regard
{
    public partial class InfoOrder : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private readonly string fail = @"C:\Users\shest\OneDrive\Рабочий стол\form\otchet2.docx";
        private static InfoOrder instance;
        private order _orderForm;
        private int id_order;
        private string id_client;
        private string selectedComboBox2Item;  
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True";
        public InfoOrder(int orderId, string clientId, string orderName, string employeeId)
        {
            InitializeComponent();
             
            id_order = orderId;
            id_client = clientId;
            this.FormBorderStyle = FormBorderStyle.None; // Установить стиль рамки None
            LoadCustomerStatus();
            this.Paint += YourForm_Paint; // Подписать на событие Paint формы
            this.MouseDown += InfoOrder_MouseDown;
            this.MouseMove += InfoOrder_MouseMove;
            this.MouseUp += InfoOrder_MouseUp;
            this.FormClosed += InfoOrder_FormClosed;
            if (instance == null || instance.IsDisposed)
            {
                instance = this;
            }
             
            guna2ComboBox2.Items.AddRange(new string[] { "ожидает оплаты", "заказано", "оплачено", "отменено", "готов к упаковке", "упаковано" });

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT o.id_order, o.id_client, o.name_order, o.id_vendors, o.order_date, o.Status, o.id_tovar, s.FIO AS 'сотрудники' FROM [order] o INNER JOIN sotrudniki s ON o.id_sotrudnika = s.id_sotrudnika WHERE o.id_order = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    guna2TextBox1.Text = reader["id_order"].ToString();
                    guna2TextBox2.Text = reader["name_order"].ToString();
                    guna2TextBox7.Text = reader["order_date"].ToString();
                    guna2TextBox9.Text = reader["сотрудники"].ToString();
                    string status = reader["Status"].ToString();
                    if (guna2ComboBox2.Items.Contains(status))
                    {
                        guna2ComboBox2.SelectedItem = status;
                    }

                    int vendorsId = int.Parse(reader["id_vendors"].ToString());
                    int tovarId = int.Parse(reader["id_tovar"].ToString());
                    reader.Close();

                    query = @"SELECT Client.id_client, Client.name AS 'Клиент', Client.contact_number, Client.address, Client.email, Client.id_status FROM [order]  INNER JOIN Client on [order].id_client = Client.id_client WHERE Client.name = @name;";

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", clientId);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        guna2TextBox5.Text = reader["address"].ToString();
                        guna2TextBox15.Text = reader["Клиент"].ToString();
                        guna2TextBox16.Text = reader["contact_number"].ToString();
                        guna2TextBox17.Text = reader["address"].ToString();
                        guna2TextBox18.Text = reader["email"].ToString();
                        guna2ComboBox2.Text = reader["id_status"].ToString();
                         guna2TextBox8.Text = clientId; 

                        string customerStatus = reader["id_status"].ToString();
                        if (guna2ComboBox1.Items.Contains(customerStatus))
                        {
                            guna2ComboBox1.SelectedItem = customerStatus;
                        }
                    }
                    reader.Close();

                    query = "SELECT name, number, contact_name, email FROM vendors WHERE id_vendors = @VendorsId";
                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@VendorsId", vendorsId);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        guna2TextBox11.Text = reader["name"].ToString();
                        guna2TextBox12.Text = reader["number"].ToString();
                        guna2TextBox13.Text = reader["contact_name"].ToString();
                        guna2TextBox14.Text = reader["email"].ToString();
                    }
                    reader.Close();

                    query = "SELECT model, price FROM tovar WHERE id_tovar = @tovarId";
                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tovarId", tovarId);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        guna2TextBox3.Text = reader["model"].ToString();
                        guna2TextBox4.Text = reader["price"].ToString();
                        guna2TextBox6.Text = reader["price"].ToString();
                    }
                    reader.Close();
                }
            }
        }
        private void InfoOrder_Load(object sender, EventArgs e)
        {
        }

        private void FillComboBox2()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT DISTINCT Status FROM [order]"; // Извлекаем уникальные статусы из таблицы заказов
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        guna2ComboBox2.Items.Add(reader["Status"].ToString()); // Добавляем каждый статус в комбобокс
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при заполнении комбобокса: " + ex.Message);
                }
            }
        }
        private void LoadCustomerStatus()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT vid_statusa FROM Custumer_Status";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string status = reader["vid_statusa"].ToString();
                        guna2ComboBox1.Items.Add(status);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке статусов клиентов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private int GetStatusId(string statusName)
        {
            int statusId = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT id_status FROM Custumer_Status WHERE vid_statusa = @statusName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@statusName", statusName);
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        statusId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении id_status: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return statusId;
        }


       
        private void InfoOrder_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
        }

        private void InfoOrder_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && e.Button == MouseButtons.Left)
            {
                int dx = e.Location.X - lastLocation.X;
                int dy = e.Location.Y - lastLocation.Y;
                this.Location = new Point(this.Location.X + dx, this.Location.Y + dy);
            }
        }

        private void InfoOrder_MouseUp(object sender, MouseEventArgs e)
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

        public static InfoOrder GetInstance(int orderId, string clientId, string orderName, string employeeId)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new InfoOrder(orderId, clientId, orderName, employeeId);
            }
            return instance;
        }


        private void InfoOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }






        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string neworderStatus = selectedComboBox2Item;
            string newAddress = guna2TextBox5.Text;
            string newid_status = guna2ComboBox1.SelectedItem?.ToString();
            string newOrderStatus = guna2ComboBox2.SelectedItem?.ToString();
            string id_client = guna2TextBox8.Text;
            string newName = guna2TextBox15.Text;
            int rowsAffectedClient = 0;
            int rowsAffectedOrder = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string clientQuery = "UPDATE Client SET name = @Name, address = @Address, id_status = @id_status WHERE name = @Name";
                SqlCommand clientCommand = new SqlCommand(clientQuery, connection);
                clientCommand.Parameters.AddWithValue("@Address", newAddress);
                clientCommand.Parameters.AddWithValue("@id_status", (object)newid_status ?? DBNull.Value);
                clientCommand.Parameters.AddWithValue("@Name", newName);
                rowsAffectedClient = clientCommand.ExecuteNonQuery();
            }

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string orderQuery = "UPDATE [order] SET Status = @OrderStatus WHERE id_order = @id_order";
                SqlCommand orderCommand = new SqlCommand(orderQuery, connection);
                orderCommand.Parameters.AddWithValue("@OrderStatus", (object)newOrderStatus ?? DBNull.Value);
                orderCommand.Parameters.AddWithValue("@id_order", id_order);

                rowsAffectedOrder = orderCommand.ExecuteNonQuery();
            }

            if (rowsAffectedClient > 0 || rowsAffectedOrder > 0)
            {
                MessageBox.Show("Данные успешно обновлены.");
                if (this.Owner != null)
                {
                    ((order)this.Owner).ShowOrders();
                }
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении данных.");
            }
        }


        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedComboBox2Item = guna2ComboBox2.SelectedItem?.ToString();
            selectedComboBox2Item = guna2ComboBox2.SelectedItem?.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            var orderName = guna2TextBox2.Text;
            var dataZakaza = guna2TextBox7.Text;
            var sposobVidachi = guna2TextBox10.Text;
            var summa = guna2TextBox4.Text;
            var contact_name = guna2TextBox13.Text;
            var number = guna2TextBox11.Text;
            var email = guna2TextBox14.Text;

            var Client_name = guna2TextBox15.Text;
            var contact_number = guna2TextBox16.Text;
            var Client_email = guna2TextBox18.Text;
            var wordap = new Word.Application();
            wordap.Visible = false;

            var worddoc = wordap.Documents.Open(@"C:\Users\shest\OneDrive\Рабочий стол\form\otchet2.docx");

            // Замена метки в документе на значения из текстовых полей
            ReplaceWord("{nazvanie zakaza}", orderName, worddoc);
            ReplaceWord("{data zakaza}", dataZakaza, worddoc);
            ReplaceWord("{sposob_vidachi}", sposobVidachi, worddoc);
            ReplaceWord("{summa}", summa, worddoc);
            ReplaceWord("{summa1}", summa, worddoc);
            ReplaceWord("{summa3}", summa, worddoc);
            ReplaceWord("{Fio}", contact_name, worddoc);
            ReplaceWord("{telefon}", number, worddoc);
            ReplaceWord("{pochta}", email, worddoc);
            ReplaceWord("{Fio pokupatela}", Client_name, worddoc);
            ReplaceWord("{telefon pokupatela}", contact_number, worddoc);
            ReplaceWord("{pochta pokupatela}", Client_email, worddoc);


            worddoc.SaveAs(@"C:\Users\shest\Downloads\otchet.docx");
            worddoc.Close();
            MessageBox.Show("Отчет сохранен");
        }



        private void ReplaceWord(string stubreplace, string text, Word.Document worddoc)
        {
            var range = worddoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubreplace, ReplaceWith: text);
        }

        private void guna2TextBox8_TextChanged(object sender, EventArgs e)
        {


        }

    }

}

