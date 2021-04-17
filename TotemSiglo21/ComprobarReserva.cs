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
    public partial class ComprobarReserva : Form
    {
        public ComprobarReserva()
        {
            InitializeComponent();
        }

        OracleConnection conexion = new OracleConnection("DATA SOURCE=localhost:1521/xe;PASSWORD=123;USER ID=SIGLO21");


        private void Form1_Load(object sender, EventArgs e)
        {
            cboMesa.DropDownStyle = ComboBoxStyle.DropDownList;
            dpFecRes.Value = DateTime.Today;
        }

        private void limpiarCampos()
        {
            dataGridDatos.DataSource = null;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtAmaterno.Text = string.Empty;
            txtFecRes.Text = string.Empty;
            txtCantPersonas.Text = string.Empty;
            txtHora.Text = string.Empty;
            cboMesa.DataSource = null;
            txtRut.Text = string.Empty;
            dpFecRes.Value = DateTime.Today;

        }

        private void buscarReserva()
        {
            try
            {
                conexion.Open();
                OracleCommand comando = new OracleCommand("SP_LISTAR_RESERVA_TOTEM", conexion);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.Parameters.Add("V_RUT_CLIENTE", txtRut.Text);
                comando.Parameters.Add("V_FECHA", (DateTime)dpFecRes.Value);
                comando.Parameters.Add("RESERVA1", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataAdapter adaptador = new OracleDataAdapter();
                adaptador.SelectCommand = comando;
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridDatos.DataSource = tabla;
                conexion.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Lo sentimos. No se ha encontrado ninguna reserva para el rut y la fecha ingresada.");
                conexion.Close();
            }

        }

        private void btnInfoReserva_Click(object sender, EventArgs e)
        {
            buscarReserva();
        }

        private int guardarIdOrden()
        {
            conexion.Open();
            OracleCommand cmd = new OracleCommand("select max(ORDEN_ID) from ORDEN ", conexion);
            int seq_currval = (Convert.ToInt32(cmd.ExecuteScalar()));
            conexion.Close();
            return seq_currval;
        }

        private void btnComenzar_Click(object sender, EventArgs e)
        {
            try
            {
                var principalForm = Application.OpenForms.OfType<Login>().FirstOrDefault();

                WsReserva.AgregarReservaClient agregar = new WsReserva.AgregarReservaClient();
                foreach (DataGridViewRow fila in dataGridDatos.Rows)
                {
                    if (Convert.ToDateTime(fila.Cells[3].Value.ToString()) == DateTime.Today)
                    {
                        
                        if (agregar.agr(txtRut.Text, principalForm.guardarRutTrabajador(), int.Parse(cboMesa.SelectedValue.ToString())) == true)
                        {
                            MessageBox.Show("Mesa asignada correctamente. Su código de verificación es: " + guardarIdOrden().ToString());
                            limpiarCampos();
                        }
                    }
                    else
                    {
                        limpiarCampos();
                        MessageBox.Show("Lo sentimos su reserva ha expirado o aún no es la fecha asignada.");
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error al asignar mesa. Por favor verifique los campos");
            }
        }

        private void cargarDisponibilidadMesas()
        {
            conexion.Open();
            OracleCommand comando = new OracleCommand("SP_BUSCAR_MESA_TOTEM", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("V_CANT_PERSONAS", int.Parse(txtCantPersonas.Text));
            comando.Parameters.Add("MESA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            OracleDataAdapter adaptador = new OracleDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            cboMesa.DataSource = tabla;
            cboMesa.DisplayMember = "NRO_MESA";
            cboMesa.ValueMember = "NRO_MESA";
            conexion.Close();
        }

        private void dataGridDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtNombre.Text = dataGridDatos.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtApellido.Text = dataGridDatos.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtAmaterno.Text = dataGridDatos.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtFecRes.Text = dataGridDatos.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtCantPersonas.Text = dataGridDatos.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtHora.Text = dataGridDatos.Rows[e.RowIndex].Cells[5].Value.ToString();
            cargarDisponibilidadMesas();
        }
    }
}
