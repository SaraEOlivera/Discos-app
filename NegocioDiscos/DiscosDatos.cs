using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DominioDiscos;
using System.Data.Common;

namespace NegocioDiscos
{
    public class DiscosDatos
    {
        public List<Disco> listarDiscos() 
        {
			List<Disco>lista = new List<Disco>();

			SqlConnection conexion = new SqlConnection();
			SqlCommand comando = new SqlCommand();
			SqlDataReader lector;

			try
			{
				conexion.ConnectionString = "server = .\\SQLEXPRESS; database = DISCOS_DB; Integrated Security=true";
				comando.CommandType = System.Data.CommandType.Text;
				comando.CommandText = "select D.Id, Titulo, FechaLanzamiento ,CantidadCanciones, UrlImagenTapa, E.Descripcion as Estilo, T.Descripcion as Tipo, D.idEstilo, D.IdTipoEdicion, Activo from DISCOS D, ESTILOS E, TIPOSEDICION T where E.Id = D.IdEstilo and T.Id = D.IdTipoEdicion AND Activo = 1";
				comando.Connection = conexion;

				conexion.Open();
				lector = comando.ExecuteReader();

				while (lector.Read())
				{
					Disco auxiliar = new Disco();
					auxiliar.Id = (int)lector["Id"];
					auxiliar.Titulo = (string)lector["Titulo"];

					if (lector["FechaLanzamiento"] is DBNull)
						auxiliar.FechaLanzamiento = null;
					else
                        auxiliar.FechaLanzamiento = (DateTime)lector["FechaLanzamiento"];

                    auxiliar.CantidadCanciones = (int)lector["CantidadCanciones"];

                    if (!(lector["UrlImagenTapa"] is DBNull))
                        auxiliar.UrlImagenTapa = (string)lector["UrlImagenTapa"];

					auxiliar.Estilo = new Estilo();
					auxiliar.Estilo.Id = (int)lector["IdEstilo"];
					auxiliar.Estilo.Descripcion = (string)lector["Estilo"];

                    auxiliar.Tipo = new Tipo();
                   auxiliar.Tipo.Id = (int)lector["IdTipoEdicion"];
                    auxiliar.Tipo.Descripcion = (string)lector["Tipo"];
					auxiliar.Activo = (bool)lector["Activo"];

					lista.Add(auxiliar);
				}
				return lista;
			}
			catch (Exception)
			{
				throw;
			}

			finally 
			{
				conexion.Close();
			}
        }

		public void agregar(Disco nuevo) 
		{
			AccesoDatos datosDeAcceso = new AccesoDatos();
			try
			{
                //datosDeAcceso.setearConsulta("INSERT INTO DISCOS (Titulo, FechaLanzamiento ,CantidadCanciones, Activo, IdEstilo, IdTipoEdicion, UrlImagenTapa) VALUES ('" + nuevo.Titulo + "', " + nuevo.CantidadCanciones + ", " + " 1, " + "@IdEstilo, " + "@IdTipoEdicion, " + "@UrlImagenTapa)");
                datosDeAcceso.setearConsulta("INSERT INTO DISCOS (Titulo, FechaLanzamiento, CantidadCanciones, Activo, IdEstilo, IdTipoEdicion, UrlImagenTapa) VALUES (@Titulo,@FechaLanzamiento, @CantidadCanciones,  1, @IdEstilo, @IdTipoEdicion, @UrlImagenTapa)");

				datosDeAcceso.setearParametro("@Titulo", nuevo.Titulo);
				datosDeAcceso.setearParametro("@FechaLanzamiento", nuevo.FechaLanzamiento);
				datosDeAcceso.setearParametro("@CantidadCanciones", nuevo.CantidadCanciones);
                datosDeAcceso.setearParametro("@IdEstilo", nuevo.Estilo.Id);
				datosDeAcceso.setearParametro("@IdTipoEdicion", nuevo.Tipo.Id);
				datosDeAcceso.setearParametro("@UrlImagenTapa", nuevo.UrlImagenTapa);
                datosDeAcceso.ejecutarAccion();
			}
			catch (Exception)
			{
				throw;
			}
			finally 
			{
				datosDeAcceso.cerrarConexion();
			}
		}

		public void modificar(Disco disc)
		{
			AccesoDatos datos = new AccesoDatos();
			try
			{
				datos.setearConsulta("Update DISCOS Set Titulo = @titulo, FechaLanzamiento = @fechaLanzamiento , CantidadCanciones = @cantidadCanciones ,UrlImagenTapa = @urlImagenTapa, IdEstilo = @idEstilo, IdTipoEdicion = @idTipoEdicion where Id = @id");

				//parametros
				datos.setearParametro("@titulo", disc.Titulo);
				datos.setearParametro("@fechaLanzamiento", disc.FechaLanzamiento);
				datos.setearParametro("@cantidadCanciones", disc.CantidadCanciones);
				datos.setearParametro("@urlImagenTapa", disc.UrlImagenTapa);
				datos.setearParametro("@idEstilo", disc.Estilo.Id);
				datos.setearParametro("@idTipoEdicion", disc.Tipo.Id);
				datos.setearParametro("@id", disc.Id);

				datos.ejecutarAccion();

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

		public void eliminar(int id) 
		{
			try
			{
				AccesoDatos datos = new AccesoDatos();
				datos.setearConsulta("Delete From DISCOS Where Id = @id");
				datos.setearParametro("@id", id);
				datos.ejecutarAccion();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void eliminarLogico(int id) 
		{
			try
			{
				AccesoDatos datos = new AccesoDatos();
				datos.setearConsulta("Update DISCOS Set Activo = 0 where Id = @id");
				datos.setearParametro("@id", id);
				datos.ejecutarAccion();
			}
			catch (Exception ex)
			{

				throw ex;
			}
		}

        public List<Disco> filtrar(string campo, string criterio, string filtro)
        {
			List<Disco> lista = new List<Disco> ();
			AccesoDatos datos = new AccesoDatos();
			try
			{
				string consulta = "select D.Id, Titulo, CantidadCanciones, UrlImagenTapa, E.Descripcion as Estilo, T.Descripcion as Tipo, D.idEstilo, D.IdTipoEdicion from DISCOS D, ESTILOS E, TIPOSEDICION T where E.Id = D.IdEstilo and T.Id = D.IdTipoEdicion AND Activo = 1 AND ";
				if (campo == "Titulo")
				{
					switch (criterio)
					{
						case "Comienza con":
							consulta += "Titulo like ' " + filtro + "%' ";
							break;
						case "Termina con":
							consulta += "Titulo like ' %" + filtro + " ' ";
							break;
						default:
							consulta += "Titulo like '%" + filtro + "%' ";
							break;
					}
				}
				else if (campo == "Cant. de Canciones")
				{
					switch (criterio)
					{
						case "Mayor a":
							consulta += "CantidadCanciones >" + filtro;
							break;
						case "Menor a":
							consulta += "CantidadCanciones <" + filtro;
							break;
						default:
							consulta += "CantidadCanciones =" + filtro;
							break;

					}
				}
				else 
				{
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "E.Descripcion like ' " + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "E.Descripcion like ' %" + filtro + " ' ";
                            break;
                        default:
                            consulta += "E.Descripcion like '%" + filtro + "%' ";
                            break;
                    }
                }

				datos.setearConsulta(consulta);
				datos.ejecutarConsulta();

                while (datos.Lector.Read())
                {
                    Disco auxiliar = new Disco();
                    auxiliar.Id = (int)datos.Lector["Id"];
                    auxiliar.Titulo = (string)datos.Lector["Titulo"];
                    auxiliar.CantidadCanciones = (int)datos.Lector["CantidadCanciones"];


                    if (!(datos.Lector["UrlImagenTapa"] is DBNull))
                        auxiliar.UrlImagenTapa = (string)datos.Lector["UrlImagenTapa"];

                    auxiliar.Estilo = new Estilo();
                    auxiliar.Estilo.Id = (int)datos.Lector["IdEstilo"];
                    auxiliar.Estilo.Descripcion = (string)datos.Lector["Estilo"];

                    auxiliar.Tipo = new Tipo();
                    auxiliar.Tipo.Id = (int)datos.Lector["IdTipoEdicion"];
                    auxiliar.Tipo.Descripcion = (string)datos.Lector["Tipo"];

                    lista.Add(auxiliar);
                }

                return lista;
	
			}
			catch (Exception ex)
			{
				throw ex;
			}
        }
    }
}
