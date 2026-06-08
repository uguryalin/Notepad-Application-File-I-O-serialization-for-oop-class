// ============================================================
// Models/NoteDocument.cs
// OOP Kavramı: Inheritance (kalıtım), Polymorphism, File I/O
// ============================================================
using System.Text;

namespace NotepadApp.Models;

/// <summary>
/// Düz metin belgesi.
/// OOP – Inheritance: BaseDocument'tan türetilmiş somut sınıf.
/// OOP – Polymorphism: Save/Load metotlarını override eder.
/// </summary>
[Serializable]   // Binary serializasyon için işaretleme
public class NoteDocument : BaseDocument
{
    // ── Ek özellikler ─────────────────────────────────────────────
    public Encoding FileEncoding { get; set; } = Encoding.UTF8;
    public string   Extension    { get; set; } = ".txt";

    // ── Constructor'lar (Constructor Overloading) ──────────────────
    public NoteDocument() : base("Yeni Belge") { }

    public NoteDocument(string title) : base(title) { }

    public NoteDocument(string title, string content) : base(title)
    {
        Content = content;
    }

    // ── Override metotlar (Polymorphism) ──────────────────────────

    /// <summary>
    /// Belgeyi düz metin olarak diske yazar.
    /// FILE I/O: StreamWriter kullanarak dosyaya yazma.
    /// </summary>
    public override void Save(string path)
    {
        // StreamWriter ile dosyaya yazma (File I/O)
        using var writer = new StreamWriter(path, append: false, FileEncoding);
        writer.Write(Content);

        FilePath = path;
        Title    = Path.GetFileNameWithoutExtension(path);
        ClearModified();
    }

    /// <summary>
    /// Düz metin dosyasını okur.
    /// FILE I/O: StreamReader kullanarak dosyadan okuma.
    /// </summary>
    public override void Load(string path)
    {
        // StreamReader ile dosyadan okuma (File I/O)
        using var reader = new StreamReader(path, detectEncodingFromByteOrderMarks: true);
        Content  = reader.ReadToEnd();
        FilePath = path;
        Title    = Path.GetFileNameWithoutExtension(path);
        ClearModified();
    }

    public override string GetMimeType() => "text/plain";

    // ── Ek yardımcı metotlar ──────────────────────────────────────
    public int GetWordCount() =>
        string.IsNullOrWhiteSpace(Content)
            ? 0
            : Content.Split(new[] { ' ', '\n', '\r', '\t' },
                            StringSplitOptions.RemoveEmptyEntries).Length;

    public int GetLineCount() =>
        string.IsNullOrWhiteSpace(Content)
            ? 1
            : Content.Split('\n').Length;

    public int GetCharCount() => Content.Length;

    /// <summary>Dosya bilgilerini DTO olarak döndürür (Serializasyon için).</summary>
    public DocumentDto ToDto() => new()
    {
        Title          = Title,
        FilePath       = FilePath,
        EncodingName   = FileEncoding.WebName,
        Extension      = Extension,
        CreatedAt      = CreatedAt,
        LastModifiedAt = LastModifiedAt
    };
}

// ── DTO (Data Transfer Object) – Serializasyon ────────────────────
/// <summary>
/// NoteDocument'ın serializasyon için kullanılan hafif temsili.
/// OOP – Separation of Concerns: Veri transferi ve iş mantığını ayırır.
/// </summary>
public class DocumentDto
{
    public string    Title          { get; set; } = string.Empty;
    public string?   FilePath       { get; set; }
    public string    EncodingName   { get; set; } = "utf-8";
    public string    Extension      { get; set; } = ".txt";
    public DateTime  CreatedAt      { get; set; }
    public DateTime  LastModifiedAt { get; set; }
}
