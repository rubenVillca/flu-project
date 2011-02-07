using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Servidor_Sockets;

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
        }
    }
}
