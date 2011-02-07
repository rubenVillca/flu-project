using System;
namespace Servidor_Sockets
{
    /// <summary>
    /// Interfaz que todo plugin debe implementar
    /// </summary>
    public interface IServerPlugin
    {
        void InicializarPlugin(ServidorFlu Servidor);
    }

    /// <summary>
    /// Todas las clases que deseen ser plugins del servidor, deben ademas implementar este atributo
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServerPluginAttribute : Attribute 
    {
        public ServerPluginAttribute(String Descripcion) 
        {
            Description = Descripcion;
        }

        public String Description { get; set; }
    }
}
