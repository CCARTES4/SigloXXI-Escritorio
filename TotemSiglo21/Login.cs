using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace TotemSiglo21
{
    public partial class Login : Form
    {

        OracleConnection conexion = new OracleConnection("DATA SOURCE=localhost:1521/xe;PASSWORD=123;USER ID=SIGLO21");


        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                login();
            }
            catch (Exception)
            {

                MessageBox.Show("Error al iniciar sesion ");
            }
        }

        private void login()
        {
            conexion.Open();
            OracleCommand cmd = new OracleCommand("SELECT RUT FROM TRABAJADOR WHERE RUT = :rut", conexion);
            cmd.Parameters.Add(":rut", txtRut.Text);
            OracleDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                ComprobarReserva cr = new ComprobarReserva();
                cr.Show();
                Visible = false;
                conexion.Close();
            }
            else
            {
                MessageBox.Show("No existe ningun usuario con el rut ingresado.");
                conexion.Close();
            }
            conexion.Close();
        }

        public string guardarRutTrabajador()
        {
            String rut = txtRut.Text;
            return rut;
        }

        private void Inicio_Load(object sender, EventArgs e)
        {

        }

        private void txtRut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                login();
            }
        }
    }
}
