using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flu_Rewrite
{
    public static class Extensiones
    {
        public static void Limpiar(this Byte[] Arreglo)
        {
            Arreglo = new Byte[Arreglo.Length];
        }
    }
}
