using System;
using System.IO;
using System.Reflection;

internal static class GameSmokeTest
{
    [STAThread]
    private static int Main()
    {
        try
        {
            string gamePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "El Cantar de Mio Cid.exe");
            Type type = Assembly.LoadFrom(gamePath).GetType("CidGameModern", true);
            object game = Activator.CreateInstance(type, true);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo state = type.GetField("state", flags);
            FieldInfo index = type.GetField("index", flags);
            FieldInfo correct = type.GetField("correctIndex", flags);
            MethodInfo showCover = type.GetMethod("ShowCover", flags);
            MethodInfo showQuestion = type.GetMethod("ShowQuestion", flags);
            MethodInfo choose = type.GetMethod("Choose", flags);
            showQuestion.Invoke(game, new object[0]);
            choose.Invoke(game, new object[] { ((int)correct.GetValue(game) + 1) % 3 });
            if ((string)state.GetValue(game) != "lost") throw new Exception("No terminó al fallar.");
            showCover.Invoke(game, new object[0]);
            showQuestion.Invoke(game, new object[0]);
            for (int i = 0; i < 100; i++) choose.Invoke(game, new object[] { (int)correct.GetValue(game) });
            if ((string)state.GetValue(game) != "end" || (int)index.GetValue(game) != 100)
                throw new Exception("No terminó tras 100 aciertos.");
            Console.WriteLine("Prueba superada: derrota, reinicio y 100 decisiones correctas.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
