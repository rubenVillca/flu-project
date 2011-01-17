using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Servidor_Sockets
{
    class ServidorFlu
    {
        private List<TcpClient> Clientes;
        private TcpListener Servidor;
        //Avisa al plugin de control que hay un nuveo cliente
        public event EventHandler ClienteConectado;

        public ServidorFlu()
        {
            Servidor = new TcpListener(21); Servidor.Start();
            Clientes = new List<TcpClient>();
            (new Thread(new ThreadStart(AceptarConexiones))).Start();
        }

        //Como no hay un delegado para conexiones entrantes toca hacerlo de esta manera XD
        public void AceptarConexiones()
        {
            while (true)
            {
                //Esta instruccion bloquea el hilo hasta que un nuevo cliente se conecte :)
                Clientes.Add(Servidor.AcceptTcpClient());
                OnClienteConectado();
            }
        }

        /// <summary>
        /// Envia un comando al cliente seleccionado
        /// </summary>
        /// <param name="Cliente"></param>
        /// <param name="Comando"></param>
        /// <param name="Parametros"></param>
        /// <param name="Interno"></param>
        /// <returns></returns>
        public String EnviarComando(TcpClient Cliente, String Comando, String Parametros, Boolean Interno)
        {
            try
            {
                Byte[] Respuesta = new Byte[20480];
                //Alertar al cliente
                //Condicion ? Se cumple : No se cumple :)
                Cliente.Client.Send(Encoding.ASCII.GetBytes(Interno ? "Comando" : "Comando Externo"));
                //Enviar el comando
                Cliente.Client.Send(Encoding.ASCII.GetBytes(Comando));
                Cliente.Client.Send(Encoding.ASCII.GetBytes(Parametros));
                //Esto bloquea el hilo hasta que se reciba una respuesta, por tanto se retorna "" si el tiempo limite se excede
                Cliente.Client.Receive(Respuesta);
                return Encoding.ASCII.GetString(Respuesta);
            }
            catch { return ""; }
        }

        //TODO: Hacer que este delegado envie datos del nuevo cliente
        public virtual void OnClienteConectado()
        {
            EventHandler Delegado = ClienteConectado;
            if (Delegado != null) Delegado(this, null);
        }
    }
}
