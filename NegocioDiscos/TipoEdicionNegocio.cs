using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominioDiscos;
using NegocioDiscos;

namespace NegocioDiscos
{
    public class TipoEdicionNegocio
    {
        public List<Tipo> listar()
        {
            List<Tipo> listaTipo = new List<Tipo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Select T.Id,  T.Descripcion from TIPOSEDICION T\r\n");
                datos.ejecutarConsulta();

                while (datos.Lector.Read())
                {
                    Tipo auxiliarTipo = new Tipo();
                    auxiliarTipo.Id = (int)datos.Lector["Id"];
                    auxiliarTipo.Descripcion = (string)datos.Lector["Descripcion"];

                    listaTipo.Add(auxiliarTipo);
                }
                return listaTipo;
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

