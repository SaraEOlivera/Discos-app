﻿using System;
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

        //variables pagination
        private int paginaActual = 1;
        private int DiscosPorPagina = 6;
        private List<Disco> listaDiscos;

        public frmDiscos()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Banda");
            cboCampo.Items.Add("Titulo");
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
                cargarGrillaConPaginas();

                if(listaDiscos.Count > 0)
                    cargarImagen(listaDiscos[0].UrlImagenTapa);

                ocultarColumnas();

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
            dgvDiscos.Columns["FechaLanzamiento"].Visible = false;
            dgvDiscos.Columns["Activo"].Visible = false;
            dgvDiscos.Columns["Tipo"].Visible = false;
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
                listaFiltrada = listaDiscos.FindAll(x => x.Titulo.ToUpper().Contains(filtro.ToUpper()) || x.Estilo.Descripcion.ToUpper().Contains(filtro.ToUpper()) || x.Bandas.Nombre.ToUpper().Contains(filtro.ToUpper()));
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
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con ");
                cboCriterio.Items.Add("Termina con ");
                cboCriterio.Items.Add("Contiene ");

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
                btnAnterior.Enabled = false;
                btnSiguiente.Enabled = false;
            }
            else
            {
                btnModificar.Enabled = true;
                btnEliminacionLogica.Enabled = true;
                btnEliminacionFisica.Enabled = true;
                btnAnterior.Enabled = paginaActual > 1;
                btnSiguiente.Enabled = true;
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            //cargar();
            paginaActual = 1;
            cargarGrillaConPaginas();
            habilitarBotones();
            btnVolver.Visible = false;
        }

        private void dgvDiscos_CellDoubleClick(object sender, DataGridViewCellEventArgs evento)
        {
            if (evento.RowIndex >= 0) 
            {
                Disco fila = (Disco)dgvDiscos.CurrentRow.DataBoundItem;
                frmDetalle vista = new frmDetalle(fila);
                vista.ShowDialog();
            }
        }
       
        private void cargarGrillaConPaginas()
        {
            if (listaDiscos == null || listaDiscos.Count == 0)
                return;

            int inicio = (paginaActual - 1) * DiscosPorPagina;
            int fin = Math.Min(inicio + DiscosPorPagina, listaDiscos.Count);

            List<Disco> listaConPaginas = listaDiscos.GetRange(inicio, fin - inicio);

            dgvDiscos.DataSource = null;
            dgvDiscos.DataSource = listaConPaginas;
            dgvDiscos.Columns["Bandas"].DisplayIndex = 0;

            lblPagina.Text = $"Página {paginaActual} de {Math.Ceiling((double)listaDiscos.Count / DiscosPorPagina)}";

            btnAnterior.Enabled = paginaActual > 1; //expresion booleana
            btnSiguiente.Enabled = fin < listaDiscos.Count;
            ocultarColumnas();
        }   

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1) 
            {
                paginaActual --;
                cargarGrillaConPaginas();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (paginaActual * DiscosPorPagina < listaDiscos.Count) 
            {
                paginaActual++;
                cargarGrillaConPaginas();
            }
        }
    }


}
