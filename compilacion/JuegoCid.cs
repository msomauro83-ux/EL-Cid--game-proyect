using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace JuegoCid
{
    internal sealed class Decision
    {
        public string Pregunta, Correcta, Alternativa1, Alternativa2;
    }

    internal sealed class Ventana : Form
    {
        private readonly List<Decision> decisiones = new List<Decision>();
        private readonly Label encabezado = new Label();
        private readonly Label pregunta = new Label();
        private readonly Label pie = new Label();
        private readonly PictureBox imagen = new PictureBox();
        private readonly Button[] botones = new Button[3];
        private int indice;
        private int posicionCorrecta;
        private string estado = "portada";
        private readonly string carpetaImagenes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imagenes");

        public Ventana()
        {
            Text = "El Cantar de Mío Cid — 100 decisiones";
            ClientSize = new Size(1100, 820);
            MinimumSize = new Size(760, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 26, 24);
            ForeColor = Color.FromArgb(244, 232, 208);

            using (Stream entrada = Assembly.GetExecutingAssembly().GetManifestResourceStream("preguntas.txt"))
            using (StreamReader lector = new StreamReader(entrada, Encoding.UTF8))
            {
                string linea;
                while ((linea = lector.ReadLine()) != null)
                {
                    string[] partes = linea.Split('|');
                    if (partes.Length != 4) throw new InvalidDataException("Pregunta incompleta: " + (decisiones.Count + 1));
                    decisiones.Add(new Decision { Pregunta = partes[0], Correcta = partes[1], Alternativa1 = partes[2], Alternativa2 = partes[3] });
                }
            }
            if (decisiones.Count != 100) throw new InvalidDataException("El juego necesita exactamente 100 preguntas.");

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(24, 12, 24, 16);
            panel.ColumnCount = 1;
            panel.RowCount = 5;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            Controls.Add(panel);

            encabezado.Dock = DockStyle.Fill;
            encabezado.Font = new Font("Georgia", 12, FontStyle.Bold);
            encabezado.ForeColor = Color.FromArgb(220, 175, 100);
            panel.Controls.Add(encabezado, 0, 0);

            imagen.Dock = DockStyle.Fill;
            imagen.SizeMode = PictureBoxSizeMode.Zoom;
            imagen.BackColor = Color.FromArgb(18, 17, 16);
            panel.Controls.Add(imagen, 0, 1);

            pregunta.Dock = DockStyle.Fill;
            pregunta.Font = new Font("Georgia", 16);
            pregunta.TextAlign = ContentAlignment.MiddleLeft;
            pregunta.Padding = new Padding(0, 5, 0, 5);
            panel.Controls.Add(pregunta, 0, 2);

            TableLayoutPanel opciones = new TableLayoutPanel();
            opciones.Dock = DockStyle.Fill;
            opciones.ColumnCount = 1;
            opciones.RowCount = 3;
            for (int i = 0; i < 3; i++) opciones.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333f));
            panel.Controls.Add(opciones, 0, 3);
            for (int i = 0; i < 3; i++)
            {
                Button boton = new Button();
                boton.Dock = DockStyle.Fill;
                boton.Margin = new Padding(0, 3, 0, 3);
                boton.FlatStyle = FlatStyle.Flat;
                boton.FlatAppearance.BorderColor = Color.FromArgb(139, 102, 58);
                boton.BackColor = Color.FromArgb(58, 45, 35);
                boton.ForeColor = Color.FromArgb(249, 238, 215);
                boton.Font = new Font("Segoe UI", 11);
                boton.TextAlign = ContentAlignment.MiddleLeft;
                boton.Padding = new Padding(16, 0, 8, 0);
                int opcion = i;
                boton.Click += (sender, e) => Elegir(opcion);
                opciones.Controls.Add(boton, 0, i);
                botones[i] = boton;
            }

            pie.Dock = DockStyle.Fill;
            pie.TextAlign = ContentAlignment.BottomRight;
            pie.Font = new Font("Segoe UI", 9);
            pie.ForeColor = Color.FromArgb(190, 174, 149);
            panel.Controls.Add(pie, 0, 4);
            Portada();
        }

        private void MostrarImagen(string nombre)
        {
            string ruta = Path.Combine(carpetaImagenes, nombre + ".png");
            if (!File.Exists(ruta)) throw new FileNotFoundException("Falta una ilustración del juego.", ruta);
            Image anterior = imagen.Image;
            using (Image fuente = Image.FromFile(ruta)) imagen.Image = new Bitmap(fuente);
            if (anterior != null) anterior.Dispose();
        }

        private void Portada()
        {
            indice = 0;
            estado = "portada";
            encabezado.Text = "UNA AVENTURA BASADA EN EL CANTAR DE MÍO CID";
            pregunta.Text = "EL CANTAR DE MÍO CID\n100 decisiones para recuperar la honra";
            MostrarImagen("portada");
            botones[0].Text = "COMENZAR LA AVENTURA";
            botones[0].Visible = true;
            botones[1].Visible = botones[2].Visible = false;
            pie.Text = "Un error termina la partida. Debes comenzar de nuevo.";
        }

        private void MostrarPregunta()
        {
            estado = "pregunta";
            string capitulo = indice < 42 ? "I · EL DESTIERRO" : indice < 68 ? "II · LAS BODAS" : "III · LA AFRENTA DE CORPES";
            encabezado.Text = capitulo + "    /    DECISIÓN " + (indice + 1) + " DE 100";
            Decision decision = decisiones[indice];
            pregunta.Text = decision.Pregunta;
            MostrarImagen((indice + 1).ToString("D3"));
            posicionCorrecta = (indice * 7 + 1) % 3;
            string[] respuestas = new string[3];
            respuestas[posicionCorrecta] = decision.Correcta;
            int alternativa = 0;
            for (int i = 0; i < 3; i++)
                if (i != posicionCorrecta) respuestas[i] = alternativa++ == 0 ? decision.Alternativa1 : decision.Alternativa2;
            for (int i = 0; i < 3; i++)
            {
                botones[i].Text = (i + 1) + ".  " + respuestas[i];
                botones[i].Visible = true;
            }
            pie.Text = "Una decisión incorrecta te devuelve al inicio de la aventura.";
        }

        private void Elegir(int opcion)
        {
            if (estado == "portada") { MostrarPregunta(); return; }
            if (estado == "derrota" || estado == "fin") { Portada(); return; }
            if (opcion != posicionCorrecta)
            {
                estado = "derrota";
                encabezado.Text = "FIN DE LA PARTIDA";
                pregunta.Text = "La historia tomó otro camino. Acertaste " + indice + " de 100 decisiones.\nEn el cantar: " + decisiones[indice].Correcta + ".";
                botones[0].Text = "VOLVER AL PRINCIPIO";
                botones[1].Visible = botones[2].Visible = false;
                pie.Text = "La siguiente partida comienza desde la primera decisión.";
                indice = 0;
                return;
            }
            indice++;
            if (indice < 100) { MostrarPregunta(); return; }
            estado = "fin";
            MostrarImagen("final");
            encabezado.Text = "EL FINAL DEL CANTAR  /  100 DE 100";
            pregunta.Text = "¡Has completado la historia!\nLa honra del Cid y de sus hijas ha sido reparada.";
            botones[0].Text = "VOLVER A JUGAR";
            botones[1].Visible = botones[2].Visible = false;
            pie.Text = "Una adaptación del poema, entre historia y leyenda.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && imagen.Image != null) imagen.Image.Dispose();
            base.Dispose(disposing);
        }

        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Ventana());
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "El Cantar de Mío Cid", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
