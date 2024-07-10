using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
using System.Drawing.Drawing2D;


namespace regard
{
    public partial class addOrder : Form
    {

        DataBase database = new DataBase();
        private bool mouseDown;
        private Point lastLocation;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True ";
        private order parentForm;
        private readonly checkUser _user;
        private string selectedStatus;


        public addOrder(order parentForm, checkUser user)
        {

            InitializeComponent();
            this.Paint += YourForm_Paint; // Подписать на событие Paint формы
            this.MouseDown += YourForm_MouseDown; // Подписать на событие нажатия кнопки мыши
            this.MouseMove += YourForm_MouseMove;
            this.MouseUp += YourForm_MouseUp;
            this.parentForm = parentForm;
            this.FormClosing += addOrder_FormClosing;
            _user = user;

            Status.Items.Add("ожидает оплаты");
            Status.Items.Add("заказано");
            Status.Items.Add("оплачено");
            Status.Items.Add("отменено");
            Status.Items.Add("готов к упаковке");
            Status.Items.Add("упаковано");
            // Заполнение ComboBox данными из базы данных для клиентов
            DataTable clientsTable = new DataTable();
            SqlDataAdapter clientsAdapter = new SqlDataAdapter("SELECT id_client, name FROM Client", ConnectionString);
            clientsAdapter.Fill(clientsTable);
            guna2ComboBox1.DataSource = clientsTable;
            guna2ComboBox1.DisplayMember = "name"; 
            guna2ComboBox1.ValueMember = "id_client"; 



            DataTable vendorsTable = new DataTable();
            SqlDataAdapter vendorsAdapter = new SqlDataAdapter("SELECT id_vendors, name FROM vendors", ConnectionString);
            vendorsAdapter.Fill(vendorsTable);
            guna2ComboBox2.DataSource = vendorsTable;
            guna2ComboBox2.DisplayMember = "name"; 
            guna2ComboBox2.ValueMember = "id_vendors"; 

            DataTable tovarTable = new DataTable();
            SqlDataAdapter tovarAdapter = new SqlDataAdapter("SELECT id_tovar , model FROM tovar", ConnectionString);
            tovarAdapter.Fill(tovarTable);
            guna2ComboBox3.DataSource = tovarTable;
            guna2ComboBox3.DisplayMember = "model";
            guna2ComboBox3.ValueMember = "id_tovar";



            DataTable punktTable = new DataTable();
            SqlDataAdapter punktAdapter = new SqlDataAdapter("SELECT id_punkt , sposob_vidachi  FROM sposob_vidachi", ConnectionString);
            punktAdapter.Fill(punktTable);
            guna2ComboBox4.DataSource = punktTable;

            guna2ComboBox4.DisplayMember = "sposob_vidachi";
            guna2ComboBox4.ValueMember = "id_punkt";


            DataTable employeeTable = new DataTable();
            SqlDataAdapter employeeAdapter = new SqlDataAdapter("SELECT id_sotrudnika, FIO FROM sotrudniki", ConnectionString);
            employeeAdapter.Fill(employeeTable);

            guna2ComboBox5.DataSource = employeeTable;
            guna2ComboBox5.DisplayMember = "FIO";
            guna2ComboBox5.ValueMember = "id_sotrudnika";

            DataTable oplataTable = new DataTable();
            SqlDataAdapter oplataAdapter = new SqlDataAdapter("SELECT id_payment, method_name FROM payment", ConnectionString);
            oplataAdapter.Fill(oplataTable);
            guna2ComboBox6.DataSource = oplataTable;
            guna2ComboBox6.DisplayMember = "method_name";
            guna2ComboBox6.ValueMember = "id_payment";



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

        private void addOrder_FormClosing(object sender, FormClosingEventArgs e)
        {
            parentForm.Show();
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверка роли пользователя
            if (_user.Role.Equals("AdminVendors", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("У вас нет прав на добавление заказов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            database.openConnection();

            var id_client = GetClientId(guna2ComboBox1.Text);
            if (id_client == -1)
            {
                MessageBox.Show("Клиент не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                database.closeConnection();
                return;
            }

            var name_order = nazvanieZakaza.Text;
            var id_vendors = GetVendorId(guna2ComboBox2.Text);
            var order_date = date.Text;
            var id_payment = GetPaymentMethodId(guna2ComboBox6.Text);
            var status = selectedStatus; // статус заказа
            int id_punkt = GetPunktId(guna2ComboBox4.Text); //  идентификатор пункта выдачи
            int id_tovar = GettovarId(guna2ComboBox3.Text);
            int id_sotrudnika = GetEmployeeId(guna2ComboBox5.Text); //  идентификатор сотрудника

            var addQuery = $"INSERT INTO [order] (id_client, name_order, id_vendors, order_date, Status, id_punkt, id_tovar, id_sotrudnika, id_payment) " +
                $"VALUES ('{id_client}', '{name_order}', '{id_vendors}', '{order_date}', '{status}', '{id_punkt}', '{id_tovar}', '{id_sotrudnika}','{id_payment}') ";

            var command = new SqlCommand(addQuery, database.getConnection());
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Запись создана");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.closeConnection();
            }
        }

        private int GetPaymentMethodId(string methodName)
        {
            int paymentMethodId = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
            SELECT id_payment
            FROM payment
            WHERE method_name = @methodName";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@methodName", methodName);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    paymentMethodId = Convert.ToInt32(result);
                }
            }

            return paymentMethodId;
        }


        private int GetClientId(string name)
        {
            int clientId = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
            SELECT Client.id_client 
            FROM Client 
            LEFT JOIN [order] ON Client.id_client = [order].id_client
            WHERE Client.name = @name";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    clientId = int.Parse(result.ToString());
                }
            }

            return clientId;
        }
        private int GetPunktId(string sposob_vidachi)
        {
            int punktId = 1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
            SELECT pv.id_punkt
            FROM sposob_vidachi pv
            WHERE pv.sposob_vidachi = @sposob_vidachi";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@sposob_vidachi", sposob_vidachi);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    punktId = int.Parse(result.ToString());
                }
            }

            return punktId;
        }

        private int GetEmployeeId(string employeeName)
        {
            int employeeId = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
            SELECT s.id_sotrudnika
            FROM sotrudniki s
            WHERE s.FIO = @employeeName";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@employeeName", employeeName);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    employeeId = Convert.ToInt32(result);
                }
            }

            return employeeId;
        }


        private int GetVendorId(string vendorName)
        {
            int vendorId = 1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT id_vendors FROM vendors WHERE name = @vendorName";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@vendorName", vendorName);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    vendorId = int.Parse(result.ToString());
                }
            }

            return vendorId;
        }


        private int GettovarId(string tovarModel)
        {
            int tovarId = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT id_tovar FROM tovar WHERE model = @tovarmodel";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tovarmodel", tovarModel);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    tovarId = Convert.ToInt32(result);
                }
            }

            return tovarId;
        }


        private void Status_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedStatus = Status.SelectedItem?.ToString();
        }




        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            parentForm.Show();
        }

       
    }
}
     

    
