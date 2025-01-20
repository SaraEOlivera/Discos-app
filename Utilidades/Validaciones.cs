using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public static class Validaciones
    {
        public static bool soloNumeros(string cadena) 
        {
            foreach (char caracter in cadena) 
            {
                if (!(char.IsNumber(caracter))) 
                {
                    return false;
                }
            }
            return true;

        }


    }
}
