using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Servidor_Sockets;
using System.IO;

namespace ConsolePlugin
{
    [ServerPlugin("Solo imprime cuando se conecta un cliente")]
    public class Plugin : IServerPlugin
    {
        private ServidorFlu Servidor;

        public void InicializarPlugin(ServidorFlu Servidor)
        {
            this.Servidor = Servidor;
            Servidor.ClienteConectado += new EventHandler<ClienteConectado>(ClienteConectado);
        }

        void ClienteConectado(object sender, ClienteConectado e)
        {
            Console.WriteLine("Un cliente se ha conectado desde:" + e.Direccion.ToString());
            TextWriter TW = new StreamWriter(new FileStream("Log.txt", FileMode.OpenOrCreate));
            TW.WriteLine("Cliente conectado desde:" + e.Direccion.ToString());
        }
    }
}
