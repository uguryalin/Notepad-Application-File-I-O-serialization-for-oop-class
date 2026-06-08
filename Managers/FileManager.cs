// ============================================================
// Managers/FileManager.cs
// OOP Kavramı: Generic sınıf, Static Factory Method
// FILE I/O: Tüm dosya işlemlerini merkezi yönetim
// ============================================================
using NotepadApp.Models;

namespace NotepadApp.Managers;

/// <summary>
/// Dosya okuma/yazma işlemlerini yöneten yardımcı sınıf.
/// OOP – Generics: Farklı belge tipleriyle çalışabilir.
/// OOP – Static Members: Yardımcı metotlar static tanımlanmıştır.
/// FILE I/O: File, Directory, Path sınıfları kullanılır.
/// </summary>
public static class FileManager
{
    // ── Desteklenen dosya filtreleri ──────────────────────────────
    public const string TextFilter =
        "Metin Dosyaları (*.txt)|*.txt|" +
        "C# Dosyaları (*.cs)|*.cs|" +
        "HTML Dosyaları (*.html)|*.html|" +
        "Tüm Dosyalar (*.*)|*.*";

    public const string JsonFilter  = "JSON Dosyaları (*.json)|*.json|Tüm Dosyalar (*.*)|*.*";
    public const string XmlFilter   = "XML Dosyaları (*.xml)|*.xml|Tüm Dosyalar (*.*)|*.*";
    public const string BinaryFilter = "Binary Dosyaları (*.bin)|*.bin|Tüm Dosyalar (*.*)|*.*";

    // ── Temel File I/O Metotları ──────────────────────────────────

    /// <summary>
    /// Dosyayı metin olarak okur.
    /// FILE I/O: File.ReadAllText() – tüm içeriği tek seferde okur.
    /// </summary>
    public static string ReadAllText(string filePath)
    {
        ValidatePath(filePath);
        return File.ReadAllText(filePath, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// Dosyayı satır satır okur.
    /// FILE I/O: File.ReadAllLines() – satır dizisi döndürür.
    /// </summary>
    public static string[] ReadAllLines(string filePath)
    {
        ValidatePath(filePath);
        return File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// Büyük dosyaları satır satır okur (verimli).
    /// FILE I/O: StreamReader ile satır satır okuma.
    /// </summary>
    public static IEnumerable<string> ReadLinesLazy(string filePath)
    {
        ValidatePath(filePath);

        // using + yield ile verimli akış (streaming)
        using var reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
        string? line;
        while ((line = reader.ReadLine()) is not null)
            yield return line;
    }

    /// <summary>
    /// Metni dosyaya yazar (üzerine yazar).
    /// FILE I/O: File.WriteAllText() kullanılır.
    /// </summary>
    public static void WriteAllText(string filePath, string content)
    {
        EnsureDirectory(filePath);
        File.WriteAllText(filePath, content, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// Metni dosyaya ekler (append).
    /// FILE I/O: File.AppendAllText() – dosyanın sonuna ekler.
    /// </summary>
    public static void AppendText(string filePath, string content)
    {
        EnsureDirectory(filePath);
        File.AppendAllText(filePath, content, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// Dosyayı bayt dizisi olarak okur.
    /// FILE I/O: File.ReadAllBytes() – binary okuma.
    /// </summary>
    public static byte[] ReadAllBytes(string filePath)
    {
        ValidatePath(filePath);
        return File.ReadAllBytes(filePath);
    }

    /// <summary>
    /// Bayt dizisini dosyaya yazar.
    /// FILE I/O: File.WriteAllBytes() – binary yazma.
    /// </summary>
    public static void WriteAllBytes(string filePath, byte[] data)
    {
        EnsureDirectory(filePath);
        File.WriteAllBytes(filePath, data);
    }

    // ── Dosya Bilgi Metotları ─────────────────────────────────────

    /// <summary>Dosya boyutunu okunabilir formatta döndürür.</summary>
    public static string GetFileSizeString(string filePath)
    {
        if (!File.Exists(filePath)) return "—";

        long bytes = new FileInfo(filePath).Length;
        return bytes switch
        {
            < 1024         => $"{bytes} B",
            < 1024 * 1024  => $"{bytes / 1024.0:F1} KB",
            _              => $"{bytes / (1024.0 * 1024):F1} MB"
        };
    }

    /// <summary>Dosyanın son değiştirilme tarihini döndürür.</summary>
    public static DateTime GetLastModified(string filePath) =>
        File.Exists(filePath) ? File.GetLastWriteTime(filePath) : DateTime.MinValue;

    // ── Dizin İşlemleri ───────────────────────────────────────────

    /// <summary>Dizini ve alt dizinlerini listeler.</summary>
    public static IEnumerable<string> GetFiles(string directoryPath, string pattern = "*.*") =>
        Directory.GetFiles(directoryPath, pattern, SearchOption.AllDirectories);

    /// <summary>Geçici dosya yolu oluşturur.</summary>
    public static string GetTempFilePath(string extension = ".tmp") =>
        Path.Combine(Path.GetTempPath(), $"NotepadApp_{Guid.NewGuid():N}{extension}");

    // ── Yardımcı Metotlar ─────────────────────────────────────────
    private static void ValidatePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Dosya yolu boş olamaz.", nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Dosya bulunamadı: {filePath}");
    }

    private static void EnsureDirectory(string filePath)
    {
        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }
}
