using System;
using System.Collections.Generic;
using System.Linq;

namespace Servidor_Sockets
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ServidorFlu SF = new ServidorFlu();
        }
    }
}
