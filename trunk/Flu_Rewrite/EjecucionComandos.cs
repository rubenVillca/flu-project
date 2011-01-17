using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flu_Rewrite
{
    class EjecucionComandos
    {
        private static System.Diagnostics.Process cmd;

        //TODO: Ahorrar espacio al controlar el retorno de datos con un parametro booleano en vez de tener dos metodos?
        public static void Ejecutar(string comando, string argumento)
        {
            cmd = new System.Diagnostics.Process();
            cmd.EnableRaisingEvents = true;
            cmd.Exited += new EventHandler(cmd_Exited);
            cmd.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = comando;
            cmd.StartInfo.Arguments = argumento;
            cmd.Start();
            String salida;
            while ((salida = cmd.StandardOutput.ReadLine()) != null)
                RetornoDatos.RetornarInformacion(salida);
        }

        public static void EjecutarComandoInterno(string comando, string argumento)
        {
            cmd = new System.Diagnostics.Process();
            cmd.EnableRaisingEvents = true;
            cmd.Exited += new EventHandler(cmd_Exited);
            cmd.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = comando;
            cmd.StartInfo.Arguments = argumento;
            cmd.Start();
        }

        private static void cmd_Exited(object sender, EventArgs e)
        {
            //TODO: Se deberia mandar una respuesta cercana a algo como "Evento terminado"
            //Discutirlo con @JAntonioCalles
        }
    }
}
