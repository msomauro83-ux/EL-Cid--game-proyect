using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

internal sealed class Decision
{
    internal string Question, Correct, Wrong1, Wrong2;
}

internal sealed class CidGame : Form
{
    private readonly Decision[] decisions;
    private readonly Label heading = new Label();
    private readonly Label question = new Label();
    private readonly Label footer = new Label();
    private readonly PictureBox picture = new PictureBox();
    private readonly Button[] buttons = new Button[3];
    private int index;
    private int correctPosition;
    private string state;

    internal CidGame()
    {
        string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "decisiones.txt");
        if (!File.Exists(dataPath)) throw new FileNotFoundException("Falta decisiones.txt", dataPath);
        decisions = File.ReadAllLines(dataPath, Encoding.UTF8)
            .Where(line => !String.IsNullOrWhiteSpace(line))
            .Select(line => line.Split('|'))
            .Select(parts => {
                if (parts.Length != 4) throw new InvalidDataException("Una decisión no tiene cuatro campos.");
                return new Decision { Question = parts[0], Correct = parts[1], Wrong1 = parts[2], Wrong2 = parts[3] };
            }).ToArray();
        if (decisions.Length != 100) throw new InvalidDataException("El juego necesita exactamente 100 decisiones.");

        Text = "El Cantar de Mío Cid — 100 decisiones";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1100, 820);
        MinimumSize = new Size(760, 650);
        BackColor = Color.FromArgb(30, 26, 24);
        ForeColor = Color.FromArgb(244, 232, 208);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24, 12, 24, 16), ColumnCount = 1, RowCount = 5 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        Controls.Add(layout);

        heading.Dock = DockStyle.Fill;
        heading.Font = new Font("Georgia", 12, FontStyle.Bold);
        heading.ForeColor = Color.FromArgb(220, 175, 100);
        layout.Controls.Add(heading, 0, 0);

        picture.Dock = DockStyle.Fill;
        picture.SizeMode = PictureBoxSizeMode.Zoom;
        picture.BackColor = Color.FromArgb(18, 17, 16);
        layout.Controls.Add(picture, 0, 1);

        question.Dock = DockStyle.Fill;
        question.Font = new Font("Georgia", 16);
        question.TextAlign = ContentAlignment.MiddleLeft;
        question.Padding = new Padding(0, 5, 0, 5);
        layout.Controls.Add(question, 0, 2);

        var choices = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        for (int i = 0; i < 3; i++) choices.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 3f));
        layout.Controls.Add(choices, 0, 3);
        for (int i = 0; i < 3; i++)
        {
            int choice = i;
            var button = new Button {
                Dock = DockStyle.Fill, Margin = new Padding(0, 3, 0, 3), FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(58, 45, 35), ForeColor = Color.FromArgb(249, 238, 215),
                Font = new Font("Segoe UI", 11), TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 8, 0)
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(139, 102, 58);
            button.Click += (sender, args) => Choose(choice);
            buttons[i] = button;
            choices.Controls.Add(button, 0, i);
        }

        footer.Dock = DockStyle.Fill;
        footer.TextAlign = ContentAlignment.BottomRight;
        footer.Font = new Font("Segoe UI", 9);
        footer.ForeColor = Color.FromArgb(190, 174, 149);
        layout.Controls.Add(footer, 0, 4);
        ShowCover();
    }

    private void SetImage(string name)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imagenes", name + ".png");
        if (!File.Exists(path)) throw new FileNotFoundException("Falta una imagen del juego", path);
        Image old = picture.Image;
        using (var stream = File.OpenRead(path))
        using (var source = Image.FromStream(stream)) picture.Image = new Bitmap(source);
        if (old != null) old.Dispose();
    }

    private void ShowCover()
    {
        state = "cover";
        index = 0;
        heading.Text = "UNA AVENTURA BASADA EN EL CANTAR DE MÍO CID";
        question.Text = "EL CANTAR DE MÍO CID\n100 decisiones para recuperar la honra";
        SetImage("portada");
        buttons[0].Text = "COMENZAR LA AVENTURA";
        buttons[0].Visible = true;
        buttons[1].Visible = buttons[2].Visible = false;
        footer.Text = "Un error termina la partida. Debes comenzar de nuevo.";
    }

    private void ShowQuestion()
    {
        state = "question";
        string chapter = index < 42 ? "I · EL DESTIERRO" : index < 68 ? "II · LAS BODAS" : "III · LA AFRENTA DE CORPES";
        heading.Text = chapter + "    /    DECISIÓN " + (index + 1) + " DE 100";
        Decision d = decisions[index];
        question.Text = d.Question;
        SetImage((index + 1).ToString("D3"));
        correctPosition = (index * 7 + 1) % 3;
        string[] options = new string[3];
        options[correctPosition] = d.Correct;
        int wrong = 0;
        for (int i = 0; i < 3; i++) if (i != correctPosition) options[i] = wrong++ == 0 ? d.Wrong1 : d.Wrong2;
        for (int i = 0; i < 3; i++) { buttons[i].Text = (i + 1) + ".  " + options[i]; buttons[i].Visible = true; }
        footer.Text = "Una decisión incorrecta te devuelve al inicio de la aventura.";
    }

    private void Choose(int choice)
    {
        if (state == "cover") { ShowQuestion(); return; }
        if (state == "end" || state == "lost") { ShowCover(); return; }
        if (choice != correctPosition)
        {
            string answer = decisions[index].Correct;
            state = "lost";
            heading.Text = "FIN DE LA PARTIDA";
            question.Text = "La historia tomó otro camino. Acertaste " + index + " de 100 decisiones.\nEn el cantar: " + answer + ".";
            buttons[0].Text = "VOLVER AL PRINCIPIO";
            buttons[1].Visible = buttons[2].Visible = false;
            footer.Text = "La siguiente partida comienza desde la primera decisión.";
            index = 0;
            return;
        }
        index++;
        if (index < 100) { ShowQuestion(); return; }
        state = "end";
        SetImage("final");
        heading.Text = "EL FINAL DEL CANTAR  /  100 DE 100";
        question.Text = "¡Has completado la historia!\nLa honra del Cid y de sus hijas ha sido reparada.";
        buttons[0].Text = "VOLVER A JUGAR";
        buttons[1].Visible = buttons[2].Visible = false;
        footer.Text = "Una adaptación del poema, entre historia y leyenda.";
    }

    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try { Application.Run(new CidGame()); }
        catch (Exception e) { MessageBox.Show(e.Message, "El Cantar de Mío Cid", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
