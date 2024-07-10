using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Linq;
using regard.Resources;
using Guna.UI2.WinForms;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.Drawing.Printing;


using System.Xml.Linq;
using System.IO;
using System.Reflection.Metadata;
namespace regard
{

    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }

    public partial class order : Form
    {
        private InfoOrder infoOrderForm;
        private Button activeButton;
        private Form selectedForm = null;
        private authorization form1Instance;
        private readonly checkUser _user;
        private Dictionary<string, List<TextBox>> textBoxesByTable = new Dictionary<string, List<TextBox>>();
        private ComboBox userComboBox;
        DataBase database = new DataBase();
        private int selectedRow;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True";
        private string selectedTable = string.Empty;
        private int selectedRowIndex;
        private string selectedStatus;
        public order(checkUser user)
        {
            InitializeComponent();
            InitializeTextBoxes();
            InitializeBox();
            _user = user;

            comboBox2.Items.Add("Аккаунт");
            comboBox2.Items.Add("выйти");
        }

        private void InitializeBox()
        {

        }

        private void IsAdmin()
        {
            //проверка админа 
            управлениеToolStripMenuItem.Enabled = _user.IsAdmin;

        }

        private void AdminOrder()
        {


        }

        private void ConfigureUIForRole(string role)
        {


        }

        private void order_Load(object sender, EventArgs e)
        {
            ShowOrders();
            IsAdmin();
            AdminOrder();
            guna2DataGridView1.DataError += guna2DataGridView1_DataError;
            textBox1.Text = $"{_user.Login}: {_user.Status} ";
            ConfigureUIForRole(_user.Role);
            guna2DataGridView1.CellDoubleClick += guna2DataGridView1_CellDoubleClick;
            guna2DataGridView1.CellContentClick += guna2DataGridView1_CellContentClick;
            // Скрытие колонки с id
            guna2DataGridView1.Columns["id_order"].Visible = false;
            guna2DataGridView1.AllowUserToResizeColumns = true;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }



        public class DatabaseHelper
        {

            //проверка роли 
            private string connectionString = "Data Source=DESKTOP-BBVLBHA\\SQLEXPRESS;Initial Catalog=regard;Integrated Security=True";

            public string GetUserRole(string username)
            {
                string role = "DefaultRole";  // Роль по умолчанию

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Role FROM register WHERE login_user = @Username";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            role = result.ToString();
                        }
                    }
                }

                return role;
            }
        }





        public void ShowOrders()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
            SELECT o.id_order, o.name_order AS 'Название заказа', t.model AS 'Товар', 
                o.Status AS 'Статус', c.name AS 'Клиент', v.name AS 'Поставщик', 
                o.order_date AS 'Дата создания', pv.sposob_vidachi AS 'Доставка', 
                s.FIO AS 'Сотрудник', pm.method_name AS 'Способ оплаты'
            FROM [order] o
            INNER JOIN vendors v ON o.id_vendors = v.id_vendors
            INNER JOIN Client c ON o.id_client = c.id_client
            INNER JOIN sposob_vidachi pv ON o.id_punkt = pv.id_punkt
            INNER JOIN sotrudniki s ON o.id_sotrudnika = s.id_sotrudnika
            INNER JOIN tovar t ON o.id_tovar = t.id_tovar
            INNER JOIN payment pm ON o.id_payment = pm.id_payment"; // INNER JOIN с таблицей способов оплаты

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                guna2DataGridView1.DataSource = dataTable;

                // кнопка "Подробнее", 
                if (!guna2DataGridView1.Columns.Contains("ViewDetails"))
                {
                    DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                    buttonColumn.Name = "ViewDetails";
                    buttonColumn.HeaderText = "";
                    buttonColumn.Text = "Подробнее";
                    buttonColumn.UseColumnTextForButtonValue = true;

                    guna2DataGridView1.Columns.Add(buttonColumn);
                }

                guna2DataGridView1.Columns["id_order"].Visible = false;

                guna2DataGridView1.Columns["ViewDetails"].DisplayIndex = guna2DataGridView1.Columns.Count - 1;
            }
        }



        private void guna2DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (activeButton == button3)
            {
                if (_user.Role == "admin" || _user.Role == "AdminVendors")
                {
                    // Показ формы InfoVendors
                    if (e.RowIndex >= 0)
                    {
                        // Получение данных о курьере из выбранной строки
                        DataGridViewRow selectedRow = guna2DataGridView1.Rows[e.RowIndex];
                        int vendorsId = Convert.ToInt32(selectedRow.Cells["id_vendors"].Value);
                        string vendorsName = Convert.ToString(selectedRow.Cells["Название"].Value);
                        string vendorsNumber = Convert.ToString(selectedRow.Cells["номер"].Value);
                        string contactName = Convert.ToString(selectedRow.Cells["Имя курьера"].Value);
                        string vendorsEmail = Convert.ToString(selectedRow.Cells["Почта"].Value);


                        infoVendors infoVendorsForm = new infoVendors(vendorsId, vendorsName, vendorsNumber, contactName, vendorsEmail);
                        infoVendorsForm.Owner = this;

                        // Подписка на событие закрытия формы
                        infoVendorsForm.FormClosed += (s, args) =>
                        {
                            ((Form)s).Dispose(); // Освобождение ресурсов
                        };

                        infoVendorsForm.ShowDialog();
                    }
                }
                else
                {
                    // Пользователь не имеет прав для просмотра информации о курьерах
                    MessageBox.Show("Недостаточно прав для просмотра информации о курьерах.");
                }
            }
            else if (activeButton == button2)
            {
                if (_user.Role == "admin" || _user.Role == "AdminOrder")
                {
                    // Показ формы InfoOrder
                    if (e.RowIndex >= 0)
                    {
                        if (guna2DataGridView1.Columns.Contains("id_order") &&
                            guna2DataGridView1.Columns.Contains("Клиент") &&
                            guna2DataGridView1.Columns.Contains("название заказа"))
                        {
                            // Получение данных о заказе из выбранной строки
                            DataGridViewRow selectedRow = guna2DataGridView1.Rows[e.RowIndex];
                            int orderId = Convert.ToInt32(selectedRow.Cells["id_order"].Value);
                            string clientId = Convert.ToString(selectedRow.Cells["Клиент"].Value);
                            string orderName = Convert.ToString(selectedRow.Cells["название заказа"].Value);
                            string employeeId = Convert.ToString(selectedRow.Cells["сотрудник"].Value);


                            InfoOrder infoOrderForm = new InfoOrder(orderId, clientId, orderName, employeeId);
                            infoOrderForm.Owner = this;
                            // Подписка на событие закрытия формы
                            infoOrderForm.FormClosed += (s, args) =>
                            {
                                ((Form)s).Dispose(); // Освобождение ресурсов
                            };

                            infoOrderForm.ShowDialog();
                        }
                        else
                        {
                            // Обработка ситуации, когда не все необходимые столбцы присутствуют в DataGridView
                            MessageBox.Show("Необходимые столбцы отсутствуют в DataGridView.");
                        }
                    }
                }
                else
                {
                    // Пользователь не имеет прав для просмотра информации о заказах
                    MessageBox.Show("Недостаточно прав для просмотра информации о заказах.");
                }
            }
            else if (activeButton == button4)
            {
                if (_user.Role == "admin" || _user.Role == "AdminTovar")
                {
                    if (e.RowIndex >= 0)
                    {
                        if (guna2DataGridView1.Columns.Contains("id_tovar") &&
                            guna2DataGridView1.Columns.Contains("Название") &&
                            guna2DataGridView1.Columns.Contains("Модель") &&
                            guna2DataGridView1.Columns.Contains("Цена") &&
                            guna2DataGridView1.Columns.Contains("Статус"))
                        {
                            // Получение данных о компоненте из выбранной строки
                            DataGridViewRow selectedRow = guna2DataGridView1.Rows[e.RowIndex];
                            int id_tovar = Convert.ToInt32(selectedRow.Cells["id_tovar"].Value);
                            string name = Convert.ToString(selectedRow.Cells["Название"].Value);
                            string model = Convert.ToString(selectedRow.Cells["Модель"].Value);
                            decimal price = Convert.ToDecimal(selectedRow.Cells["Цена"].Value);
                            string id_statusa = Convert.ToString(selectedRow.Cells["Статус"].Value); // Изменено на string

                            // Создание и отображение формы InfoComponents
                            InfoComponents infoComponentsForm = new InfoComponents(id_tovar, name, model, price, id_statusa);
                            infoComponentsForm.ShowDialog();
                            // Заполнение ComboBox после отображения формы InfoComponents

                            // Подписка на событие закрытия формы
                            infoComponentsForm.FormClosed += (s, args) =>
                            {
                                ((Form)s).Dispose(); // Освобождение ресурсов
                            };
                        }
                        else
                        {
                            // Обработка ситуации, когда не все необходимые столбцы присутствуют в DataGridView
                            MessageBox.Show("Необходимые столбцы отсутствуют в DataGridView.");
                        }
                    }
                }
                else
                {
                    // Пользователь не имеет прав для просмотра информации о товарах
                    MessageBox.Show("Недостаточно прав для просмотра информации о товарах.");
                }
            }
        }







        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void ClearControls()
        {
            // Очистка элементов на панели

            foreach (System.Windows.Forms.Control control in Controls.OfType<System.Windows.Forms.Label>().ToList())
            {
                Controls.Remove(control);
                control.Dispose();
            }

            foreach (System.Windows.Forms.Control control in Controls.OfType<System.Windows.Forms.TextBox>().ToList())
            {
                Controls.Remove(control);
                control.Dispose();
            }
        }



        public void ShowVendors()
        {
            //показ Vendors
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT id_vendors, name AS 'Название', number AS 'номер', contact_name AS 'Имя курьера', email AS 'Почта' FROM vendors";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);


                if (!guna2DataGridView1.Columns.Contains("ViewDetails"))
                {
                    DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                    buttonColumn.Name = "ViewDetails";
                    buttonColumn.HeaderText = "";
                    buttonColumn.Text = "Подробнее";
                    buttonColumn.UseColumnTextForButtonValue = true;

                    //  столбец кнопки в DataGridView
                    guna2DataGridView1.Columns.Add(buttonColumn);
                }

                //  источник данных после добавления столбца с кнопкой
                guna2DataGridView1.DataSource = dataTable;

                // Скрываем первую колонку с идентификатором id_vendors
                guna2DataGridView1.Columns["id_vendors"].Visible = false;

                //  индекс отображения столбца кнопки "Подробнее" в конец
                guna2DataGridView1.Columns["ViewDetails"].DisplayIndex = guna2DataGridView1.Columns.Count - 1;
            }
        }


        public void AddViewDetailsButtonColumn()
        {
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.Name = "ViewDetails";
            buttonColumn.HeaderText = "";
            buttonColumn.Text = "Подробнее";
            buttonColumn.UseColumnTextForButtonValue = true;


            guna2DataGridView1.Columns.Add(buttonColumn);
            guna2DataGridView1.Columns["ViewDetails"].DisplayIndex = guna2DataGridView1.Columns.Count - 1;
        }


        public void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == guna2DataGridView1.Columns["ViewDetails"].Index && e.RowIndex >= 0)
            {

                DataRowView rowView = (DataRowView)guna2DataGridView1.Rows[e.RowIndex].DataBoundItem;
                DataRow row = rowView.Row;

                if (row.Table.Columns.Contains("id_order") &&
                    row.Table.Columns.Contains("Клиент") &&
                    row.Table.Columns.Contains("название заказа"))
                {
                    //данные о заказе из выбранной строки
                    int orderId = Convert.ToInt32(row["id_order"]);
                    string clientId = Convert.ToString(row["Клиент"]);

                    string orderName = Convert.ToString(row["название заказа"]);
                    string employeeId = Convert.ToString(row["сотрудник"]);

                    if (Application.OpenForms.OfType<InfoOrder>().Any())
                    {

                    }
                    else
                    {

                        InfoOrder infoOrderForm = new InfoOrder(orderId, clientId, orderName, employeeId);
                        infoOrderForm.FormClosed += InfoOrder_FormClosed;
                        infoOrderForm.Show();
                    }
                }
                else if (row.Table.Columns.Contains("id_tovar") &&
            row.Table.Columns.Contains("Название") &&
            row.Table.Columns.Contains("Модель") &&
            row.Table.Columns.Contains("Цена") &&
            row.Table.Columns.Contains("Статус"))

                {
                    int id_tovar = Convert.ToInt32(row["id_tovar"]);
                    string name = Convert.ToString(row["Название"]);
                    string model = Convert.ToString(row["Модель"]);
                    decimal price = Convert.ToDecimal(row["Цена"]);
                    string id_statusa = Convert.ToString(row["Статус"]);

                    if (Application.OpenForms.OfType<InfoComponents>().Any())
                    {

                    }
                    else
                    {
                        InfoComponents infoComponentsForm = new InfoComponents(id_tovar, name, model, price, id_statusa);
                        infoComponentsForm.FormClosed += InfoComponents_FormClosed;
                        infoComponentsForm.Show();

                    }
                }
                else
                {
                    MessageBox.Show("Необходимые столбцы отсутствуют в DataGridView.");
                }
            }
        }
        private void InfoOrder_FormClosed(object sender, FormClosedEventArgs e)
        {

            ((Form)sender).Dispose();
        }

        private void InfoComponents_FormClosed(object sender, FormClosedEventArgs e)
        {

            ((Form)sender).Dispose();
        }




        private void guna2DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

            e.Cancel = true;
        }


        public void Showtovar()
        {
            // Показ товаров
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
        SELECT t.id_tovar, t.name AS 'Название', t.model AS 'Модель', t.price AS 'Цена', s.status_tovara AS 'Статус' , photo AS 'Фото'
        FROM tovar t
        INNER JOIN status_tovara s ON t.id_statusa = s.id_statusa";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (!guna2DataGridView1.Columns.Contains("ViewDetails"))
                {
                    DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                    buttonColumn.Name = "ViewDetails";
                    buttonColumn.HeaderText = "";
                    buttonColumn.Text = "Подробнее";
                    buttonColumn.UseColumnTextForButtonValue = true;
                    guna2DataGridView1.Columns.Add(buttonColumn);
                }

                guna2DataGridView1.DataSource = dataTable;
                guna2DataGridView1.Columns["id_tovar"].Visible = false;
                guna2DataGridView1.Columns["ViewDetails"].DisplayIndex = guna2DataGridView1.Columns.Count - 1;
            }
        }




        private void del_Click(object sender, EventArgs e)
        {
            // Проверка, выбрана ли строка для удаления
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                // Подтверждение удаления записи
                DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Если пользователь подтвердил удаление
                if (result == DialogResult.Yes)
                {
                    // Получение значения первичного ключа выбранной записи
                    int primaryKeyToDelete = 0;
                    string tableName = GetSelectedTable(); // Получаем имя выбранной таблицы

                    // Имя столбца с первичным ключом может различаться для разных таблиц,
                    // поэтому проверяем имя таблицы и выбираем соответствующий столбец
                    switch (tableName)
                    {
                        case "vendors":
                            primaryKeyToDelete = Convert.ToInt32(guna2DataGridView1.SelectedRows[0].Cells["id_vendors"].Value);
                            break;
                        case "order":
                            primaryKeyToDelete = Convert.ToInt32(guna2DataGridView1.SelectedRows[0].Cells["id_order"].Value);
                            break;
                        case "tovar":
                            primaryKeyToDelete = Convert.ToInt32(guna2DataGridView1.SelectedRows[0].Cells["id_tovar"].Value);
                            break;
                        default:
                            // Для обработки других таблиц, добавьте соответствующие случаи
                            break;
                    }

                    // Формирование запроса на удаление
                    string deleteQuery = GetDeleteQuery(tableName, primaryKeyToDelete);

                    // Установка соединения с базой данных и выполнение запроса на удаление
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            command.ExecuteNonQuery(); // Выполнение запроса на удаление
                        }
                    }

                    // Обновление отображаемых данных в DataGridView
                    ShowDataForSelectedTable();

                    // Обновление DataGridView
                    RefreshDataGridView();
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private string GetDeleteQuery(string tableName, int primaryKeyToDelete)
        {
            return $"DELETE FROM [{tableName}] WHERE id_{tableName} = {primaryKeyToDelete}";
        }




        private void ShowDataForSelectedTable()
        {                                                    //отображение данных для таблицы 
            string selectedTable = GetSelectedTable();

            switch (selectedTable)
            {
                case "order":
                    ShowOrders();
                    break;
                case "vendors":
                    ShowVendors();
                    break;
                case "tovar":
                    Showtovar();
                    break;
                default:
                    break;
            }
        }





        private string GetSelectedTable()
        {
            return selectedTable;
        }






        private void button4_Click(object sender, EventArgs e)
        {
            Showtovar();
            ClearControls();
            selectedTable = "tovar";
            button4.Enabled = true;
            activeButton = (Button)sender;

            // Проверка роли текущего пользователя
            if (_user.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) || _user.Role.Equals("AdminTovar", StringComparison.OrdinalIgnoreCase))
            {
                if (selectedForm != null)
                {
                    selectedForm.Dispose();
                }

                selectedForm = new addComponents(this);
            }
            else
            {
                MessageBox.Show("У вас нет прав доступа для открытия этой формы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ShowOrders();
            selectedTable = "order";
            ClearControls();
            button2.Focus();
            activeButton = (Button)sender;
            button2.Enabled = true;

            //  роль текущего пользователя
            if (_user.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) || _user.Role.Equals("AdminOrder", StringComparison.OrdinalIgnoreCase))
            {

                if (selectedForm != null)
                {
                    selectedForm.Dispose();
                }

                selectedForm = new addOrder(this, _user);
            }
            else
            {
                MessageBox.Show("У вас нет прав доступа для открытия этой формы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ShowVendors();
            selectedTable = "vendors";
            ClearControls();
            button3.Focus();
            activeButton = (Button)sender;
            button3.Enabled = true;
            selectedForm = null; // Обнуляем выбранную форму


            if (_user.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) || _user.Role.Equals("AdminVendors", StringComparison.OrdinalIgnoreCase))
            {
                if (selectedForm != null)
                {
                    selectedForm.Dispose();
                }
                selectedForm = new addVenors(this);


            }
            else
            {
                MessageBox.Show("У вас нет прав доступа для открытия этой формы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {


            if (selectedForm != null)
            {
                selectedForm.ShowDialog(this);

                RefreshDataGridView();



            }



        }


        private void button6_Click(object sender, EventArgs e) //кнопка выхода
        {


            authorization form1 = new authorization();
            form1.Show();

            this.Close();

        }
        private void SearchRecords(string searchText, DataGridView dataGridView)
        {
            if (dataGridView.DataSource is DataTable dataTable)
            {
                // Создаем новый DataTable, чтобы избежать изменений в исходных данных
                DataTable filteredTable = dataTable.Clone();

                // Приводим текст поиска к нижнему регистру для регистронезависимого поиска
                string searchLower = searchText.ToLower();

                // Проходимся по всем строкам исходной таблицы и добавляем совпадающие строки в отфильтрованную таблицу
                foreach (DataRow row in dataTable.Rows)
                {
                    // Проходимся по всем ячейкам в строке
                    foreach (object item in row.ItemArray)
                    {
                        // Проверяем, содержит ли значение ячейки текст поиска
                        if (item != null && item.ToString().ToLower().Contains(searchLower))
                        {
                            // Если есть совпадение, добавляем строку в отфильтрованную таблицу
                            filteredTable.ImportRow(row);
                            // Выходим из цикла по ячейкам, чтобы не добавить строку несколько раз
                            break;
                        }
                    }
                }

                // Обновляем источник данных DataGridView
                dataGridView.DataSource = filteredTable;



            }
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            string searchText = textBox2.Text;
            SearchRecords(searchText, guna2DataGridView1);

        }


        private void UpdateDataGridView()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                database.openConnection();

                switch (selectedTable)
                {
                    case "order":
                        ShowOrders();
                        break;
                    case "vendors":
                        ShowVendors();
                        break;
                    case "tovar":
                        Showtovar();
                        break;
                    default:
                        break;
                }
            }
        }

        private void RefreshDataGridView()
        {
            guna2DataGridView1.DataSource = null; // Очистка источника данных
            guna2DataGridView1.Rows.Clear(); // Очистка строк
            guna2DataGridView1.Columns.Clear(); // Очистка столбцов
            guna2DataGridView1.Refresh(); // Обновление DataGridView

            UpdateDataGridView(); // Вызов метода загрузки данных заново
        }


        private void InitializeTextBoxes()
        {
            //  инициализация всех текстовых полей 

        }

        private void button10_Click(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }








        private void SaveChangesToDatabase(string name_order, string order_date, string delivery)
        {

        }
        private void LoadDataForEdit(string tableName, int primaryKeyValue)
        {

        }




        private void управлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Adminis adminis = new Adminis();
            adminis.Show();

        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }



        private void button5_Click(object sender, EventArgs e)
        {
            Client client = new Client(_user);
            client.Show();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem.ToString() == "Аккаунт")
            {
                //   форма Account
                Account accountForm = new Account();
                accountForm.Show();
                selectedStatus = comboBox2.SelectedItem?.ToString();
            }
            else if (comboBox2.SelectedItem.ToString() == "Выйти")
            {

            }
        }


        private void информацияОСотрудникахToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (_user.Role == "admin" || _user.Role == "HR-manager")
            {
                // Если пользователь имеет необходимую роль, открывается форма информации о сотрудниках
                sotrudniki sotrudnikiForm = new sotrudniki();
                sotrudnikiForm.ShowDialog();
            }
            else
            {

                MessageBox.Show("У вас нет доступа для выполнения этой операции.", "Недостаточно прав", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (_user.Role == "admin" || _user.Role == "AdminOrder")
            {

                OrderHistory orderHistory = new OrderHistory();
                orderHistory.Show();
            }
            else
            {
                MessageBox.Show("У вас нет доступа для выполнения этой операции.", "Недостаточно прав", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
