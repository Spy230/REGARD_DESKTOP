using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Guna.UI2.WinForms;
namespace regard
{

    public partial class addVenors : Form

    {
        DataBase database = new DataBase();


        private const string ConnectionString = "Data Source=DESKTOP-BBVLBHA;Initial Catalog=regard;Integrated Security=True ";
        private order parentForm;
        public addVenors(order parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;
            this.FormClosing += addVenors_FormClosing;





        }


        private void addVenors_FormClosing(object sender, FormClosingEventArgs e)
        {
            parentForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            database.openConnection();


            var name = guna2TextBox1.Text;
            var number = guna2TextBox2.Text;
            var contact_name = guna2TextBox3.Text;
            var email = guna2TextBox4.Text;
            var addQuerry = $"insert into [vendors] ( name,  number , contact_name , email  ) values ('{name}','{number}', '{contact_name}','{email}') ";

            var command = new SqlCommand(addQuerry, database.getConnection());
            command.ExecuteNonQuery();
            MessageBox.Show("запись создана");



            database.closeConnection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            parentForm.Show();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
