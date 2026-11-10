using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace InstaladorCid
{
    internal static class Programa
    {
        private const string Nombre = "El Cantar de Mio Cid";
        private static readonly byte[] Firma = Encoding.ASCII.GetBytes("CIDSETUP");

        private static void Acceso(string ruta, string destino, string directorio)
        {
            object shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
            object acceso = shell.GetType().InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { ruta });
            acceso.GetType().InvokeMember("TargetPath", BindingFlags.SetProperty, null, acceso, new object[] { destino });
            acceso.GetType().InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, acceso, new object[] { directorio });
            acceso.GetType().InvokeMember("Save", BindingFlags.InvokeMethod, null, acceso, new object[0]);
        }

        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                DialogResult resultado = MessageBox.Show("Se instalará el juego con sus 102 ilustraciones en tu carpeta de aplicaciones. ¿Continuar?", Nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resultado != DialogResult.Yes) return;
                string destino = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Nombre);
                Directory.CreateDirectory(destino);
                using (FileStream instalador = File.OpenRead(Assembly.GetExecutingAssembly().Location))
                using (BinaryReader lector = new BinaryReader(instalador))
                {
                    instalador.Seek(-16, SeekOrigin.End);
                    long longitud = lector.ReadInt64();
                    byte[] firma = lector.ReadBytes(8);
                    if (Encoding.ASCII.GetString(firma) != Encoding.ASCII.GetString(Firma) || longitud <= 0 || longitud > int.MaxValue || longitud > instalador.Length - 16)
                        throw new InvalidDataException("El paquete de instalación está incompleto.");
                    instalador.Seek(-16 - longitud, SeekOrigin.End);
                    byte[] contenido = lector.ReadBytes(checked((int)longitud));
                    if (contenido.Length != longitud) throw new InvalidDataException("No se pudo leer el paquete completo.");
                    using (MemoryStream memoria = new MemoryStream(contenido))
                    using (ZipArchive paquete = new ZipArchive(memoria, ZipArchiveMode.Read))
                    {
                        int ilustraciones = 0;
                        foreach (ZipArchiveEntry archivo in paquete.Entries)
                        {
                            if (string.IsNullOrEmpty(archivo.Name)) continue;
                            string salida = Path.GetFullPath(Path.Combine(destino, archivo.FullName.Replace('/', Path.DirectorySeparatorChar)));
                            if (!salida.StartsWith(destino.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                                throw new InvalidDataException("El paquete contiene una ruta no válida.");
                            Directory.CreateDirectory(Path.GetDirectoryName(salida));
                            using (Stream origen = archivo.Open())
                            using (FileStream nuevo = File.Create(salida)) origen.CopyTo(nuevo);
                            if (archivo.FullName.StartsWith("imagenes/", StringComparison.OrdinalIgnoreCase) && archivo.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) ilustraciones++;
                        }
                        if (ilustraciones != 102 || !File.Exists(Path.Combine(destino, "ElCid.exe")))
                            throw new InvalidDataException("Faltan archivos del juego en el paquete.");
                    }
                }
                string programas = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), Nombre);
                Directory.CreateDirectory(programas);
                Acceso(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Nombre + ".lnk"), Path.Combine(destino, "ElCid.exe"), destino);
                Acceso(Path.Combine(programas, Nombre + ".lnk"), Path.Combine(destino, "ElCid.exe"), destino);
                Acceso(Path.Combine(programas, "Desinstalar.lnk"), Path.Combine(destino, "desinstalar.cmd"), destino);
                MessageBox.Show("El juego se instaló correctamente. Se abrirá ahora.", Nombre, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start(Path.Combine(destino, "ElCid.exe"));
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, Nombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
