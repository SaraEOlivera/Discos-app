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
            //txtLanzamientoDetalle.Text = seleccionado.FechaLanzamiento.Text;
            txtCantCancionesDetalle.Text = seleccionado.CantidadCanciones.ToString();
            txtEstiloDetalle.Text = seleccionado.Estilo.Descripcion;
            txtTipoEdicionDetalle.Text = seleccionado.Tipo.Descripcion;
           // txtActivoDetalle.Text = seleccionado.Activo.ToString();

        }

        private void btnVolverDetalle_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
