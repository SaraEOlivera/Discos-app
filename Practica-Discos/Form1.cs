using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DominioDiscos;
using NegocioDiscos;
using Utilidades;

namespace Practica_Discos
{
    public partial class frmDiscos : Form
    {
        public frmDiscos()
        {
            InitializeComponent();
        }

        private List<Disco> listaDiscos;
        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Titulo");
            cboCampo.Items.Add("Cant. de Canciones");
            cboCampo.Items.Add("Estilo");
            habilitarBotones();
        }

        private void dgvDiscos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDiscos.CurrentRow != null) 
            {
                Disco seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagenTapa);
            }
            habilitarBotones();
            btnVolver.Visible = false;
        }

        private void cargar() 
        {
            DiscosDatos datos = new DiscosDatos();

            try
            {
                listaDiscos = datos.listarDiscos();
                dgvDiscos.DataSource = listaDiscos;
                ocultarColumnas();

                cargarImagen(listaDiscos[0].UrlImagenTapa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas() 
        {
            dgvDiscos.Columns["UrlImagenTapa"].Visible = false;
            dgvDiscos.Columns["Id"].Visible = false;
        }

        private void cargarImagen(string imagen) 
        {
            try
            {
                pbxDiscos.Load(imagen);

            }
            catch (Exception ex)
            {
                pbxDiscos.Load("https://t4.ftcdn.net/jpg/06/71/92/37/360_F_671923740_x0zOL3OIuUAnSF6sr7PuznCI5bQFKhI0.jpg");
            }
        }

        private void dgvDiscos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaDisco alta = new frmAltaDisco();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Disco seleccionado;
            seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem;

            frmAltaDisco modificar = new frmAltaDisco(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminacionFisica_Click(object sender, EventArgs e)
        {
            eliminar();
            habilitarBotones();
        }

        private void btnEliminacionLogica_Click(object sender, EventArgs e)
        {
            eliminar(true);
            habilitarBotones();
        }

        private void eliminar(bool logico = false) 
        {
            DiscosDatos datos = new DiscosDatos();
            Disco seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("¿De verdad querés eliminar un disco?", "Eliminar Disco", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem;
                    if (logico)
                        datos.eliminarLogico(seleccionado.Id);
                    else
                        datos.eliminar(seleccionado.Id);

                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro() 
        {
            if (cboCampo.SelectedIndex < 0) 
            {
                MessageBox.Show("Seleccione una opción dentro de Campo");
                return true;
            }
            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione una opción dentro de Criterio");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Cant. de Canciones") 
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debe completar el campo filtro");
                    return true;
                }

                if (!(Validaciones.soloNumeros(txtFiltroAvanzado.Text))) 
                {
                    MessageBox.Show("Debe ingresar solo números");
                    return true;
                }
            }
            return false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            List<Disco> listaFiltrada;
            string filtro = txtFiltroRapido.Text;
            if (filtro.Length >= 3)
                listaFiltrada = listaDiscos.FindAll(x => x.Titulo.ToUpper().Contains(filtro.ToUpper()) || x.Estilo.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            else
                listaFiltrada = listaDiscos;

            dgvDiscos.DataSource = null;
            dgvDiscos.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void btnFiltroRapido_Click(object sender, EventArgs e)
        {

        }

        private void txtFiltroAvanzado_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboCriterio_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCampo.SelectedItem != null)
            {
                string opcion = cboCampo.SelectedItem.ToString();
                if (opcion == "Cant. de Canciones")
                {
                    cboCriterio.Items.Clear();
                    cboCriterio.Items.Add("Mayor a");
                    cboCriterio.Items.Add("Menor a");
                    cboCriterio.Items.Add("Igual a");
                }
                else
                {
                    cboCriterio.Items.Clear();
                    cboCriterio.Items.Add("Comienza con ");
                    cboCriterio.Items.Add("Termina con ");
                    cboCriterio.Items.Add("Contiene ");
                }
            }
            else
            {
                cboCriterio.Items.Clear();
            }
        }

        private void btnFiltroAvanzado_Click(object sender, EventArgs e)
        {
            DiscosDatos datos = new DiscosDatos();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                dgvDiscos.DataSource = datos.filtrar(campo, criterio, filtro);
                btnVolver.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            habilitarBotones();
            //limpiar el filtro
            txtFiltroAvanzado.Text = string.Empty;
            if (cboCampo != null && cboCampo.Items.Count > 0)
                cboCampo.SelectedIndex = -1;

            if (cboCriterio != null && cboCriterio.Items.Count > 0)
                cboCriterio.SelectedIndex = -1;
        }

        private void habilitarBotones() 
        {
            if (dgvDiscos.Rows.Count == 0)
            {
                btnModificar.Enabled = false;
                btnEliminacionLogica.Enabled = false;
                btnEliminacionFisica.Enabled = false;
            }
            else
            {
                btnModificar.Enabled = true;
                btnEliminacionLogica.Enabled = true;
                btnEliminacionFisica.Enabled = true;
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            cargar();
            btnVolver.Visible = false;
        }

        private void dgvDiscos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //se abre la vista detallada
        }
    }

 
}
