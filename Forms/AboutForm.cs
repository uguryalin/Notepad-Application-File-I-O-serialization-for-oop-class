// ============================================================
// Forms/AboutForm.cs  –  Hakkında Penceresi
// OOP: Inheritance (Form), Encapsulation
// ============================================================
using NotepadApp.Models;

namespace NotepadApp.Forms;

public class AboutForm : Form
{
    public AboutForm()
    {
        var (bg, fg, _) = AppSettings.Instance.ThemeColors;

        Text            = "ℹ️ Hakkında";
        Size            = new Size(480, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition   = FormStartPosition.CenterParent;
        MaximizeBox     = false;
        MinimizeBox     = false;
        BackColor       = bg;
        ForeColor       = fg;

        // Başlık
        var lblTitle = new Label
        {
            Text      = "📝 NotepadApp",
            Left      = 0, Top = 20, Width = 480,
            Font      = new Font("Segoe UI", 22f, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 180, 255),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblSubtitle = new Label
        {
            Text      = "File I/O & Serialization Demo",
            Left      = 0, Top = 60, Width = 480,
            Font      = new Font("Segoe UI", 11f, FontStyle.Italic),
            ForeColor = Color.LightGray,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // OOP Kavramları listesi
        var info = new RichTextBox
        {
            Left        = 20, Top = 100, Width = 430, Height = 220,
            ReadOnly    = true,
            BorderStyle = BorderStyle.None,
            BackColor   = bg,
            ForeColor   = fg,
            Font        = new Font("Consolas", 10f),
            Text        =
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\r\n" +
                "  OOP Kavramları:\r\n" +
                "  ✔ Encapsulation  – AppSettings, BaseDocument\r\n" +
                "  ✔ Inheritance    – NoteDocument : BaseDocument\r\n" +
                "  ✔ Polymorphism   – Save() / Load() override\r\n" +
                "  ✔ Abstraction    – ISerializer<T> interface\r\n" +
                "  ✔ Generics       – ISerializer<T>, JsonDocSerializer<T>\r\n" +
                "  ✔ Singleton      – AppSettings.Instance\r\n" +
                "  ✔ Composition    – RecentFilesManager, FileManager\r\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\r\n" +
                "  Serialization Formatları:\r\n" +
                "  📄 JSON   – System.Text.Json\r\n" +
                "  📄 XML    – System.Xml.Serialization\r\n" +
                "  📄 Binary – BinaryWriter / BinaryReader\r\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\r\n" +
                $"  .NET 8 | Windows Forms | {DateTime.Now.Year}"
        };

        var btnClose = new Button
        {
            Text   = "Kapat",
            Left   = 190, Top = 340, Width = 100, Height = 34,
            Anchor = AnchorStyles.Bottom
        };
        btnClose.Click += (_, _) => Close();
        AcceptButton    = btnClose;

        Controls.AddRange(new Control[] { lblTitle, lblSubtitle, info, btnClose });
    }
}
