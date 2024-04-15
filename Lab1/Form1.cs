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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab1
{
    public partial class Form1 : Form
    {
        string connectionString = @"Server=DESKTOP-82UKUNB\SQLEXPRESS;Database=Magazin_Instrumente;
        Integrated Security=true;TrustServerCertificate=true;";
        DataSet ds = new DataSet();
        SqlDataAdapter parentAdapter;
        SqlDataAdapter childAdapter;
        BindingSource bsParent;
        BindingSource bsChild;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    //MessageBox.Show("Starea Conexiunii: " + conn.State.ToString());
                    parentAdapter = new SqlDataAdapter("SELECT * FROM Producator;", conn);
                    childAdapter = new SqlDataAdapter("SELECT * FROM Instrument;", conn);
                    parentAdapter.Fill(ds, "Producator");
                    childAdapter.Fill(ds, "Instrument");
                    DataColumn pkColumn = ds.Tables["Producator"].Columns["id_prod"];
                    DataColumn fkColumn = ds.Tables["Instrument"].Columns["id_prod"];
                    DataRelation relation = new DataRelation("FK_Producator_Instrument", pkColumn,
                    fkColumn);
                    ds.Relations.Add(relation);
                    bsParent = new BindingSource();
                    bsChild = new BindingSource();
                    bsParent.DataSource = ds.Tables["Producator"];
                    producatorGridView.DataSource = bsParent;
                    bsChild.DataSource = bsParent;
                    bsChild.DataMember = "FK_Producator_Instrument";
                    instrumentGridView.DataSource = bsChild;
                    textBox1.DataBindings.Add("Text", bsParent, "nume_prod", true);
                    textBox2.DataBindings.Add("Text", bsParent, "id_prod", true);
                    textBox5.DataBindings.Add("Text", bsChild, "id_inst", true);
                    textBox9.DataBindings.Add("Text", bsChild, "id_inst", true);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    parentAdapter.SelectCommand.Connection = connection;
                    childAdapter.SelectCommand.Connection = connection;
                    ds.Tables["Instrument"].Clear();
                    ds.Tables["Producator"].Clear();
                    parentAdapter.Fill(ds, "Producator");
                    childAdapter.Fill(ds, "Instrument");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (producatorGridView.CurrentRow != null)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string querry = "IF EXISTS(SELECT * FROM Producator WHERE id_prod = @cod_prod)" +
                            "BEGIN " +
                            "IF @pret > 0 " +
                            "BEGIN " +
                            "INSERT INTO Instrument (id_prod, model_instrument, pret_instrument) VALUES (@cod_prod, @model, @pret) " +
                            "END " +
                            "END ";
                        using (SqlCommand command = new SqlCommand(querry, connection))
                        {
                            command.Parameters.AddWithValue("@cod_prod", textBox2.Text);
                            command.Parameters.AddWithValue("@model", textBox3.Text);
                            command.Parameters.AddWithValue("@pret", textBox4.Text);

                            int result = command.ExecuteNonQuery();
                            if(result > 0) {
                                MessageBox.Show("Instrumentul a fost adaugat cu succes.");
                            }
                            else
                            {
                                MessageBox.Show("Instrumentul nu a fost adaugat.");
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                buttonRefresh_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Selectați un producator pentru a adăuga un instrument.");
            }

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (instrumentGridView.CurrentRow != null)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string querry = "IF EXISTS(SELECT * FROM Instrument WHERE id_inst = @cod_inst) " +
                            "BEGIN " +
                            "DELETE FROM Instrument WHERE id_inst = @cod_inst " +
                            "END";
                        using (SqlCommand command = new SqlCommand(querry, connection))
                        {
                            command.Parameters.AddWithValue("@cod_inst", textBox5.Text);
                            int res = command.ExecuteNonQuery();
                            if(res > 0)
                            {
                                MessageBox.Show("Instrumentul a fost sters cu succes.");
                            }
                            else
                            {
                                MessageBox.Show("Instrumentul cu id-ul dat nu exista.");
                            }
                        }
                        connection.Close();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                buttonRefresh_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Nu exista instrumente.");
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if(instrumentGridView.CurrentRow != null && producatorGridView.CurrentRow != null)
            {
                try
                {
                    using(SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string querry = "IF EXISTS(SELECT * FROM Instrument WHERE id_inst = @cod_inst) " +
                            "BEGIN " +
                            "IF EXISTS(SELECT * FROM Producator WHERE id_prod = @cod_prod) " +
                            "BEGIN " +
                            "IF @pret > 0 " +
                            "BEGIN " +
                            "UPDATE Instrument SET id_prod = @cod_prod, model_instrument = @model, pret_instrument = @pret WHERE id_inst = @cod_inst " +
                            "END " +
                            "END " +
                            "END";
                        using (SqlCommand command = new SqlCommand(querry,connection))
                        {
                            command.Parameters.AddWithValue("@cod_inst", textBox9.Text);
                            command.Parameters.AddWithValue("@cod_prod", textBox8.Text);
                            command.Parameters.AddWithValue("@model", textBox7.Text);
                            command.Parameters.AddWithValue("@pret", textBox6.Text);
                            int res = command.ExecuteNonQuery();
                            if (res > 0)
                            {
                                MessageBox.Show("Instrumentul a fost actualizat cu succes.");
                            }
                            else
                            {
                                MessageBox.Show("Actualizarea instumentului a esuat.");
                            }
                        }
                        connection.Close();

                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                buttonRefresh_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Nu exista instrumente sau producatori.");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
