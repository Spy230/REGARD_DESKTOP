using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace regard
{
    public partial class Client : Form
    {
        private readonly checkUser _user;
        private DataTable dataTable;
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True";
      
        public Client(checkUser user)
        {
            InitializeComponent();
            LoadClientData();
            
            _user = user;
            guna2DataGridView1.CellDoubleClick += guna2DataGridView1_CellDoubleClick;
           
            guna2DataGridView1.DataBindingComplete += guna2DataGridView1_DataBindingComplete;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверяем роль пользователя
            if (_user.Role == "Manager_Client" || _user.Role == "admin")
            {
                // Если пользователь имеет необходимую роль, открываем форму добавления клиента
                AddClient addClient = new AddClient();
                addClient.Show();
            }
            else
            {
                // Если у пользователя нет необходимой роли, выводим сообщение об ошибке или просто игнорируем действие
                MessageBox.Show("У вас нет доступа для выполнения этой операции.", "Недостаточно прав", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
       
        private void LoadClientData()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"
            SELECT c.id_client, c.name AS 'Имя', 
                   c.[contact_number] AS 'телефон', 
                   c.[email] AS 'почта', 
                   cs.[vid_statusa] AS 'статус', 
                   c.[address] AS 'адресс', 
                   c.[photo] AS 'фото', 
                   c.[gender] AS 'пол', 
                   tc.[type] AS 'тип клиента', 
                   c.[birth_date] AS 'Дата рождения'  
            FROM [regard].[dbo].[Client] c
            INNER JOIN [regard].[dbo].[Custumer_Status] cs ON c.[id_status] = cs.[id_status]
            INNER JOIN [regard].[dbo].[type_client] tc ON c.[id_type] = tc.[id_type]";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Присваиваем источник данных
                guna2DataGridView1.DataSource = dataTable;
            }
        }



        private void guna2DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Скрываем столбец id_client после завершения привязки данных
            if (guna2DataGridView1.Columns.Contains("id_client"))
            {
                guna2DataGridView1.Columns["id_client"].Visible = false;
            }
        }


        private void guna2DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_user.Role == "admin" || _user.Role == "Manager_Client")
            {
                 
                if (e.RowIndex >= 0)
                {

                    // Получаем выбранную строку
                    DataGridViewRow selectedRow = guna2DataGridView1.Rows[e.RowIndex];
                    string id_client = selectedRow.Cells["id_client"].Value.ToString();
                    // Получаем данные из выбранной строки
                    string name = selectedRow.Cells["Имя"].Value.ToString();
                    string contactNumber = selectedRow.Cells["телефон"].Value.ToString();
                    string email = selectedRow.Cells["почта"].Value.ToString();
                    string gender = selectedRow.Cells["пол"].Value.ToString();
                    string customerStatus = selectedRow.Cells["статус"].Value.ToString();
                    string type = selectedRow.Cells["тип клиента"].Value.ToString();
                    string address = selectedRow.Cells["адресс"].Value.ToString();
                    string birthDate = selectedRow.Cells["Дата рождения"].Value.ToString();

                    // Получаем массив байтов фотографии, предварительно проверив на NULL
                    byte[] photoBytes = selectedRow.Cells["фото"].Value != DBNull.Value ? (byte[])selectedRow.Cells["фото"].Value : null;

                    // Создаем и отображаем форму InfoClient
                    InfoClient infoClient = new InfoClient();
                    infoClient.FillData(id_client, name, contactNumber, email, gender, customerStatus, type, address, birthDate);
                    infoClient.ShowImage(photoBytes); // Отображаем фотографию
                    infoClient.Show();
                }
            }
            else
            {
                MessageBox.Show("Недостаточно прав для изменения информации о клиентах.");

            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
