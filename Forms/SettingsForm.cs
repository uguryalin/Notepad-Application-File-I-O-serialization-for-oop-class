// ============================================================
// Forms/SettingsForm.cs  –  Ayarlar Penceresi
// OOP: Encapsulation, AppSettings Singleton kullanımı
// Serialization: Ayarlar JSON olarak kaydedilir
// ============================================================
using NotepadApp.Models;

namespace NotepadApp.Forms;

public class SettingsForm : Form
{
    private ComboBox  _cmbFont    = null!;
    private NumericUpDown _nudSize = null!;
    private CheckBox  _chkWrap    = null!;
    private CheckBox  _chkStatus  = null!;
    private ComboBox  _cmbTheme   = null!;
    private NumericUpDown _nudTab = null!;
    private Button    _btnOk      = null!;
    private Button    _btnCancel  = null!;

    public SettingsForm()
    {
        InitializeComponent();
        LoadCurrentSettings();
    }

    private void InitializeComponent()
    {
        Text            = "⚙️ Ayarlar";
        Size            = new Size(420, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition   = FormStartPosition.CenterParent;
        MaximizeBox     = false;
        MinimizeBox     = false;

        var s = AppSettings.Instance;
        var (bg, fg, _) = s.ThemeColors;
        BackColor = bg;
        ForeColor = fg;

        int y = 20;
        AddLabel("Yazı Tipi:", 20, y);
        _cmbFont = new ComboBox { Left = 150, Top = y, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbFont.Items.AddRange(new object[] { "Consolas", "Courier New", "Segoe UI", "Arial", "Calibri", "Lucida Console" });
        Controls.Add(_cmbFont);

        y += 45;
        AddLabel("Yazı Boyutu:", 20, y);
        _nudSize = new NumericUpDown { Left = 150, Top = y, Width = 80, Minimum = 8, Maximum = 36 };
        Controls.Add(_nudSize);

        y += 45;
        _chkWrap = new CheckBox { Text = "Kelime Kaydırma (Word Wrap)", Left = 20, Top = y, Width = 300, ForeColor = fg };
        Controls.Add(_chkWrap);

        y += 35;
        _chkStatus = new CheckBox { Text = "Durum Çubuğunu Göster", Left = 20, Top = y, Width = 300, ForeColor = fg };
        Controls.Add(_chkStatus);

        y += 45;
        AddLabel("Tema:", 20, y);
        _cmbTheme = new ComboBox { Left = 150, Top = y, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbTheme.Items.AddRange(new object[] { "Dark", "Light" });
        Controls.Add(_cmbTheme);

        y += 45;
        AddLabel("Tab Genişliği:", 20, y);
        _nudTab = new NumericUpDown { Left = 150, Top = y, Width = 80, Minimum = 2, Maximum = 8 };
        Controls.Add(_nudTab);

        y += 55;
        _btnOk = new Button { Text = "✔ Tamam", Left = 200, Top = y, Width = 90, Height = 34 };
        _btnOk.Click += BtnOk_Click;
        _btnCancel = new Button { Text = "✖ İptal", Left = 300, Top = y, Width = 90, Height = 34 };
        _btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.Add(_btnOk);
        Controls.Add(_btnCancel);

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;
    }

    private void AddLabel(string text, int x, int y)
    {
        var lbl = new Label { Text = text, Left = x, Top = y + 3, Width = 120,
                              ForeColor = AppSettings.Instance.ThemeColors.Foreground };
        Controls.Add(lbl);
    }

    private void LoadCurrentSettings()
    {
        var s = AppSettings.Instance;
        _cmbFont.SelectedItem  = s.FontFamily;
        _nudSize.Value         = (decimal)s.FontSize;
        _chkWrap.Checked       = s.WordWrap;
        _chkStatus.Checked     = s.ShowStatusBar;
        _cmbTheme.SelectedItem = s.Theme;
        _nudTab.Value          = s.TabSize;
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        // Ayarları AppSettings'e yaz (Singleton güncelleniyor)
        var s = AppSettings.Instance;
        s.FontFamily   = _cmbFont.SelectedItem?.ToString() ?? "Consolas";
        s.FontSize     = (float)_nudSize.Value;
        s.WordWrap     = _chkWrap.Checked;
        s.ShowStatusBar = _chkStatus.Checked;
        s.Theme        = _cmbTheme.SelectedItem?.ToString() ?? "Dark";
        s.TabSize      = (int)_nudTab.Value;

        // SERIALIZATION: Ayarları JSON olarak diske kaydet
        s.Save();

        DialogResult = DialogResult.OK;
        Close();
    }
}
