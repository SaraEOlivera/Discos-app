using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DominioDiscos;
using NegocioDiscos;
using Utilidades;

namespace Practica_Discos
{
    public partial class frmAltaDisco : Form
    {
        //atributo privado
        private Disco disco = null;
        private OpenFileDialog archivo = null;

        public frmAltaDisco()
        {
            InitializeComponent();
        }

        public frmAltaDisco(Disco disco)
        {
            InitializeComponent();
            this.disco = disco;
            Text = "Modificar Disco";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        //validar campos 
        private bool validarCamposAlta() 
        {
            DateTime lanzamiento;

            if (string.IsNullOrEmpty(txtTitulo.Text)) 
            {
                MessageBox.Show("El campo Título es obligatorio");
                return true;
            }
        
            if (string.IsNullOrEmpty(txtCanciones.Text)) 
            {
                MessageBox.Show("El campo Cantidad de Canciones es obligatorio");
                return true;
            }

            if (!(Validaciones.soloNumeros(txtCanciones.Text))) 
            {
                MessageBox.Show("Debe ingresar solo números en este campo");
                return true;
            }

            if (!(DateTime.TryParse(this.txtFechaLanzamiento.Text, out lanzamiento))) 
            {
                MessageBox.Show("Debe ingresar solo fechas en el campo Fecha");
                return true;
            }

            return false;
        }


        private void btnAceptar_Click(object sender, EventArgs e)
        {
           //Disco nuevoDisco = new Disco();
            DiscosDatos datos = new DiscosDatos();
            try
            {
                if (validarCamposAlta())
                    return;

                if (disco == null) 
                    disco = new Disco();

                //Cargar el objeto con los datos
                disco.Titulo = txtTitulo.Text;
                disco.CantidadCanciones = int.Parse(txtCanciones.Text);
                disco.FechaLanzamiento = DateTime.Parse(txtFechaLanzamiento.Text);
                disco.UrlImagenTapa = txtUrlImagen.Text;
                //Desplegable 
                disco.Estilo = (Estilo)cboEstilo.SelectedItem;
                disco.Tipo = (Tipo)cboTipoEdicion.SelectedItem;

                if (disco.Id != 0)
                {
                    //modificar Disco a la BD
                    datos.modificar(disco);
                    MessageBox.Show("Disco modificado con éxito!");
                }
                else 
                {
                    //agregar disco a la DB
                    datos.agregar(disco);
                    MessageBox.Show("Disco agregado con éxito!");
                }

                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["image-folder"] + archivo.SafeFileName);


                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void frmAltaDisco_Load(object sender, EventArgs e)
        {
            EstiloNegocio estiloNegocio = new EstiloNegocio();
            TipoEdicionNegocio tipoENegocio = new TipoEdicionNegocio();
            try
            {
                cboEstilo.DataSource = estiloNegocio.listar();
                cboEstilo.ValueMember = "Id";
                cboEstilo.DisplayMember = "Descripcion";

                cboTipoEdicion.DataSource = tipoENegocio.listar();
                cboTipoEdicion.ValueMember = "Id";
                cboTipoEdicion.DisplayMember = "Descripcion";

                if (disco != null)
                {
                    txtTitulo.Text = disco.Titulo;
                    txtCanciones.Text = disco.CantidadCanciones.ToString();
                    txtFechaLanzamiento.Text = disco.FechaLanzamiento.HasValue 
                        ? disco.FechaLanzamiento.Value.ToString("dd-MM-yyyy") : string.Empty;
                    txtUrlImagen.Text = disco.UrlImagenTapa;
                    cargarImagen(disco.UrlImagenTapa);

                    cboEstilo.SelectedValue = disco.Estilo.Id;
                    cboTipoEdicion.SelectedValue = disco.Tipo.Id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
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

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo =  new OpenFileDialog();
            archivo.Filter = "jpg|*jpg;|png|*.png";

            if (archivo.ShowDialog() == DialogResult.OK)
            { 
                txtUrlImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["image-folder"] + archivo.SafeFileName); 

            }



        }
    }
}
