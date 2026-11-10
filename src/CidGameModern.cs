using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

internal sealed class StoryDecision
{
    internal string Question, Correct, Wrong1, Wrong2;
}

internal sealed class CidGameModern : Window
{
    private static readonly Brush Bg = Paint("#0B1018");
    private static readonly Brush Panel = Paint("#151C27");
    private static readonly Brush Gold = Paint("#D8B573");
    private static readonly Brush White = Paint("#F4F1E9");
    private static readonly Brush Muted = Paint("#A9B2BE");
    private readonly StoryDecision[] decisions;
    private readonly Grid sceneLayer = new Grid { ClipToBounds = true };
    private readonly TextBlock sceneEyebrow = Label("LA LEYENDA DEL CID", 12, Gold, FontWeights.SemiBold);
    private readonly TextBlock sceneCaption = Label("Un viaje por la honra y la memoria", 24, White, FontWeights.SemiBold);
    private readonly TextBlock progressText = Label("100 DECISIONES · 3 CANTARES", 12, Muted, FontWeights.SemiBold);
    private readonly Border progressFill = new Border { Background = Gold, CornerRadius = new CornerRadius(3), HorizontalAlignment = HorizontalAlignment.Left };
    private readonly Grid progressTrack = new Grid();
    private readonly StackPanel content = new StackPanel();
    private int index;
    private int correctIndex;
    private string state;

    private static Brush Paint(string hex) { return (Brush)new BrushConverter().ConvertFromString(hex); }
    private static TextBlock Label(string text, double size, Brush color, FontWeight weight)
    {
        return new TextBlock { Text = text, FontSize = size, Foreground = color, FontWeight = weight,
            FontFamily = new FontFamily("Segoe UI"), TextWrapping = TextWrapping.Wrap };
    }

    internal CidGameModern()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "decisiones.txt");
        decisions = File.ReadAllLines(path, Encoding.UTF8).Where(x => !String.IsNullOrWhiteSpace(x))
            .Select(x => x.Split('|')).Select(parts => {
                if (parts.Length != 4) throw new InvalidDataException("Una decisión está incompleta.");
                return new StoryDecision { Question = parts[0], Correct = parts[1], Wrong1 = parts[2], Wrong2 = parts[3] };
            }).ToArray();
        if (decisions.Length != 100) throw new InvalidDataException("El juego requiere 100 decisiones.");

        Title = "El Cantar de Mío Cid — 100 decisiones";
        Width = 1380; Height = 850; MinWidth = 1050; MinHeight = 690;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = Bg;
        FontFamily = new FontFamily("Segoe UI");
        string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cid.ico");
        if (File.Exists(iconPath)) Icon = new BitmapImage(new Uri(iconPath));

        var root = new Grid { Margin = new Thickness(28, 20, 28, 18) };
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(78) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(12) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });
        Content = root;

        var header = new Grid();
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetRow(header, 0); root.Children.Add(header);
        var brand = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        header.Children.Add(brand);
        var logo = new Border { Width = 52, Height = 52, CornerRadius = new CornerRadius(13), Background = Paint("#253044"), Margin = new Thickness(0, 0, 14, 0) };
        string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cid.png");
        if (File.Exists(logoPath)) logo.Child = new Image { Source = LoadImage(logoPath), Stretch = Stretch.UniformToFill };
        else logo.Child = new TextBlock { Text = "✦", FontSize = 29, Foreground = Gold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        brand.Children.Add(logo);
        var titles = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        titles.Children.Add(Label("EL CANTAR DE MÍO CID", 19, White, FontWeights.Bold));
        var subtitle = Label("UNA AVENTURA NARRATIVA", 10, Gold, FontWeights.SemiBold);
        subtitle.Margin = new Thickness(0, 2, 0, 0);
        titles.Children.Add(subtitle); brand.Children.Add(titles);
        progressText.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(progressText, 1); header.Children.Add(progressText);

        progressTrack.Height = 4; progressTrack.VerticalAlignment = VerticalAlignment.Top;
        progressTrack.Background = Paint("#263241");
        progressTrack.Children.Add(progressFill);
        progressTrack.SizeChanged += (s, e) => UpdateProgress();
        Grid.SetRow(progressTrack, 1); root.Children.Add(progressTrack);

        var body = new Grid { Margin = new Thickness(0, 18, 0, 0) };
        body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(58, GridUnitType.Star) });
        body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(18) });
        body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42, GridUnitType.Star) });
        Grid.SetRow(body, 2); root.Children.Add(body);

        var imageFrame = new Border { CornerRadius = new CornerRadius(20), Background = Paint("#151C26"), ClipToBounds = true };
        Grid.SetColumn(imageFrame, 0); body.Children.Add(imageFrame);
        imageFrame.Child = sceneLayer;
        var shade = new System.Windows.Shapes.Rectangle { VerticalAlignment = VerticalAlignment.Bottom, Height = 255,
            Fill = new LinearGradientBrush(Color.FromArgb(0, 8, 12, 19), Color.FromArgb(236, 8, 12, 19), 90) };
        sceneLayer.Children.Add(shade);
        var imageCaption = new StackPanel { VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(28, 0, 28, 30) };
        sceneEyebrow.Margin = new Thickness(0, 0, 0, 7);
        imageCaption.Children.Add(sceneEyebrow); imageCaption.Children.Add(sceneCaption); sceneLayer.Children.Add(imageCaption);

        var card = new Border { CornerRadius = new CornerRadius(20), Background = Panel,
            BorderBrush = Paint("#2A3443"), BorderThickness = new Thickness(1) };
        Grid.SetColumn(card, 2); body.Children.Add(card);
        var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, Padding = new Thickness(36, 34, 36, 34) };
        card.Child = scroll; scroll.Content = content;

        var bottom = new Grid { Margin = new Thickness(3, 13, 3, 0) };
        bottom.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        bottom.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var hint = Label("Inspirado en el poema medieval · La historia mezcla hechos y leyenda", 11, Muted, FontWeights.Normal);
        bottom.Children.Add(hint);
        var help = Label("TECLAS 1 · 2 · 3  PARA ELEGIR", 10, Muted, FontWeights.SemiBold);
        Grid.SetColumn(help, 1); bottom.Children.Add(help);
        Grid.SetRow(bottom, 3); root.Children.Add(bottom);

        KeyDown += OnKeyDown;
        ShowCover();
    }

    private static BitmapImage LoadImage(string file)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit(); bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(file); bitmap.EndInit(); bitmap.Freeze();
        return bitmap;
    }
    private void ImageFor(string name)
    {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imagenes", name + ".png");
        sceneLayer.Background = new ImageBrush(LoadImage(file)) { Stretch = Stretch.UniformToFill };
    }
    private void UpdateProgress()
    {
        double value = state == "end" ? 1 : state == "question" ? (index + 1) / 100.0 : 0;
        progressFill.Width = Math.Max(0, progressTrack.ActualWidth * value);
    }
    private void Header(string eyebrow, string title, string description)
    {
        content.Children.Clear();
        var overline = Label(eyebrow, 11, Gold, FontWeights.Bold); overline.Margin = new Thickness(0, 0, 0, 18);
        content.Children.Add(overline);
        var heading = Label(title, 32, White, FontWeights.Bold);
        heading.FontFamily = new FontFamily("Georgia"); heading.LineHeight = 42;
        heading.Margin = new Thickness(0, 0, 0, 20); content.Children.Add(heading);
        if (!String.IsNullOrEmpty(description))
        {
            var details = Label(description, 15, Muted, FontWeights.Normal);
            details.LineHeight = 25; details.Margin = new Thickness(0, 0, 0, 24); content.Children.Add(details);
        }
    }
    private Button MakeButton(string text, bool primary, Action action)
    {
        var button = new Button { Height = primary ? 58 : 68, Margin = new Thickness(0, 0, 0, 10),
            Background = primary ? Gold : Paint("#202A38"), Foreground = primary ? Bg : White,
            BorderBrush = primary ? Gold : Paint("#39475A"), BorderThickness = new Thickness(1),
            FontSize = 15, FontWeight = FontWeights.SemiBold, HorizontalContentAlignment = HorizontalAlignment.Left,
            Padding = new Thickness(18, 10, 18, 10), Cursor = Cursors.Hand,
            Content = Label(text, 15, primary ? Bg : White, FontWeights.SemiBold) };
        var template = new ControlTemplate(typeof(Button));
        var border = new FrameworkElementFactory(typeof(Border)); border.Name = "frame";
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(12));
        border.SetBinding(Border.BackgroundProperty, new System.Windows.Data.Binding("Background") { RelativeSource = System.Windows.Data.RelativeSource.TemplatedParent });
        border.SetBinding(Border.BorderBrushProperty, new System.Windows.Data.Binding("BorderBrush") { RelativeSource = System.Windows.Data.RelativeSource.TemplatedParent });
        border.SetBinding(Border.BorderThicknessProperty, new System.Windows.Data.Binding("BorderThickness") { RelativeSource = System.Windows.Data.RelativeSource.TemplatedParent });
        var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
        presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        presenter.SetBinding(ContentPresenter.MarginProperty, new System.Windows.Data.Binding("Padding") { RelativeSource = System.Windows.Data.RelativeSource.TemplatedParent });
        border.AppendChild(presenter); template.VisualTree = border;
        var hover = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
        hover.Setters.Add(new Setter(Border.BorderBrushProperty, Gold, "frame"));
        hover.Setters.Add(new Setter(Border.BackgroundProperty, primary ? Paint("#E8C98C") : Paint("#2A394C"), "frame"));
        template.Triggers.Add(hover);
        var pressed = new Trigger { Property = Button.IsPressedProperty, Value = true };
        pressed.Setters.Add(new Setter(Border.BackgroundProperty, Paint("#A6864F"), "frame"));
        template.Triggers.Add(pressed);
        button.Template = template;
        button.Click += (s, e) => action();
        return button;
    }
    private void ShowCover()
    {
        state = "cover"; index = 0; UpdateProgress();
        ImageFor("portada"); sceneEyebrow.Text = "EL CANTAR DE MÍO CID";
        sceneCaption.Text = "Toda una vida en cien decisiones";
        progressText.Text = "100 DECISIONES · 3 CANTARES";
        Header("LA AVENTURA COMIENZA", "La honra se gana con cada decisión.",
            "Recorre el destierro, la conquista de Valencia y la justicia de Corpes. Cada elección sigue el camino del poema.");
        content.Children.Add(MakeButton("COMENZAR LA HISTORIA   →", true, ShowQuestion));
        var rule = Label("UNA SOLA OPORTUNIDAD POR PARTIDA", 10, Gold, FontWeights.Bold);
        rule.Margin = new Thickness(0, 30, 0, 8); content.Children.Add(rule);
        content.Children.Add(Label("Si eliges una opción incorrecta, volverás al comienzo. Para llegar al final debes acertar las 100 decisiones seguidas.", 14, Muted, FontWeights.Normal));
    }
    private void ShowQuestion()
    {
        state = "question"; UpdateProgress();
        StoryDecision d = decisions[index];
        string chapter = index < 42 ? "I · EL DESTIERRO" : index < 68 ? "II · LAS BODAS" : "III · LA AFRENTA DE CORPES";
        ImageFor((index + 1).ToString("D3"));
        sceneEyebrow.Text = chapter; sceneCaption.Text = "DECISIÓN " + (index + 1).ToString("D2") + " DE 100";
        progressText.Text = chapter + "    ·    " + (index + 1) + " / 100";
        Header("ELIGE TU CAMINO", d.Question, null);
        correctIndex = (index * 7 + 1) % 3;
        string[] options = new string[3]; options[correctIndex] = d.Correct;
        int wrong = 0;
        for (int i = 0; i < 3; i++) if (i != correctIndex) options[i] = wrong++ == 0 ? d.Wrong1 : d.Wrong2;
        for (int i = 0; i < 3; i++)
        {
            int chosen = i;
            content.Children.Add(MakeButton((i + 1) + "   " + options[i], false, () => Choose(chosen)));
        }
        var notice = Label("Una respuesta incorrecta termina esta partida.", 12, Muted, FontWeights.Normal);
        notice.Margin = new Thickness(0, 16, 0, 0); content.Children.Add(notice);
    }
    private void Choose(int choice)
    {
        if (choice != correctIndex)
        {
            state = "lost"; UpdateProgress();
            Header("EL CAMINO SE INTERRUMPE", "La historia tomó otro rumbo.",
                "Has acertado " + index + " de 100 decisiones. En el cantar: " + decisions[index].Correct + ".");
            content.Children.Add(MakeButton("VOLVER AL PRINCIPIO   ↺", true, ShowCover));
            progressText.Text = "PARTIDA TERMINADA";
            return;
        }
        index++;
        if (index < 100) { ShowQuestion(); return; }
        state = "end"; UpdateProgress(); ImageFor("final");
        sceneEyebrow.Text = "EL FINAL DEL CANTAR"; sceneCaption.Text = "La honra ha sido restaurada";
        progressText.Text = "100 / 100 · HISTORIA COMPLETA";
        Header("HISTORIA COMPLETADA", "El Cid ha recuperado su honra.",
            "Has seguido las cien decisiones del poema. Se ha hecho justicia por Elvira y Sol, y sus nuevas bodas unen su linaje con Navarra y Aragón.");
        content.Children.Add(MakeButton("JUGAR DE NUEVO   ↺", true, ShowCover));
    }
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (state == "question" && e.Key >= Key.D1 && e.Key <= Key.D3) Choose((int)e.Key - (int)Key.D1);
        else if (state == "question" && e.Key >= Key.NumPad1 && e.Key <= Key.NumPad3) Choose((int)e.Key - (int)Key.NumPad1);
        else if (state != "question" && e.Key == Key.Enter) { if (state == "cover") ShowQuestion(); else ShowCover(); }
    }
    [STAThread]
    private static void Main(string[] args)
    {
        var app = new Application();
        try {
            var game = new CidGameModern();
            if (args.Length == 2 && (args[0] == "--preview" || args[0] == "--preview-question"))
            {
                if (args[0] == "--preview-question") game.ShowQuestion();
                var visual = (FrameworkElement)game.Content;
                visual.Measure(new Size(1380, 850));
                visual.Arrange(new Rect(0, 0, 1380, 850));
                visual.UpdateLayout();
                var bitmap = new RenderTargetBitmap(1380, 850, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(visual);
                var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (var file = File.Create(args[1])) encoder.Save(file);
                return;
            }
            app.Run(game);
        }
        catch (Exception error) { MessageBox.Show(error.Message, "El Cantar de Mío Cid", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}
