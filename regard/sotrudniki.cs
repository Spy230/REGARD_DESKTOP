using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace regard
{
    public partial class sotrudniki : Form
    {
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True";
        public sotrudniki()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT s.[FIO] AS 'ФИО', 
                                   s.[number] AS 'Номер телефона', 
                                   d.[vid_dolgnost] AS 'Должность', 
                                   s.[id_sotrudnika]
                            FROM [regard].[dbo].[sotrudniki] s
                            INNER JOIN [regard].[dbo].[dolgnost] d ON s.[id_dolgnost] = d.[id_dolgnost];";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    guna2DataGridView1.DataSource = dataTable;
                    guna2DataGridView1.Columns["id_sotrudnika"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            addSotrudniki addSotrudnikiForm = new addSotrudniki(); 
            addSotrudnikiForm.Show(); 
        }

    }
}
