using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Flu_Rewrite
{
    //TODO: Exigir encriptacion
    //TODO: Cambiar a UDPServer
    class ClienteSockets
    {

        //Es la magia que nos permite comunicarnos con el servidor :)
        private TcpClient Cliente;
        //Nos permite enviar y recibir datos en la conexion
        private NetworkStream Stream;
        //Sube datos al servidor
        private StreamWriter Escritor;
        //Baja datos del servidor
        private StreamReader Lector;
        //Este delegado se ejecutaria cada vez que recibamos un comando del servidor
        public delegate Boolean ComandoRecibido(Boolean Interno, String Comando, String Parametros);

        //En un futuro se implementara UPnP para poder aleatorizar el puerto :D
        //Por ahora el puerto de FTP es una buena opcion, aunque discutible
        //TODO: Implementar un sistema de Logs
        public ClienteSockets(String Host)
        { Cliente = new TcpClient(Host, 21); Thread Loop = new Thread(new ThreadStart(LoopPrincipal)); Loop.Start(); }

        public void LoopPrincipal()
        {
            Stream = Cliente.GetStream();
            Escritor = new StreamWriter(Stream);
            Lector = new StreamReader(Stream);
            while (true)
            {
                //Si hay un nuevo comando...
                if (Lector.ReadLine() == "Pendiente")
                {
                    //Enviar un mensaje de confirmacion...
                    Escritor.WriteLine("OK");
                    Escritor.Flush();
                    //Leemos el comando...
                    String Comando = Lector.ReadLine();
                    //Deberia haber algo aqui, pero primero tengo que escribir el otro lado de la ecuacion y definir un protocolo XD
                }
            }
        }
    }
}
