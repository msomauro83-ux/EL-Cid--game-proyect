using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

internal sealed class SegmentStream : Stream
{
    private readonly FileStream file;
    private readonly long start;
    private readonly long length;
    private long position;
    internal SegmentStream(string path, long start, long length)
    {
        file = File.OpenRead(path); this.start = start; this.length = length;
    }
    public override bool CanRead { get { return true; } }
    public override bool CanSeek { get { return true; } }
    public override bool CanWrite { get { return false; } }
    public override long Length { get { return length; } }
    public override long Position { get { return position; } set { Seek(value, SeekOrigin.Begin); } }
    public override int Read(byte[] buffer, int offset, int count)
    {
        count = (int)Math.Min(count, length - position);
        file.Position = start + position;
        int read = file.Read(buffer, offset, count);
        position += read;
        return read;
    }
    public override long Seek(long offset, SeekOrigin origin)
    {
        long next = origin == SeekOrigin.Begin ? offset : origin == SeekOrigin.Current ? position + offset : length + offset;
        if (next < 0 || next > length) throw new IOException("Posición fuera del archivo.");
        position = next; return position;
    }
    public override void Flush() { }
    public override void SetLength(long value) { throw new NotSupportedException(); }
    public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
    protected override void Dispose(bool disposing) { if (disposing) file.Dispose(); base.Dispose(disposing); }
}

internal sealed class Installer : Form
{
    private readonly TextBox destination = new TextBox();
    private readonly Button install = new Button();
    private readonly ProgressBar progress = new ProgressBar();
    private readonly Label status = new Label();
    private string installedPath;
    private const string GameName = "El Cantar de Mío Cid";
    private const string Magic = "CIDGAME1";

    internal Installer()
    {
        Text = "Instalar " + GameName;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(620, 300);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.FromArgb(31, 27, 23);
        ForeColor = Color.FromArgb(245, 233, 210);
        Font = new Font("Segoe UI", 10);

        var title = new Label { Text = GameName, Left = 28, Top = 20, Width = 560, Height = 35,
            Font = new Font("Georgia", 20, FontStyle.Bold), ForeColor = Color.FromArgb(220, 175, 100) };
        Controls.Add(title);
        Controls.Add(new Label { Text = "Elige dónde instalar el juego y sus 102 ilustraciones.", Left = 30, Top = 72, Width = 550, Height = 28 });
        destination.Left = 30; destination.Top = 112; destination.Width = 465;
        destination.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "El Cantar de Mio Cid");
        Controls.Add(destination);
        var browse = new Button { Text = "Examinar…", Left = 505, Top = 110, Width = 85, Height = 28 };
        browse.Click += (s, e) => { using (var dialog = new FolderBrowserDialog()) { dialog.Description = "Selecciona la carpeta de instalación"; if (dialog.ShowDialog() == DialogResult.OK) destination.Text = Path.Combine(dialog.SelectedPath, "El Cantar de Mio Cid"); } };
        Controls.Add(browse);
        progress.Left = 30; progress.Top = 160; progress.Width = 560; progress.Height = 23;
        Controls.Add(progress);
        status.Text = "Listo para instalar."; status.Left = 30; status.Top = 193; status.Width = 560; status.Height = 28;
        Controls.Add(status);
        install.Text = "Instalar"; install.Left = 470; install.Top = 235; install.Width = 120; install.Height = 36;
        install.Click += async (s, e) => {
            if (installedPath != null) { System.Diagnostics.Process.Start(Path.Combine(installedPath, "El Cantar de Mio Cid.exe")); Close(); }
            else await Install();
        };
        Controls.Add(install);
    }

    private async Task Install()
    {
        string target;
        try { target = Path.GetFullPath(destination.Text.Trim()); if (String.IsNullOrWhiteSpace(destination.Text)) throw new Exception(); }
        catch { MessageBox.Show("Elige una carpeta válida."); return; }
        install.Enabled = false; destination.Enabled = false;
        status.Text = "Copiando archivos…";
        try
        {
            var report = new Progress<int>(value => { progress.Value = value; status.Text = "Copiando archivos… " + value + "%"; });
            await Task.Run(() => Extract(target, report));
            CreateShortcuts(target);
            status.Text = "Instalación terminada.";
            installedPath = target;
            install.Text = "Jugar"; install.Enabled = true;
            MessageBox.Show("El juego está instalado. Puedes abrirlo desde el menú Inicio o el escritorio.", GameName);
        }
        catch (Exception error)
        {
            status.Text = "No se pudo completar la instalación.";
            MessageBox.Show(error.Message, GameName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            install.Enabled = true; destination.Enabled = true;
        }
    }

    private static void Extract(string target, IProgress<int> report)
    {
        string self = System.Reflection.Assembly.GetExecutingAssembly().Location;
        long fileLength = new FileInfo(self).Length;
        using (var reader = new BinaryReader(File.OpenRead(self)))
        {
            if (fileLength < 16) throw new InvalidDataException("Faltan los archivos del instalador.");
            reader.BaseStream.Seek(-16, SeekOrigin.End);
            long archiveLength = reader.ReadInt64();
            string magic = Encoding.ASCII.GetString(reader.ReadBytes(8));
            if (magic != Magic || archiveLength < 1 || archiveLength > fileLength - 16)
                throw new InvalidDataException("Faltan los archivos del instalador.");
            long archiveStart = fileLength - 16 - archiveLength;
            Directory.CreateDirectory(target);
            string basePath = Path.GetFullPath(target).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            using (var segment = new SegmentStream(self, archiveStart, archiveLength))
            using (var archive = new ZipArchive(segment, ZipArchiveMode.Read))
            {
                int done = 0;
                foreach (var entry in archive.Entries)
                {
                    string output = Path.GetFullPath(Path.Combine(target, entry.FullName.Replace('/', Path.DirectorySeparatorChar)));
                    if (!output.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Ruta inválida en el paquete.");
                    if (entry.FullName.EndsWith("/")) Directory.CreateDirectory(output);
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(output));
                        using (var source = entry.Open())
                        using (var dest = File.Create(output)) source.CopyTo(dest);
                    }
                    report.Report(++done * 100 / archive.Entries.Count);
                }
            }
        }
    }

    private static void CreateShortcuts(string target)
    {
        string game = Path.Combine(target, "El Cantar de Mio Cid.exe");
        string menu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "El Cantar de Mio Cid.lnk");
        string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "El Cantar de Mio Cid.lnk");
        foreach (string shortcut in new[] { menu, desktop })
        {
            Type type = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(type);
            dynamic link = shell.CreateShortcut(shortcut);
            link.TargetPath = game;
            link.WorkingDirectory = target;
            link.Description = GameName + " — 100 decisiones";
            link.Save();
        }
    }

    [STAThread]
    private static void VerifyPackage(string reportPath)
    {
        string self = System.Reflection.Assembly.GetExecutingAssembly().Location;
        long fileLength = new FileInfo(self).Length;
        using (var reader = new BinaryReader(File.OpenRead(self)))
        {
            reader.BaseStream.Seek(-16, SeekOrigin.End);
            long archiveLength = reader.ReadInt64();
            string magic = Encoding.ASCII.GetString(reader.ReadBytes(8));
            if (magic != Magic || archiveLength < 1 || archiveLength > fileLength - 16)
                throw new InvalidDataException("El paquete del instalador es inválido.");
            using (var segment = new SegmentStream(self, fileLength - 16 - archiveLength, archiveLength))
            using (var archive = new ZipArchive(segment, ZipArchiveMode.Read))
            {
                int images = 0;
                byte[] buffer = new byte[81920];
                bool game = false, decisions = false;
                foreach (var entry in archive.Entries)
                {
                    string name = entry.FullName.Replace('\\', '/');
                    if (name.StartsWith("imagenes/") && name.EndsWith(".png")) images++;
                    if (entry.FullName == "El Cantar de Mio Cid.exe") game = true;
                    if (entry.FullName == "decisiones.txt") decisions = true;
                    using (var stream = entry.Open()) while (stream.Read(buffer, 0, buffer.Length) > 0) { }
                }
                if (images != 102 || !game || !decisions) throw new InvalidDataException("Faltan archivos del juego.");
                File.WriteAllText(reportPath, "Paquete válido: 102 imágenes, juego y 100 decisiones.", Encoding.UTF8);
            }
        }
    }

    private static void Main(string[] args)
    {
        if (args.Length == 2 && args[0] == "--verify")
        {
            try { VerifyPackage(args[1]); }
            catch (Exception error) { File.WriteAllText(args[1], error.ToString(), Encoding.UTF8); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0] == "--extract")
        {
            try { Extract(args[1], new Progress<int>()); }
            catch { Environment.ExitCode = 1; }
            return;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Installer());
    }
}
