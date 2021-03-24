﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopApp.Luis;
using MySql.Data.MySqlClient;

namespace DesktopApp.Klea
{
    public partial class JustifikoMungesatUC : UserControl
    {
        string nxenesID;
        string mID;
        int i;
        public JustifikoMungesatUC()
        {
            InitializeComponent();
        }

        private void JustifikoMungesatUC_Load(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fullName = comboBox3.Text;
            comboBox3.Text = fullName;
            var names = fullName.Split(' ');
            var firstName = names[0];
            var lastName = names[1];

            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT NxenesID from Nxenes WHERE KlasaID in (SELECT KlasaID FROM Klasa WHERE Emri = '" + textBox4.Text + "') AND Emri = '" + firstName + "' AND Mbiemri = '" + lastName + "'";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //Iterate through the rows and add it to the combobox's items
                        while (reader.Read())
                        {
                            nxenesID = reader.GetString("NxenesID");
                        }
                    }
                }
            }
            DataGridRefresher();
        }
        void DataGridRefresher()
        {
            
            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "Select a.MungesaID, b.EmerLende, a.DATAT, a.Justifikuar from Mungesat a, Lendet b where a.LendaID = b.LendaID and a.NxenesID = '" + nxenesID + "'; ";
                   using (var da = new MySqlDataAdapter(query, connection))
                {
                    var ds = new DataSet();
                    da.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                }
                connection.Close();
            }
        }

        private void comboBox3_Click(object sender, EventArgs e)
        {
            try
            {
                var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM Nxenes WHERE KlasaID IN (SELECT KlasaID FROM Klasa WHERE Emri = '" + textBox4.Text + "')";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            //Iterate through the rows and add it to the combobox's items
                            while (reader.Read())
                            {
                                comboBox3.Items.Add(reader.GetString("Emri") + " " + reader.GetString("Mbiemri"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (i == 1)
            {
                i = 0;
                var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
                try
                {
                    string StrQuery = "UPDATE Mungesat SET Justifikuar = 'po' WHERE MungesaID = '"+mID+"'";

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        using (MySqlCommand comm = new MySqlCommand(StrQuery, conn))
                        {
                            conn.Open();
                            comm.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DataGridRefresher();

            }
            else MessageBox.Show("Së pari ju duhet të zgjidhni një mungesë nga tabela dhe më pas ta justifikoni atë duke klikuar butoin 'Justifiko'");
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            i = 1;
            mID = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            i = 1;
            mID = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
        }
    }
}
