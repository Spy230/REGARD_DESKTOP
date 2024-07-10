using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using MetroFramework.Components;
using MetroFramework.Forms;
using BCrypt.Net;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Word;
using MetroFramework.Drawing;

namespace regard
{
    public partial class Form2 : MetroForm
    {


        private MaskedTextBox maskedTextBoxPhone;
        DataBase dataBase = new DataBase();
        public Form2()
        {
            InitializePhoneMaskedTextBox();
            InitializeComponent();
        }


        private void InitializePhoneMaskedTextBox()
        {
            maskedTextBoxPhone = new MaskedTextBox();
            maskedTextBoxPhone.Mask = "+7 (999) 000-00-00";
            maskedTextBoxPhone.MaxLength = 18;
            maskedTextBoxPhone.BorderStyle = System.Windows.Forms.BorderStyle.None;  

            // Установка позиции и размера
            maskedTextBoxPhone.Location = new System.Drawing.Point(529, 227);
            maskedTextBoxPhone.Size = new Size(198, 16);

            // Добавление maskedTextBoxPhone на форму
            this.Controls.Add(maskedTextBoxPhone);

            // Привязка обработчика события
            maskedTextBoxPhone.TextChanged += MaskedTextBoxPhone_TextChanged;
        }



        private void MaskedTextBoxPhone_TextChanged(object sender, EventArgs e)
        {
            
            textBox6.Text = maskedTextBoxPhone.Text;
        }




        private void btnEnter2_Click_1(object sender, EventArgs e)
        {
            var login = textBox1.Text;
            var password = textBox2.Text;
            var confirmPassword = textBox3.Text;
            var name = textBox4.Text;
            var lastName = textBox5.Text;
            var phoneNumber = maskedTextBoxPhone.Text;

            if (password != confirmPassword)
            {
                label12.ForeColor = Color.Red;  
                label12.Text = "Пароли не совпадают";
                return;
            }
            else
            { 
                label12.Text = "";  
            }

            if (!IsValidEmailDomain(login))
            {
                label11.Visible = true;
                label11.ForeColor = Color.Red;
                label11.Text = "Введите корректный адрес электронной почты";
                return;  
            }
            else
            {
                label11.Visible = false;
            }

            // Шифрование пароля перед сохранением в базу данных
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            string queryString = $"INSERT INTO register (login_user, password_user, is_admin, name, last_name, number) " +
                                  $"VALUES ('{login}', '{hashedPassword}', 0, '{name}', '{lastName}', '{phoneNumber}')";

            try
            {
                using (SqlCommand command = new SqlCommand(queryString, dataBase.getConnection()))
                {
                    dataBase.openConnection();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Аккаунт создан");
                        authorization frm_Frm = new authorization();
                        this.Hide();
                        frm_Frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Аккаунт не создан");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании аккаунта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dataBase.closeConnection();
            }
        }


        private Boolean checkuser()

        {
            var loginUser = textBox1.Text;
            var passwordUser = textBox2.Text;


            SqlDataAdapter adapter = new SqlDataAdapter();
            string squerriString = $"select id_User, login_user,password_user , is_admin from register where login_user = '{loginUser}' and password_user = '{passwordUser}'";
            SqlCommand command = new SqlCommand(squerriString, dataBase.getConnection());
            System.Data.DataTable table = new System.Data.DataTable();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("пользователь уже существует");
                return true;
            }
            else
            {
                return false;
            }


        }



        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {

            authorization form1 = new authorization();
            form1.Show();

            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            authorization form1 = new authorization();
            form1.Show();

            this.Close();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked. Error: " + ex.Message);
            }
        }
        private void VisitLink2()
        {

            linkLabel2.LinkVisited = true;


            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo = psi
            };

            process.Start();

           
            process.StandardInput.WriteLine("start https://www.regard.ru/about/legal?ysclid=lrc0me6nfz755417372");
            process.StandardInput.Close();
            process.WaitForExit();
        }







        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked. Error: " + ex.Message);
            }
        }

        private void VisitLink()
        {

            linkLabel2.LinkVisited = true;


            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo = psi
            };

            process.Start();


            process.StandardInput.WriteLine("start https://www.regard.ru/about/legal/privacy_policy?ysclid=lrbzw2dekm614948196");
            process.StandardInput.Close();
            process.WaitForExit();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink3();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked. Error: " + ex.Message);
            }
        }

        private void VisitLink3()
        {

            linkLabel2.LinkVisited = true;


            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo = psi
            };

            process.Start();

            process.StandardInput.WriteLine("start https://www.regard.ru/contacts?ysclid=lrc0oqr1fg279638794");
            process.StandardInput.Close();
            process.WaitForExit();
        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

            textBox2.PasswordChar = guna2CheckBox1.Checked ? '\0' : '*';
            textBox3.PasswordChar = guna2CheckBox1.Checked ? '\0' : '*';
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private bool IsValidEmailDomain(string email)
        {

            Regex regex = new Regex(@"@(gmail\.com|yahoo\.com|yandex\.ru)$", RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }

        private void guna2ShadowPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}







            

       
   







