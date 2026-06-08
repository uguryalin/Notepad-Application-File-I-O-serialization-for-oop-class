// ============================================================
// Forms/FindReplaceForm.cs  –  Bul & Değiştir Penceresi
// OOP: Composition (RichTextBox referansı), Encapsulation
// ============================================================
namespace NotepadApp.Forms;

public class FindReplaceForm : Form
{
    // Composition – dışarıdan enjekte edilen editör referansı
    private readonly RichTextBox _editor;
    private int _lastSearchIndex = 0;

    private TextBox _txtFind    = null!;
    private TextBox _txtReplace = null!;
    private CheckBox _chkCase   = null!;
    private Button   _btnFind   = null!;
    private Button   _btnReplace= null!;
    private Button   _btnAll    = null!;
    private Label    _lblResult = null!;

    public FindReplaceForm(RichTextBox editor)
    {
        _editor = editor;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "🔍 Bul & Değiştir";
        Size            = new Size(450, 260);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        StartPosition   = FormStartPosition.CenterParent;
        TopMost         = true;

        var (bg, fg, _) = Models.AppSettings.Instance.ThemeColors;
        BackColor = bg; ForeColor = fg;

        int y = 15;
        Controls.Add(new Label { Text = "Aranan:", Left = 15, Top = y + 3, Width = 80, ForeColor = fg });
        _txtFind = new TextBox { Left = 100, Top = y, Width = 300 };
        Controls.Add(_txtFind);

        y += 40;
        Controls.Add(new Label { Text = "Yeni Metin:", Left = 15, Top = y + 3, Width = 80, ForeColor = fg });
        _txtReplace = new TextBox { Left = 100, Top = y, Width = 300 };
        Controls.Add(_txtReplace);

        y += 40;
        _chkCase = new CheckBox { Text = "Büyük/Küçük Harf Duyarlı", Left = 100, Top = y, Width = 220, ForeColor = fg };
        Controls.Add(_chkCase);

        y += 40;
        _btnFind    = new Button { Text = "▶ Sonrakini Bul", Left = 15,  Top = y, Width = 130, Height = 32 };
        _btnReplace = new Button { Text = "↔ Değiştir",      Left = 155, Top = y, Width = 110, Height = 32 };
        _btnAll     = new Button { Text = "⟳ Tümünü Değiştir", Left = 275, Top = y, Width = 130, Height = 32 };
        Controls.AddRange(new Control[] { _btnFind, _btnReplace, _btnAll });

        y += 45;
        _lblResult = new Label { Left = 15, Top = y, Width = 400, ForeColor = Color.LightGreen };
        Controls.Add(_lblResult);

        _btnFind.Click    += FindNext;
        _btnReplace.Click += Replace;
        _btnAll.Click     += ReplaceAll;
    }

    private StringComparison Comparison =>
        _chkCase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

    private void FindNext(object? s, EventArgs e)
    {
        var text  = _editor.Text;
        var query = _txtFind.Text;
        if (string.IsNullOrEmpty(query)) return;

        int idx = text.IndexOf(query, _lastSearchIndex, Comparison);
        if (idx < 0)
        {
            // Başa dön
            idx = text.IndexOf(query, 0, Comparison);
            if (idx < 0) { _lblResult.Text = "Bulunamadı."; return; }
        }

        _editor.Select(idx, query.Length);
        _editor.ScrollToCaret();
        _lastSearchIndex = idx + query.Length;
        _lblResult.Text  = $"Pozisyon: {idx}";
    }

    private void Replace(object? s, EventArgs e)
    {
        if (_editor.SelectedText.Equals(_txtFind.Text, Comparison))
        {
            _editor.SelectedText = _txtReplace.Text;
            _lastSearchIndex     = _editor.SelectionStart;
        }
        FindNext(s, e);
    }

    private void ReplaceAll(object? s, EventArgs e)
    {
        var query   = _txtFind.Text;
        var replace = _txtReplace.Text;
        if (string.IsNullOrEmpty(query)) return;

        int count = 0;
        var comparison = _chkCase.Checked
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        string text = _editor.Text;
        var sb = new System.Text.StringBuilder();
        int lastIdx = 0;
        int idx = 0;

        while (true)
        {
            idx = text.IndexOf(query, lastIdx, comparison);
            if (idx < 0)
            {
                sb.Append(text.Substring(lastIdx));
                break;
            }

            sb.Append(text.Substring(lastIdx, idx - lastIdx));
            sb.Append(replace);
            lastIdx = idx + query.Length;
            count++;
        }

        _editor.Text     = sb.ToString();
        _lblResult.Text  = $"{count} yer değiştirildi.";
        _lastSearchIndex = 0;
    }
}
