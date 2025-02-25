﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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


        //cuerpo de expresion para la funcion
        private void btnCancelar_Click(object sender, EventArgs e) => Close();

        //validar campos 
        private bool validarCamposAlta() 
        {
            DateTime lanzamiento;

            if (string.IsNullOrEmpty(txtBanda.Text))
            {
                MessageBox.Show("El campo Banda es obligatorio");
                return true;
            }

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

        private bool validarRepetidos(string nombre, string imagen)
        {
            DiscosDatos datos = new DiscosDatos();
            List<Disco> listaDiscos = datos.listarDiscos();
            string placeholder = "https://t4.ftcdn.net/jpg/06/71/92/37/360_F_671923740_x0zOL3OIuUAnSF6sr7PuznCI5bQFKhI0.jpg";

            foreach (Disco disco in listaDiscos)
            {
                if (disco != null && disco.Id == disco.Id)
                    continue;

                if (disco.Titulo.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Este disco ya está registrado");
                    return false;
                }
                //if (!(string.IsNullOrEmpty(imagen)) && !(imagen.Trim().Equals(placeholder.Trim())) && disco.UrlImagenTapa == imagen) 
                if (!string.IsNullOrEmpty(imagen) && !imagen.Trim().Equals(placeholder.Trim(), StringComparison.OrdinalIgnoreCase) && disco.UrlImagenTapa.Trim().Equals(imagen.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Esta tapa de disco ya está registrada");
                    return false;
                }
            }
            return true;
        }

        private bool validarFechaLanzamiento(DateTime fecha) 
        {
            //leer valores
            DateTime fechaMinima = DateTime.Parse(ConfigurationManager.AppSettings["FechaLanzamientoMinima"]);
            int diasFuturosPermitidos = int.Parse(ConfigurationManager.AppSettings["DiasFuturosPermitidos"]);
            DateTime fechaMaxima = DateTime.Now.AddDays(diasFuturosPermitidos);

            if (fecha < fechaMinima || fecha > fechaMaxima) 
            {
                MessageBox.Show($"La fecha de lanzamiento debe ser entre {fechaMinima.ToShortDateString()} y {fechaMaxima.ToShortDateString()}", "Fecha Inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool validarRangoCanciones(int cantidadCanciones) 
        {
            int cantidadMinima = int.Parse(ConfigurationManager.AppSettings["CantidadMinimaCanciones"]);
            int cantidadMaxima = int.Parse(ConfigurationManager.AppSettings["CantidadMaximaCanciones"]);

            if (cantidadCanciones < cantidadMinima || cantidadCanciones > cantidadMaxima) 
            {
                MessageBox.Show($"La cantidad de canciones debe oscilar entre {cantidadMinima} y {cantidadMaxima}.", "Rango inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
           //Disco nuevoDisco = new Disco();
            DiscosDatos datos = new DiscosDatos();
            string imagenUrl = txtUrlImagen.Text;
            string nombre = txtTitulo.Text;
            DateTime fechaLanzamiento;
            int cantidadCanciones;

            try
            {
                if (validarCamposAlta())
                    return;

                if (!DateTime.TryParse(txtFechaLanzamiento.Text, out fechaLanzamiento) || !validarFechaLanzamiento(fechaLanzamiento))
                    return;

                if(! int.TryParse(txtCanciones.Text, out cantidadCanciones) || !validarRangoCanciones(cantidadCanciones))
                    return;

                if (!(validarRepetidos(nombre, imagenUrl)))
                    return;

                if (disco == null) 
                    disco = new Disco();

                //Cargar el objeto con los datos
                disco.Titulo = txtTitulo.Text;
                disco.CantidadCanciones = int.Parse(txtCanciones.Text);
                disco.FechaLanzamiento = DateTime.Parse(txtFechaLanzamiento.Text);

                //validar img placeholder - url valida
                if (string.IsNullOrWhiteSpace(imagenUrl) || !(validarUrl(imagenUrl) || validarRutaLocal(imagenUrl)))
                    disco.UrlImagenTapa = "https://t4.ftcdn.net/jpg/06/71/92/37/360_F_671923740_x0zOL3OIuUAnSF6sr7PuznCI5bQFKhI0.jpg";
                else
                    disco.UrlImagenTapa = txtUrlImagen.Text;

                //Desplegable 
                disco.Estilo = (Estilo)cboEstilo.SelectedItem;
                disco.Tipo = (Tipo)cboTipoEdicion.SelectedItem;

                disco.Bandas = new Bandas();
                disco.Bandas.Nombre = txtBanda.Text;
                //disco.Bandas.Nombre = string.IsNullOrEmpty(txtBanda.Text) ? "Nombre de la banda" : txtBanda.Text;   

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

        private bool validarRutaLocal(string rutaLocal)
        {
            return File.Exists(rutaLocal);
        }

        private bool validarUrl(string imagenUrl)
        {
            return Uri.TryCreate(imagenUrl, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private void frmAltaDisco_Load(object sender, EventArgs e)
        {
            Bandas bandas = new Bandas();
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
                    txtBanda.Text = disco.Bandas.Nombre;
                    txtTitulo.Text = disco.Titulo;
                    txtCanciones.Text = disco.CantidadCanciones.ToString();
                    //mostrar la fecha sin hora
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
