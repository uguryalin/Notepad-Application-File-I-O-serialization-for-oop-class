// ============================================================
// Models/BaseDocument.cs
// OOP Kavramı: Abstract sınıf, Encapsulation, Abstraction
// ============================================================
namespace NotepadApp.Models;

/// <summary>
/// Tüm belge türleri için temel soyut sınıf.
/// OOP – Abstract Class: Ortak davranışı tanımlar, somut implementasyonu
/// alt sınıflara bırakır.
/// </summary>
public abstract class BaseDocument
{
    // ── Private alanlar (Encapsulation) ──────────────────────────
    private string _title;
    private string _content;
    private bool   _isModified;
    private DateTime _createdAt;
    private DateTime _lastModifiedAt;

    // ── Constructor ───────────────────────────────────────────────
    protected BaseDocument(string title = "Yeni Belge")
    {
        _title          = title;
        _content        = string.Empty;
        _isModified     = false;
        _createdAt      = DateTime.Now;
        _lastModifiedAt = DateTime.Now;
    }

    // ── Properties (Encapsulation: getter/setter ile kontrollü erişim) ──
    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                MarkModified();
            }
        }
    }

    public string Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content        = value;
                _isModified     = true;
                _lastModifiedAt = DateTime.Now;
            }
        }
    }

    public bool IsModified
    {
        get => _isModified;
        protected set => _isModified = value;
    }

    public DateTime CreatedAt      => _createdAt;
    public DateTime LastModifiedAt => _lastModifiedAt;

    /// <summary>Dosya yolu (henüz kaydedilmediyse null)</summary>
    public string? FilePath { get; set; }

    // ── Soyut metotlar (Abstraction) ─────────────────────────────
    /// <summary>Belgeyi kaydetme işlemi – alt sınıf implemente eder.</summary>
    public abstract void Save(string path);

    /// <summary>Belgeyi yükleme işlemi – alt sınıf implemente eder.</summary>
    public abstract void Load(string path);

    /// <summary>Belgenin MIME türünü döndürür.</summary>
    public abstract string GetMimeType();

    // ── Ortak metotlar ────────────────────────────────────────────
    /// <summary>Belgeyi "değiştirilmedi" olarak işaretler.</summary>
    public void ClearModified()
    {
        _isModified = false;
    }

    protected void MarkModified()
    {
        _isModified     = true;
        _lastModifiedAt = DateTime.Now;
    }

    /// <summary>Belge bilgilerini özet olarak döndürür.</summary>
    public override string ToString() =>
        $"[{GetType().Name}] {_title} | " +
        $"Değiştirildi: {_isModified} | " +
        $"Oluşturulma: {_createdAt:yyyy-MM-dd HH:mm}";
}
