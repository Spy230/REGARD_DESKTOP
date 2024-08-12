using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms; 
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Diagnostics;
using Word = Microsoft.Office.Interop.Word;


namespace regard
{
    public partial class OrderHistory : Form
    {
        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA\\SQLEXPRESS;Initial Catalog=regard;Integrated Security=True";
        private const string TemplatePath = @"C:\Users\shest\Downloads\check.docx";
        public OrderHistory()
        {
            InitializeComponent();
            LoadOrderHistory();
            LoadClients();
        }

        private void LoadOrderHistory()
        {

        }



        private void LoadClients()
        {
            // Очистка комбобокса перед загрузкой новых данных
            guna2ComboBox1.Items.Clear();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT name FROM Client";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    guna2ComboBox1.Items.Add(reader["name"].ToString()); // Добавить имя клиента в комбобокс
                }

                reader.Close();
            }
        }

        private void LoadOrderHistory(string selectedClient)
        {
            if (!string.IsNullOrEmpty(selectedClient))
            {
                DataTable orderTable = new DataTable();

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = $@"SELECT o.name_order AS 'Название заказа', 
                c.name AS 'Клиент', 
                t.price AS 'Цена', 
                o.order_date AS 'Дата заказа'
                FROM [order] o
                INNER JOIN Client c ON o.id_client = c.id_client
                INNER JOIN tovar t ON o.id_tovar = t.id_tovar
                WHERE c.name = @clientName
                ORDER BY o.order_date DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@clientName", selectedClient);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(orderTable);
                }

                guna2DataGridView1.DataSource = orderTable;
                if (guna2DataGridView1.Columns.Contains("id_order"))
                {
                    guna2DataGridView1.Columns["id_order"].Visible = false; // Скрыть колонку с идентификатором заказа
                }

                UpdateTotalSumTextBox(orderTable); // Обновление итоговой суммы после загрузки данных
            }
            else
            {
                MessageBox.Show("Выберите клиента из списка.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void guna2DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateTotalSumTextBox((DataTable)guna2DataGridView1.DataSource); // Обновление итоговуой суммы после редактирования ячеек
        }
        private void UpdateTotalSumTextBox(DataTable table)
        {
            decimal totalSum = CalculateTotalSum(table);
            guna2TextBox1.Text = totalSum.ToString("C"); 
        }

        private decimal CalculateTotalSum(DataTable table)
        {
            if (table != null && table.Rows.Count > 0)
            {
                return table.AsEnumerable().Sum(row => row.Field<decimal>("Цена"));
            }
            else
            {
                return 0;
            }
        }
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOrderHistory(guna2ComboBox1.SelectedItem?.ToString());
        }
        private void GenerateReport(string outputPath)
        {
            DataTable table = (DataTable)guna2DataGridView1.DataSource;

            if (table != null && table.Rows.Count > 0)
            {
                File.Copy(TemplatePath, outputPath, true);

                using (WordprocessingDocument doc = WordprocessingDocument.Open(outputPath, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;

                    int orderParagraphIndex = -1;
                    for (int i = 0; i < body.ChildElements.Count; i++)
                    {
                        var paragraph = body.ChildElements[i] as Paragraph;
                        if (paragraph != null && paragraph.InnerText.Contains("{order}"))
                        {
                            orderParagraphIndex = i;
                            break;
                        }
                    }

                    if (orderParagraphIndex != -1)
                    {
                        decimal totalSum = table.AsEnumerable().Sum(row => row.Field<decimal>("Цена")); // Суммирование всех цен заказов

                        foreach (var row in table.AsEnumerable())
                        {
                            var order = row.Field<string>("Название заказа");
                            var price = row.Field<decimal>("Цена");
                            var paragraphText = $"{order} - {price:C}"; // Форматирование строки с названием товара и ценой
                            var newParagraph = new Paragraph(new Run(new Text(paragraphText)));
                            body.InsertAt(newParagraph, orderParagraphIndex++);
                        }

                        // Заменяем метку {summa1} на общую сумму
                        foreach (var paragraph in body.Descendants<Paragraph>())
                        {
                            foreach (var run in paragraph.Descendants<Run>())
                            {
                                foreach (var text in run.Descendants<Text>())
                                {
                                    if (text.Text.Contains("{summa1}"))
                                    {
                                        text.Text = text.Text.Replace("{summa1}", totalSum.ToString("C")); // Форматирование суммы
                                    }
                                }
                            }
                        }

                        //  замена метки в документе Word на значение из текстбокса
                        string placeholderDsd = "{PLACEHOLDER_DSD}";
                        foreach (var paragraph in body.Descendants<Paragraph>())
                        {
                            foreach (var run in paragraph.Descendants<Run>())
                            {
                                foreach (var text in run.Descendants<Text>())
                                {
                                    if (text.Text.Contains(placeholderDsd))
                                    {
                                        text.Text = text.Text.Replace(placeholderDsd, guna2TextBox1.Text); // Замена метки на значение из текстбокса
                                    }
                                }
                            }
                        }

                        ReplacePlaceholder("{PLACEHOLDER_DSD}", guna2TextBox1.Text, doc.MainDocumentPart.Document); // Замена метки на значение из текстбокса

                    }
                }

                MessageBox.Show("Отчет успешно сгенерирован.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Нет данных для генерации отчета.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ReplacePlaceholder(string placeholder, string replacement, DocumentFormat.OpenXml.Wordprocessing.Document worddoc)
        {
            foreach (var paragraph in worddoc.MainDocumentPart.Document.Descendants<Paragraph>())
            {
                foreach (var run in paragraph.Descendants<Run>())
                {
                    foreach (var text in run.Descendants<Text>())
                    {
                        if (text.Text.Contains(placeholder))
                        {
                            text.Text = text.Text.Replace(placeholder, replacement);
                        }
                    }
                }
            }

        }
                private void button1_Click(object sender, EventArgs e)
        {
            string outputPath = @"C:\Users\shest\Downloads\check1.docx";
            GenerateReport(outputPath);
            MessageBox.Show("Отчет успешно сгенерирован.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
