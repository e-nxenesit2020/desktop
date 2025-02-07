﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using DesktopApp.Luis;

namespace DesktopApp.Martin
{
    public partial class FletTremujori : UserControl
    {
         
        string NR_amze, Mesuesi, m_arsyeshme, m_paarsyeshme;
        string date = DateTime.UtcNow.ToString("dd-MM-yyyy");
        string firstName, lastName;
        string loginID = CookieClass.LoginID;

        public FletTremujori()
        {
            InitializeComponent();
        }


        private void FletTremujori_Load(object sender, EventArgs e)
        {
            
        }

        public void exportgridtopdf(DataGridView dgw, string filename)
        {
            try
            {
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
                PdfPTable table = new PdfPTable(3);
                table.DefaultCell.PaddingLeft = 5;
                table.DefaultCell.PaddingRight = 5;
                table.DefaultCell.FixedHeight = 20;
                table.WidthPercentage = 45;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                

                PdfPTable pdftable = new PdfPTable(dgw.Columns.Count);
                pdftable.DefaultCell.PaddingLeft = 5;
                pdftable.DefaultCell.PaddingRight = 5;
                pdftable.DefaultCell.FixedHeight = 20;
                pdftable.WidthPercentage = 75;
                pdftable.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdftable.DefaultCell.BorderWidth = 1;



                iTextSharp.text.Font text = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                //add header
                foreach (DataGridViewColumn column in dgw.Columns)
                {

                    PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, text));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240); //HEADER COLOR
                    pdftable.AddCell(cell);
                }

                // add datarow
                foreach (DataGridViewRow row in dgw.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        pdftable.AddCell(new Phrase(cell.Value.ToString(), text));
                    }
                }

                //MARRJA E NR TE AMZES SE NXENESIT
                string fullName = NxenescomboBox.Text;
                var names = fullName.Split(' ');
                firstName = names[0];
                lastName = names[1];

                var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT NxenesID FROM Nxenes WHERE Emri = '" + firstName + "' AND Mbiemri = '" + lastName + "'; ";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            //Iterate through the rows and add it to the combobox's items
                            while (reader.Read())
                            {
                                NR_amze = reader.GetString("NxenesID");
                            }
                        }
                    }
                }


                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT Emri, Mbiemri FROM Mesues WHERE MesuesID IN (SELECT MesuesID FROM Klasa WHERE Emri = '" + label6.Text + "'); ";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            //Iterate through the rows and add it to the combobox's items
                            while (reader.Read())
                            {
                                Mesuesi = reader.GetString("Emri") + " " + reader.GetString("Mbiemri");
                            }
                        }
                    }
                }

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT COUNT(*) FROM Mungesat WHERE NxenesID = '"+ NR_amze +"' AND Justifikuar = 'po' ";
                    using (var command = new MySqlCommand(query, connection))
                    {
                         m_arsyeshme = Convert.ToString(command.ExecuteScalar());
                    }
                    connection.Close();
                }

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT COUNT(*) FROM Mungesat WHERE NxenesID = '" + NR_amze + "' AND Justifikuar IS NULL ; ";
                    using (var command = new MySqlCommand(query, connection))
                    {
                         m_paarsyeshme = Convert.ToString(command.ExecuteScalar());
                    }
                    connection.Close();
                }


                PdfPCell cell0 = new PdfPCell();
                cell0.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240); //HEADER COLOR
                table.AddCell(cell0);

                PdfPCell cell1 = new PdfPCell(new Phrase("Të arsyeshme", text));
                cell1.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240); //HEADER COLOR
                table.AddCell(cell1);

                PdfPCell cell2 = new PdfPCell(new Phrase("Të paarsyeshme", text));
                cell2.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240); //HEADER COLOR
                table.AddCell(cell2);

                PdfPCell cell3 = new PdfPCell(new Phrase("Mungesat", text));
                cell3.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240); //HEADER COLOR
                table.AddCell(cell3);

                table.AddCell(m_arsyeshme);
                table.AddCell(m_paarsyeshme);

                var savefiledialoge = new SaveFileDialog();
                savefiledialoge.FileName = filename;
                savefiledialoge.DefaultExt = ".pdf";

                if (savefiledialoge.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(savefiledialoge.FileName, FileMode.Create))
                    {
                        Document pdfdoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        PdfWriter.GetInstance(pdfdoc, stream);
                        pdfdoc.Open();

                        iTextSharp.text.Font myFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));
                        iTextSharp.text.Font myFont1 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));

                        Paragraph paragraph1 = new Paragraph(" ");
                        Paragraph paragraph2 = new Paragraph(" ");
                        Paragraph paragraph3 = new Paragraph("                         FLETË INFORMUESE TREMUJORI PËR PRINDIN", myFont);
                        Paragraph paragraph4 = new Paragraph(" ");
                        Paragraph paragraph5 = new Paragraph(" ");

                        Paragraph paragraph6 = new Paragraph("                Shkolla për TIK “Hermann Gmeiner”                                      Viti shkollor: 2020-2021  ");
                        Paragraph paragraph7 = new Paragraph("                Nr i amzës: " + NR_amze);
                        Paragraph paragraph8 = new Paragraph("                Data: " + date + " ");
                        Paragraph paragraph9 = new Paragraph(" ");

                        Paragraph paragraph10 = new Paragraph("                                                                         Progresi i nxënësit ", myFont1);
                        Paragraph paragraph11 = new Paragraph("                                                                               " + NxenescomboBox.Text + " ");
                        Paragraph paragraph12 = new Paragraph(" ");
                        Paragraph paragraph13= new Paragraph(" ");

                        Paragraph paragraph14 = new Paragraph("                I. Progresi i nxënësit sipas lëndëve",myFont1);
                        Paragraph paragraph15 = new Paragraph(" ");
                        Paragraph paragraph16 = new Paragraph(" ");
                        Paragraph paragraph17 = new Paragraph(" ");
                        Paragraph paragraph18 = new Paragraph("                II. Frekuentimi i shkollës nga nxënësi",myFont1);
                        Paragraph paragraph19 = new Paragraph(" ");
                        Paragraph paragraph20 = new Paragraph(" ");
                        Paragraph paragraph21 = new Paragraph(" ");
                        Paragraph paragraph22 = new Paragraph(" "); 
                        Paragraph paragraph23 = new Paragraph(" ");
                        Paragraph paragraph24 = new Paragraph("                Mësuesi/ja                                                                                       Prindi/ Kujdestari ", myFont1);
                        Paragraph paragraph25 = new Paragraph("                " + Mesuesi + "                                                                            __________________");
                        Paragraph paragraph26 = new Paragraph(" ");

                        pdfdoc.Add(paragraph1);
                        pdfdoc.Add(paragraph2);
                        pdfdoc.Add(paragraph3);
                        pdfdoc.Add(paragraph4);
                        pdfdoc.Add(paragraph5);
                        pdfdoc.Add(paragraph6);
                        pdfdoc.Add(paragraph7);
                        pdfdoc.Add(paragraph8);
                        pdfdoc.Add(paragraph9);
                        pdfdoc.Add(paragraph10);
                        pdfdoc.Add(paragraph11);
                        pdfdoc.Add(paragraph12);
                        pdfdoc.Add(paragraph13);
                        pdfdoc.Add(paragraph14);
                        pdfdoc.Add(paragraph15);
                        pdfdoc.Add(pdftable); //TABELA NOTAVE
                        pdfdoc.Add(paragraph16);
                        pdfdoc.Add(paragraph17);
                        pdfdoc.Add(paragraph18);
                        pdfdoc.Add(paragraph19);
                        pdfdoc.Add(table);//TABELA MUNGESAVE
                        pdfdoc.Add(paragraph20);
                        pdfdoc.Add(paragraph21);
                        pdfdoc.Add(paragraph22);
                        pdfdoc.Add(paragraph23);
                        pdfdoc.Add(paragraph24);
                        pdfdoc.Add(paragraph25);
                        pdfdoc.Add(paragraph26);


                        pdfdoc.Close();
                        stream.Close();
                    }
                }
            }
            catch
            {

            }
        }

        public void exportgridstopdf(DataGridView dgw, string filename)
        {
            try
            {
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
                PdfPTable pdftable = new PdfPTable(dgw.Columns.Count);
                pdftable.DefaultCell.PaddingLeft = 5;
                pdftable.DefaultCell.PaddingRight = 5;
                pdftable.DefaultCell.FixedHeight = 20;
                pdftable.WidthPercentage = 75;
                pdftable.HorizontalAlignment = Element.ALIGN_CENTER;
                //pdftable.DefaultCell.BorderWidth = 1;



                iTextSharp.text.Font text = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                //add header
                foreach (DataGridViewColumn column in dgw.Columns)
                {

                    PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, text));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240); //potential error
                    pdftable.AddCell(cell);
                }

                // add datarow
                foreach (DataGridViewRow row in dgw.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        pdftable.AddCell(new Phrase(cell.Value.ToString(), text));
                    }
                }

                //MARRJA E NR TE AMZES SE NXENESIT
                string fullName = NxenescomboBox.Text;
                var names = fullName.Split(' ');
                firstName = names[0];
                lastName = names[1];

                var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
                using (var connection = new MySqlConnection(connectionString))
                {

                    connection.Open();
                    var query = "SELECT NxenesID FROM Nxenes WHERE Emri = '" + firstName + "' AND Mbiemri = '" + lastName + "'; ";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            //Iterate through the rows and add it to the combobox's items
                            while (reader.Read())
                            {
                                NR_amze = reader.GetString("NxenesID");
                            }
                        }
                    }
                }


                var savefiledialoge = new SaveFileDialog();
                savefiledialoge.FileName = filename;
                savefiledialoge.DefaultExt = ".pdf";

                if (savefiledialoge.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(savefiledialoge.FileName, FileMode.Create))
                    {
                        Document pdfdoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        PdfWriter.GetInstance(pdfdoc, stream);
                        pdfdoc.Open();

                        iTextSharp.text.Font myFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));
                        iTextSharp.text.Font myFont1 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));

                        Paragraph paragraph10 = new Paragraph(" ");
                        Paragraph paragraph11 = new Paragraph(" ");
                        Paragraph paragraph = new Paragraph("                         FLETË INFORMUESE TREMUJORI PËR PRINDIN", myFont);
                        Paragraph paragraph12 = new Paragraph(" ");
                        Paragraph paragraph2 = new Paragraph(" ");

                        Paragraph paragraph3 = new Paragraph("                Shkolla për TIK “Hermann Gmeiner”                                      Viti shkollor: 2020-2021  ");
                        Paragraph paragraph4 = new Paragraph("                Nr i amzës: " + NR_amze, myFont1);
                        Paragraph paragraph5 = new Paragraph("                Data: " + date + " ");
                        Paragraph paragraph6 = new Paragraph(" ");

                        Paragraph paragraph7 = new Paragraph("                                                                         Progresi i nxënësit ", myFont1);
                        Paragraph paragraph8 = new Paragraph("                                                                               " + NxenescomboBox.Text + " ");
                        Paragraph paragraph9 = new Paragraph(" ");

                        pdfdoc.Add(paragraph10);
                        pdfdoc.Add(paragraph11);
                        pdfdoc.Add(paragraph);
                        pdfdoc.Add(paragraph12);
                        pdfdoc.Add(paragraph2);
                        pdfdoc.Add(paragraph3);
                        pdfdoc.Add(paragraph4);
                        pdfdoc.Add(paragraph5);
                        pdfdoc.Add(paragraph6);
                        pdfdoc.Add(paragraph7);
                        pdfdoc.Add(paragraph8);
                        pdfdoc.Add(paragraph9);
                        pdfdoc.Add(pdftable);

                        pdfdoc.Close();
                        stream.Close();
                    }
                }
            }
            catch
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            exportgridtopdf(dataGridView2, "Notat e tremujorit të nxënësit " + NxenescomboBox.Text + " ");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            //SHFAQJA E TE DHENAVE NE DATAGRIDVIEW1 
            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();  
                 var query = "SELECT Nxenes.Emri, Nxenes.Mbiemri, NotaTremujor.NotaGoje AS 'Nota me Gojë', NotaTremujor.NotaShkrim AS 'Nota e Testit', NotaTremujor.NotaPortofol AS 'Nota e Portofolit' FROM Nxenes JOIN NotaTremujor ON Nxenes.NxenesID = NotaTremujor.NxenesID WHERE NotaTremujor.KlasaID IN(SELECT KlasaID From Klasa WHERE Emri = '" + label6.Text + "') AND NotaTremujor.LendaID IN (SELECT LendaID FROM Lendet WHERE EmerLende = '"+ comboBox2.Text + "' ) AND NotaTremujor.Periudha IN (SELECT PeriudhaID FROM Periudha WHERE Emri = '" + comboBox1.Text + "' ); ";
                using (var da = new MySqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                connection.Close();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";

            //SHFAQJA E TE DHENAVE NE DATAGRIDVIEW1 KUR KLIKOHET BUTONI RIFRESKONI
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT Nxenes.Emri, Nxenes.Mbiemri, NotaTremujor.NotaGoje AS 'Nota me Gojë', NotaTremujor.NotaShkrim AS 'Nota e Testit', NotaTremujor.NotaPortofol AS 'Nota e Portofolit' FROM Nxenes JOIN NotaTremujor ON Nxenes.NxenesID = NotaTremujor.NxenesID WHERE NotaTremujor.KlasaID IN(SELECT KlasaID From Klasa WHERE Emri = '" + label6.Text + "'); ";
                using (var da = new MySqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                connection.Close();
            }

            //SHFAQJA E TE DHENAVE NE DATAGRIDVIEW2 KUR KLIKOHET BUTONI RIFRESKONI
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT Lendet.EmerLende AS 'Lënda', NotaTremujor.NotaGoje AS 'Nota me Gojë', NotaTremujor.NotaShkrim AS 'Nota e Testit', NotaTremujor.NotaPortofol AS 'Nota e Portofolit' FROM NotaTremujor JOIN Lendet ON NotaTremujor.LendaID = Lendet.LendaID WHERE NotaTremujor.KlasaID IN(SELECT KlasaID From Klasa WHERE Emri = '" + label6.Text + "'); ";
                using (var da = new MySqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView2.DataSource = dt;
                }
                connection.Close();
            }

            //MBUSHJA E COMBOBOX ME EMRAT E LENDEVE QE ZHVILLON KLASA
            comboBox2.Items.Clear();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT EmerLende FROM Lendet WHERE LendaID IN (SELECT LendaID FROM Jep_Mesim WHERE KlasaID IN (SELECT KlasaID FROM Klasa WHERE Emri = '"+ label6.Text +"'))";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //Iterate through the rows and add it to the combobox's items
                        while (reader.Read())
                        {
                            comboBox2.Items.Add(reader.GetString("EmerLende"));
                        }
                    }
                }
            }

            //MBUSHJA E COMBOBOX ME EMRAT E NXENESVE TE KLASES PERKATESE
            NxenescomboBox.Items.Clear();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "Select * from Nxenes where KlasaID in (Select KlasaID from Klasa where Emri = '" + label6.Text + "' )";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //Iterate through the rows and add it to the combobox's items
                        while (reader.Read())
                        {
                            NxenescomboBox.Items.Add(reader.GetString("Emri") + " " + reader.GetString("Mbiemri"));
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
            DataTable dt1 = new DataTable();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Nxenes WHERE KlasaID IN (select KlasaID FROM Klasa where Emri = '"+ label6.Text + "')";
                using (var da = new MySqlDataAdapter(query, connection))
                {
                    da.Fill(dt1);
                }
                connection.Close();
            }

            foreach (DataRow row in dt1.Rows)
            {
                //pdfdoc.NewPage();
                //PdfPage page = pdfdoc.Pages.Add();

                NxenescomboBox.Text = row["Emri"].ToString() + " " + row["Mbiemri"].ToString();
                        exportgridtopdf(dataGridView2, "Notat e tremujorit të nxënësit " + row["Emri"].ToString() + " " + row["Mbiemri"].ToString() + " ");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
            comboBox1.Items.Clear();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Periudha";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //Iterate through the rows and add it to the combobox's items
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader.GetString("Emri"));
                        }
                    }
                }
                connection.Close();
            }
        }

        private void comboBox2_Click(object sender, EventArgs e)
        {
            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
            //MBUSHJA E COMBOBOX ME EMRAT E LENDEVE QE ZHVILLON KLASA
            comboBox2.Items.Clear();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT EmerLende FROM Lendet WHERE LendaID IN (SELECT LendaID FROM Jep_Mesim WHERE KlasaID IN (SELECT KlasaID FROM Klasa WHERE Emri = '" + label6.Text + "'))";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //Iterate through the rows and add it to the combobox's items
                        while (reader.Read())
                        {
                            comboBox2.Items.Add(reader.GetString("EmerLende"));
                        }
                    }
                }
            }
        }

        private void NxenescomboBox_Click(object sender, EventArgs e)
        {
            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
            NxenescomboBox.Items.Clear();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "Select * from Nxenes where KlasaID in (Select KlasaID from Klasa where Emri = '" + label6.Text + "' )";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //Iterate through the rows and add it to the combobox's items
                        while (reader.Read())
                        {
                            NxenescomboBox.Items.Add(reader.GetString("Emri") + " " + reader.GetString("Mbiemri"));
                        }
                    }
                }
            }
        }

        private void NxenescomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MBUSHJA E DATAGRIDVIEW ME NOTAT E NXENESIT TE PERCAKTUAR
            string fullName = NxenescomboBox.Text;
            var names = fullName.Split(' ');
            firstName = names[0];
            lastName = names[1];

            var connectionString = "server=remotemysql.com;userid=gBh6InugME;password=NSGsLG2ITM;database=gBh6InugME";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT Lendet.EmerLende AS 'Lënda', NotaTremujor.NotaGoje AS 'Nota me Gojë', NotaTremujor.NotaShkrim AS 'Nota e Testit', NotaTremujor.NotaPortofol AS 'Nota e Portofolit' FROM NotaTremujor JOIN Lendet ON NotaTremujor.LendaID = Lendet.LendaID WHERE NotaTremujor.KlasaID IN(SELECT KlasaID From Klasa WHERE Emri = '" + label6.Text + "') AND NotaTremujor.NxenesID IN(SELECT NxenesID FROM Nxenes WHERE Emri = '"+ firstName + "' AND Mbiemri = '" + lastName + "') AND NotaTremujor.Periudha IN (SELECT PeriudhaID FROM Periudha WHERE Emri = '" + comboBox1.Text + "' ); ";
                using (var da = new MySqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView2.DataSource = dt;
                }
                connection.Close();
            }
        }
    }
}
