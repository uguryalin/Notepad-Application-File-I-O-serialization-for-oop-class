// ============================================================
// Program.cs  –  Uygulama Giriş Noktası
// OOP Kavramı: Static Main metodu, Application başlatma
// ============================================================
using NotepadApp.Forms;

namespace NotepadApp;

static class Program
{
    /// <summary>
    /// Uygulamanın ana giriş noktası.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Yüksek DPI desteği
        ApplicationConfiguration.Initialize();

        // Ana formu başlat
        Application.Run(new MainForm());
    }
}
