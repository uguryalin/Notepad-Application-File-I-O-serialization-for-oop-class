// ============================================================
// Forms/MainForm.cs  –  Ana Uygulama Penceresi
// OOP: Inheritance (Form), Composition, Event-Driven Programming
// FILE I/O + SERIALIZATION gösterimleri menüden erişilebilir
// ============================================================
using NotepadApp.Managers;
using NotepadApp.Models;
using NotepadApp.Serialization;

namespace NotepadApp.Forms;

public class MainForm : Form
{
    // ── Bileşenler (Composition) ──────────────────────────────────
    private readonly RecentFilesManager _recentMgr = new();
    private TabControl     _tabControl  = null!;
    private StatusStrip    _statusBar   = null!;
    private ToolStripStatusLabel _lblPos = null!;
    private ToolStripStatusLabel _lblStats = null!;
    private ToolStripStatusLabel _lblFile  = null!;
    private MenuStrip      _menuStrip   = null!;

    // ── Aktif belge ───────────────────────────────────────────────
    private NoteDocument? ActiveDocument =>
        _tabControl.SelectedTab?.Tag as NoteDocument;

    private RichTextBox? ActiveEditor =>
        _tabControl.SelectedTab?.Controls.OfType<RichTextBox>().FirstOrDefault();

    public MainForm()
    {
        InitializeComponent();
        ApplyTheme();
        NewDocument();   // Başlangıçta boş belge aç
    }

    // ── UI Kurulumu ───────────────────────────────────────────────
    private void InitializeComponent()
    {
        var s = AppSettings.Instance;
        SuspendLayout();

        Text            = "📝 NotepadApp – OOP Demo";
        Size            = new Size(s.WindowWidth, s.WindowHeight);
        Location        = new Point(s.WindowX, s.WindowY);
        MinimumSize     = new Size(800, 500);
        StartPosition   = FormStartPosition.Manual;
        Font            = new Font("Segoe UI", 9f);

        BuildMenu();
        BuildTabControl();
        BuildStatusBar();

        ResumeLayout(false);
        PerformLayout();
    }

    // ═══════════════════════════════════════════════════════════════
    //  MENÜ
    // ═══════════════════════════════════════════════════════════════
    private void BuildMenu()
    {
        _menuStrip = new MenuStrip { Dock = DockStyle.Top };

        // ── Dosya ────────────────────────────────────────────────
        var fileMenu = new ToolStripMenuItem("📁 Dosya");
        fileMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            MakeItem("🆕 Yeni",              "Ctrl+N", (_, _) => NewDocument()),
            MakeItem("📂 Aç…",               "Ctrl+O", (_, _) => OpenFile()),
            new ToolStripSeparator(),
            MakeItem("💾 Kaydet",            "Ctrl+S", (_, _) => SaveFile()),
            MakeItem("💾 Farklı Kaydet…",    "Ctrl+Shift+S", (_, _) => SaveFileAs()),
            new ToolStripSeparator(),
            BuildRecentMenu(),
            new ToolStripSeparator(),
            MakeItem("❌ Çıkış",             "Alt+F4", (_, _) => Close())
        });

        // ── Düzen ────────────────────────────────────────────────
        var editMenu = new ToolStripMenuItem("✏️ Düzen");
        editMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            MakeItem("↩ Geri Al",    "Ctrl+Z", (_, _) => ActiveEditor?.Undo()),
            MakeItem("↪ Yinele",     "Ctrl+Y", (_, _) => ActiveEditor?.Redo()),
            new ToolStripSeparator(),
            MakeItem("✂ Kes",        "Ctrl+X", (_, _) => ActiveEditor?.Cut()),
            MakeItem("📋 Kopyala",   "Ctrl+C", (_, _) => ActiveEditor?.Copy()),
            MakeItem("📌 Yapıştır",  "Ctrl+V", (_, _) => ActiveEditor?.Paste()),
            new ToolStripSeparator(),
            MakeItem("🔍 Bul & Değiştir…", "Ctrl+H", (_, _) => ShowFindReplace()),
            MakeItem("Tümünü Seç",   "Ctrl+A", (_, _) => ActiveEditor?.SelectAll())
        });

        // ── Serializasyon Demo ────────────────────────────────────
        var serialMenu = new ToolStripMenuItem("🔄 Serializasyon");
        serialMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            MakeItem("📤 JSON olarak Dışa Aktar…",   null, (_, _) => ExportJson()),
            MakeItem("📥 JSON'dan İçe Aktar…",       null, (_, _) => ImportJson()),
            new ToolStripSeparator(),
            MakeItem("📤 XML olarak Dışa Aktar…",    null, (_, _) => ExportXml()),
            MakeItem("📥 XML'den İçe Aktar…",        null, (_, _) => ImportXml()),
            new ToolStripSeparator(),
            MakeItem("📤 Binary olarak Dışa Aktar…", null, (_, _) => ExportBinary()),
            MakeItem("📥 Binary'den İçe Aktar…",     null, (_, _) => ImportBinary())
        });

        // ── Görünüm ───────────────────────────────────────────────
        var viewMenu = new ToolStripMenuItem("🎨 Görünüm");
        viewMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            MakeItem("⚙️ Ayarlar…", null, (_, _) => ShowSettings()),
            MakeItem("🌙 Tema Değiştir", null, (_, _) => ToggleTheme())
        });

        // ── Yardım ───────────────────────────────────────────────
        var helpMenu = new ToolStripMenuItem("❓ Yardım");
        helpMenu.DropDownItems.Add(MakeItem("ℹ️ Hakkında", null, (_, _) => ShowAbout()));

        _menuStrip.Items.AddRange(new ToolStripItem[]
            { fileMenu, editMenu, serialMenu, viewMenu, helpMenu });

        MainMenuStrip = _menuStrip;
        Controls.Add(_menuStrip);
    }

    private static ToolStripMenuItem MakeItem(
        string text, string? shortcut, EventHandler handler)
    {
        var item = new ToolStripMenuItem(text) { ShortcutKeyDisplayString = shortcut };
        item.Click += handler;
        return item;
    }

    private ToolStripMenuItem BuildRecentMenu()
    {
        var recent = new ToolStripMenuItem("🕓 Son Açılanlar");
        RefreshRecentMenu(recent);
        return recent;
    }

    private void RefreshRecentMenu(ToolStripMenuItem menu)
    {
        menu.DropDownItems.Clear();
        var entries = _recentMgr.Entries;

        if (entries.Count == 0)
        {
            menu.DropDownItems.Add(new ToolStripMenuItem("(Boş)") { Enabled = false });
            return;
        }

        foreach (var entry in entries)
        {
            var e = entry; // closure
            var item = new ToolStripMenuItem(e.ToString());
            item.Click += (_, _) => OpenFile(e.FilePath);
            menu.DropDownItems.Add(item);
        }

        menu.DropDownItems.Add(new ToolStripSeparator());
        var clear = new ToolStripMenuItem("🗑 Listeyi Temizle");
        clear.Click += (_, _) => { _recentMgr.Clear(); RefreshRecentMenu(menu); };
        menu.DropDownItems.Add(clear);
    }

    // ═══════════════════════════════════════════════════════════════
    //  TAB CONTROL
    // ═══════════════════════════════════════════════════════════════
    private void BuildTabControl()
    {
        _tabControl = new TabControl
        {
            Dock     = DockStyle.Fill,
            DrawMode = TabDrawMode.OwnerDrawFixed,
            Padding  = new Point(10, 5)
        };
        _tabControl.SelectedIndexChanged += (_, _) => UpdateStatusBar();
        _tabControl.DrawItem += DrawTabItem;

        Controls.Add(_tabControl);
        _tabControl.BringToFront();
    }

    private void DrawTabItem(object? sender, DrawItemEventArgs e)
    {
        var tab = _tabControl.TabPages[e.Index];
        var text = tab.Text;
        var g = e.Graphics;
        var s = AppSettings.Instance;

        var bgColor = e.Index == _tabControl.SelectedIndex
            ? s.ThemeColors.Background
            : Color.FromArgb(50, 50, 55);
        var fgColor = s.ThemeColors.Foreground;

        using var brush = new SolidBrush(bgColor);
        g.FillRectangle(brush, e.Bounds);

        using var fgBrush = new SolidBrush(fgColor);
        g.DrawString(text, Font, fgBrush, e.Bounds,
            new StringFormat { Alignment = StringAlignment.Center,
                               LineAlignment = StringAlignment.Center });
    }

    // ═══════════════════════════════════════════════════════════════
    //  STATUS BAR
    // ═══════════════════════════════════════════════════════════════
    private void BuildStatusBar()
    {
        _statusBar = new StatusStrip { Dock = DockStyle.Bottom };

        _lblPos   = new ToolStripStatusLabel("Satır 1, Sütun 1") { Spring = false };
        _lblStats = new ToolStripStatusLabel("") { Spring = true, TextAlign = ContentAlignment.MiddleCenter };
        _lblFile  = new ToolStripStatusLabel("") { Spring = false, TextAlign = ContentAlignment.MiddleRight };

        _statusBar.Items.AddRange(new ToolStripItem[] { _lblPos, _lblStats, _lblFile });
        Controls.Add(_statusBar);
    }

    private void UpdateStatusBar()
    {
        var ed  = ActiveEditor;
        var doc = ActiveDocument;
        if (ed is null || doc is null) return;

        int line = ed.GetLineFromCharIndex(ed.SelectionStart) + 1;
        int col  = ed.SelectionStart - ed.GetFirstCharIndexOfCurrentLine() + 1;

        _lblPos.Text   = $"Satır {line}, Sütun {col}";
        _lblStats.Text = $"Karakter: {doc.GetCharCount()} | Kelime: {doc.GetWordCount()} | Satır: {doc.GetLineCount()}";
        _lblFile.Text  = string.IsNullOrEmpty(doc.FilePath)
            ? "Kaydedilmedi"
            : FileManager.GetFileSizeString(doc.FilePath);
    }

    // ═══════════════════════════════════════════════════════════════
    //  BELGE OLUŞTURMA / SEKME YÖNETİMİ
    // ═══════════════════════════════════════════════════════════════
    private void NewDocument(NoteDocument? doc = null)
    {
        doc ??= new NoteDocument();

        var tab = new TabPage(doc.IsModified ? $"• {doc.Title}" : doc.Title)
        {
            Tag         = doc,
            BackColor   = AppSettings.Instance.ThemeColors.Background,
            ForeColor   = AppSettings.Instance.ThemeColors.Foreground
        };

        var editor = BuildEditor(doc);
        tab.Controls.Add(editor);

        _tabControl.TabPages.Add(tab);
        _tabControl.SelectedTab = tab;
        editor.Focus();
    }

    private RichTextBox BuildEditor(NoteDocument doc)
    {
        var s = AppSettings.Instance;
        var rtb = new RichTextBox
        {
            Dock        = DockStyle.Fill,
            Text        = doc.Content,
            Font        = new Font(s.FontFamily, s.FontSize),
            WordWrap    = s.WordWrap,
            BackColor   = s.ThemeColors.Background,
            ForeColor   = s.ThemeColors.Foreground,
            BorderStyle = BorderStyle.None,
            AcceptsTab  = true,
            ScrollBars  = RichTextBoxScrollBars.Both
        };

        rtb.TextChanged += (_, _) =>
        {
            doc.Content = rtb.Text;
            var tab = _tabControl.SelectedTab;
            if (tab?.Tag == doc)
                tab.Text = doc.IsModified ? $"• {doc.Title}" : doc.Title;
            UpdateStatusBar();
        };

        rtb.SelectionChanged += (_, _) => UpdateStatusBar();

        return rtb;
    }

    // ═══════════════════════════════════════════════════════════════
    //  DOSYA İŞLEMLERİ (File I/O)
    // ═══════════════════════════════════════════════════════════════
    private void NewDocument_Click() => NewDocument();

    private void OpenFile(string? path = null)
    {
        if (path is null)
        {
            using var dlg = new OpenFileDialog { Filter = FileManager.TextFilter };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            path = dlg.FileName;
        }

        try
        {
            var doc = new NoteDocument();
            doc.Load(path);                   // FILE I/O: dosyayı oku
            _recentMgr.AddFile(path);         // Son açılanlar güncelle
            NewDocument(doc);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Dosya açılamadı:\n{ex.Message}", "Hata",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveFile()
    {
        var doc = ActiveDocument;
        if (doc is null) return;

        if (string.IsNullOrEmpty(doc.FilePath))
            SaveFileAs();
        else
        {
            doc.Save(doc.FilePath);          // FILE I/O: dosyaya yaz
            _recentMgr.AddFile(doc.FilePath);
            RefreshTabTitle();
        }
    }

    private void SaveFileAs()
    {
        var doc = ActiveDocument;
        if (doc is null) return;

        using var dlg = new SaveFileDialog
        {
            Filter   = FileManager.TextFilter,
            FileName = doc.Title
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        doc.Save(dlg.FileName);               // FILE I/O: yeni yola kaydet
        _recentMgr.AddFile(dlg.FileName);
        RefreshTabTitle();
    }

    private void RefreshTabTitle()
    {
        var doc = ActiveDocument;
        if (doc is null) return;
        if (_tabControl.SelectedTab is { } tab)
            tab.Text = doc.Title;
        UpdateStatusBar();
    }

    // ═══════════════════════════════════════════════════════════════
    //  SERİALİZASYON DEMO
    // ═══════════════════════════════════════════════════════════════
    private void ExportJson()
    {
        var doc = ActiveDocument; if (doc is null) return;
        using var dlg = new SaveFileDialog { Filter = FileManager.JsonFilter, FileName = doc.Title + "_meta" };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var ser = new JsonDocSerializer<DocumentDto>();
        ser.Serialize(doc.ToDto(), dlg.FileName);   // SERIALIZATION: JSON
        ShowInfo($"JSON olarak dışa aktarıldı:\n{dlg.FileName}");
    }

    private void ImportJson()
    {
        using var dlg = new OpenFileDialog { Filter = FileManager.JsonFilter };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var ser = new JsonDocSerializer<DocumentDto>();
        var dto = ser.Deserialize(dlg.FileName);    // SERIALIZATION: JSON
        if (dto is null) { ShowInfo("JSON okunamadı."); return; }
        ShowInfo($"JSON içe aktarıldı:\nBaşlık: {dto.Title}\nDosya: {dto.FilePath ?? "—"}\nOluşturulma: {dto.CreatedAt:g}");
    }

    private void ExportXml()
    {
        var doc = ActiveDocument; if (doc is null) return;
        using var dlg = new SaveFileDialog { Filter = FileManager.XmlFilter, FileName = doc.Title + "_meta" };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var ser = new XmlDocSerializer<DocumentDto>();
        ser.Serialize(doc.ToDto(), dlg.FileName);   // SERIALIZATION: XML
        ShowInfo($"XML olarak dışa aktarıldı:\n{dlg.FileName}");
    }

    private void ImportXml()
    {
        using var dlg = new OpenFileDialog { Filter = FileManager.XmlFilter };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var ser = new XmlDocSerializer<DocumentDto>();
        var dto = ser.Deserialize(dlg.FileName);    // SERIALIZATION: XML
        if (dto is null) { ShowInfo("XML okunamadı."); return; }
        ShowInfo($"XML içe aktarıldı:\nBaşlık: {dto.Title}\nDosya: {dto.FilePath ?? "—"}");
    }

    private void ExportBinary()
    {
        var doc = ActiveDocument; if (doc is null) return;
        using var dlg = new SaveFileDialog { Filter = FileManager.BinaryFilter, FileName = doc.Title + "_meta" };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var ser = new BinaryDocSerializer<DocumentDto>();
        ser.Serialize(doc.ToDto(), dlg.FileName);   // SERIALIZATION: Binary
        ShowInfo($"Binary olarak dışa aktarıldı:\n{dlg.FileName}");
    }

    private void ImportBinary()
    {
        using var dlg = new OpenFileDialog { Filter = FileManager.BinaryFilter };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var ser = new BinaryDocSerializer<DocumentDto>();
        var dto = ser.Deserialize(dlg.FileName);    // SERIALIZATION: Binary
        if (dto is null) { ShowInfo("Binary dosya okunamadı."); return; }
        ShowInfo($"Binary içe aktarıldı:\nBaşlık: {dto.Title}\nEncoding: {dto.EncodingName}");
    }

    // ═══════════════════════════════════════════════════════════════
    //  DİĞER PENCERELER
    // ═══════════════════════════════════════════════════════════════
    private void ShowFindReplace()
    {
        if (ActiveEditor is null) return;
        var frm = new FindReplaceForm(ActiveEditor);
        frm.Show(this);
    }

    private void ShowSettings()
    {
        using var frm = new SettingsForm();
        if (frm.ShowDialog() == DialogResult.OK)
        {
            ApplyTheme();
            foreach (TabPage tab in _tabControl.TabPages)
            {
                var ed = tab.Controls.OfType<RichTextBox>().FirstOrDefault();
                if (ed is null) continue;
                var s = AppSettings.Instance;
                ed.Font      = new Font(s.FontFamily, s.FontSize);
                ed.WordWrap  = s.WordWrap;
                ed.BackColor = s.ThemeColors.Background;
                ed.ForeColor = s.ThemeColors.Foreground;
            }
        }
    }

    private void ShowAbout()
    {
        using var frm = new AboutForm();
        frm.ShowDialog(this);
    }

    private void ToggleTheme()
    {
        var s = AppSettings.Instance;
        s.Theme = s.Theme == "Dark" ? "Light" : "Dark";
        s.Save();
        ApplyTheme();
        foreach (TabPage tab in _tabControl.TabPages)
        {
            tab.BackColor = s.ThemeColors.Background;
            var ed = tab.Controls.OfType<RichTextBox>().FirstOrDefault();
            if (ed is null) continue;
            ed.BackColor = s.ThemeColors.Background;
            ed.ForeColor = s.ThemeColors.Foreground;
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  TEMA
    // ═══════════════════════════════════════════════════════════════
    private void ApplyTheme()
    {
        var (bg, fg, menuBg) = AppSettings.Instance.ThemeColors;
        BackColor              = bg;
        ForeColor              = fg;
        _menuStrip.BackColor   = menuBg;
        _menuStrip.ForeColor   = fg;
        _statusBar.BackColor   = menuBg;
        _statusBar.ForeColor   = fg;
        _tabControl.BackColor  = bg;
    }

    // ═══════════════════════════════════════════════════════════════
    //  FORM OLAYLARI
    // ═══════════════════════════════════════════════════════════════
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Kaydedilmemiş belgeler varsa sor
        foreach (TabPage tab in _tabControl.TabPages)
        {
            if (tab.Tag is NoteDocument doc && doc.IsModified)
            {
                var result = MessageBox.Show(
                    $"'{doc.Title}' kaydedilmedi. Çıkmadan önce kaydetmek ister misiniz?",
                    "Kaydedilmemiş Değişiklikler",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel) { e.Cancel = true; return; }
                if (result == DialogResult.Yes)
                {
                    _tabControl.SelectedTab = tab;
                    SaveFile();
                }
            }
        }

        // Pencere konumunu kaydet
        var s = AppSettings.Instance;
        s.WindowWidth  = Width;
        s.WindowHeight = Height;
        s.WindowX      = Location.X;
        s.WindowY      = Location.Y;
        s.Save();

        base.OnFormClosing(e);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Control | Keys.N: NewDocument(); return true;
            case Keys.Control | Keys.O: OpenFile();    return true;
            case Keys.Control | Keys.S: SaveFile();    return true;
            case Keys.Control | Keys.Shift | Keys.S: SaveFileAs(); return true;
            case Keys.Control | Keys.H: ShowFindReplace(); return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private static void ShowInfo(string msg) =>
        MessageBox.Show(msg, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
