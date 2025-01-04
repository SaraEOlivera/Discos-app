using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominioDiscos;

namespace NegocioDiscos
{
    public class EstiloNegocio
    {
        public List<Estilo> listar()
        {
            List<Estilo>lista = new List<Estilo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Select id, descripcion From ESTILOS");
                datos.ejecutarConsulta();

                while (datos.Lector.Read())
                {
                    Estilo auxiliar = new Estilo();
                    auxiliar.Id = (int)datos.Lector["Id"];
                    auxiliar.Descripcion = (string)datos.Lector["Descripcion"];

                    lista.Add(auxiliar);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                datos.cerrarConexion();
            }
        }
    }
}
