using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NegocioDiscos;
using Practica_Discos;
using DominioDiscos;

namespace Practica_Discos
{
    public partial class frmDetalle : Form
    {
        private Disco seleccionado;
        public frmDetalle()
        {
            InitializeComponent();
        }

        public frmDetalle(Disco disco)
        {
            InitializeComponent();
            this.seleccionado = disco;
        }

        private void frmDetalle_Load(object sender, EventArgs e)
        {
            txtTituloDetalle.Text = seleccionado.Titulo;
            txtLanzamientoDetalle.Text = seleccionado.FechaLanzamiento.HasValue 
                ? seleccionado.FechaLanzamiento.Value.ToString("dd/MM/yyyy")
                : "Sin informacion sobre la fecha de lanzamiento";
            txtCantCancionesDetalle.Text = seleccionado.CantidadCanciones.ToString();
            txtEstiloDetalle.Text = seleccionado.Estilo.Descripcion;
            txtTipoEdicionDetalle.Text = seleccionado.Tipo.Descripcion;
            txtActivoDetalle.Text = seleccionado.Activo.HasValue
                ? (seleccionado.Activo.Value ? "Disco Registrado" : "Disco dado de baja")
                : "Sin informacion sobre el registro de este disco";


            //Mostrar la imagen en la vista
            if (!(string.IsNullOrEmpty(seleccionado.UrlImagenTapa))) 
            {
                try
                {
                    pboImagenDetalle.Load(seleccionado.UrlImagenTapa);
                }
                catch (Exception)
                {
                    pboImagenDetalle.Load("https://t4.ftcdn.net/jpg/06/71/92/37/360_F_671923740_x0zOL3OIuUAnSF6sr7PuznCI5bQFKhI0.jpg");
                }
            }

        }

        private void btnVolverDetalle_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
