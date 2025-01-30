using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace NegocioDiscos
{
    public class AccesoDatos : IDisposable
    {
        //Atributos para hacer la lectura
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        //leer Lector desde el exterior
        public SqlDataReader Lector
        {
            get { return lector; }
        }

        //Constructor
        public AccesoDatos()
        {
            conexion = new SqlConnection("server = .\\SQLEXPRESS; database = DISCOS_DB; Integrated Security=true");
            comando = new SqlCommand();
        }

        public void setearConsulta(string consulta)
        {
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = consulta;
        }

        public void ejecutarConsulta()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ejecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }

        public void cerrarConexion()
        {
            if (lector != null)
                lector.Close();
            conexion.Close();
        }

        //liberar recursos
        public void Dispose()
        {
            cerrarConexion();
            if (comando != null)
                comando.Dispose();
            if (conexion != null)
                conexion.Dispose();
        }

        public void limpiarParametros()
        {
            comando.Parameters.Clear();
        }

        public object ejecutarEscalar()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                return comando.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int obtenerIdBanda(string nombreBanda)
        {
            using (AccesoDatos datosDeAcceso = new AccesoDatos())
            {
                int idBanda = -1;

                //Verificar si la banda ya existe para asignar el mismo id:
                datosDeAcceso.setearConsulta("Select Id from Bandas Where Nombre = @NombreBanda");
                datosDeAcceso.setearParametro("@NombreBanda", nombreBanda);
                datosDeAcceso.ejecutarConsulta();

                if (datosDeAcceso.Lector.Read())
                {
                    idBanda = (int)datosDeAcceso.Lector["Id"];
                }
                else //Si no existe, insertar una nueva banda y obtener el id
                {
                    datosDeAcceso.cerrarConexion();
                    datosDeAcceso.limpiarParametros();

                    datosDeAcceso.setearConsulta(@"INSERT INTO Bandas (Nombre) VALUES (@NombreBanda); SELECT SCOPE_IDENTITY();");
                    datosDeAcceso.setearParametro("@NombreBanda", nombreBanda);
                    idBanda = Convert.ToInt32(datosDeAcceso.ejecutarEscalar());
                }
                return idBanda;
            }
        }


    }
}
