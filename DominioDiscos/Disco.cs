using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominioDiscos
{
    public class Disco
    {
        [DisplayName("Identificador")]
        public int Id { get; set; }

        [DisplayName("Título")]
        public string Titulo { get; set; }

        [DisplayName("Cantidad de Canciones")]
        public int CantidadCanciones { get; set; }
        public string UrlImagenTapa { get; set; }
        public Estilo Estilo { get; set; }

        [DisplayName("Tipo Edición")]
        public Tipo Tipo { get; set; }
    
        [DisplayName("Fecha de Lanzamiento")]
        public DateTime? FechaLanzamiento { get; set; }
        public bool? Activo { get; set; }
        public Bandas Bandas { get; set; }

    }   
}
