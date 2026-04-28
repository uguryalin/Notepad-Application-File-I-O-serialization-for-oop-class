// ============================================================
// Managers/RecentFilesManager.cs
// OOP Kavramı: Encapsulation, Composition
// Serialization: Son açılan dosyaları JSON'da saklar
// ============================================================
using System.Text.Json;
using NotepadApp.Models;

namespace NotepadApp.Managers;

/// <summary>
/// Son açılan dosyaları yöneten sınıf.
/// OOP – Encapsulation: Veriyi gizleyip metotlarla yönetir.
/// OOP – Composition: AppSettings'i kullanır (miras değil, bileşim).
/// SERIALIZATION: Liste JSON olarak diske kaydedilir.
/// </summary>
public class RecentFilesManager
{
    // ── Sabitler ──────────────────────────────────────────────────
    private const int MaxRecentFiles = 10;

    // ── Durum ─────────────────────────────────────────────────────
    private readonly List<RecentFileEntry> _entries = new();
    private readonly string _storePath;

    // ── Constructor ───────────────────────────────────────────────
    public RecentFilesManager()
    {
        _storePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NotepadApp",
            "recent_files.json");

        Load();
    }

    // ── Public API ────────────────────────────────────────────────
    /// <summary>Son açılan dosyaların salt-okunur listesi.</summary>
    public IReadOnlyList<RecentFileEntry> Entries => _entries.AsReadOnly();

    /// <summary>Son açılan dosyaya ekle ve kaydet.</summary>
    public void AddFile(string filePath)
    {
        // Varsa eski kaydı kaldır (duplicate önleme)
        _entries.RemoveAll(e => e.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

        // Başa ekle
        _entries.Insert(0, new RecentFileEntry
        {
            FilePath     = filePath,
            FileName     = Path.GetFileName(filePath),
            LastOpenedAt = DateTime.Now
        });

        // Maksimum sayıyı aş
        if (_entries.Count > MaxRecentFiles)
            _entries.RemoveRange(MaxRecentFiles, _entries.Count - MaxRecentFiles);

        Save();
    }

    /// <summary>Belirli bir dosyayı listeden sil.</summary>
    public void RemoveFile(string filePath)
    {
        _entries.RemoveAll(e => e.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));
        Save();
    }

    /// <summary>Tüm son açılan dosyaları temizle.</summary>
    public void Clear()
    {
        _entries.Clear();
        Save();
    }

    // ── Serialization: JSON Kaydet/Yükle ──────────────────────────
    private static readonly JsonSerializerOptions _opts = new()
    {
        WriteIndented        = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Son açılan dosyaları JSON dosyasına kaydeder.
    /// SERIALIZATION: List&lt;RecentFileEntry&gt; → JSON.
    /// </summary>
    private void Save()
    {
        try
        {
            string? dir = Path.GetDirectoryName(_storePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            string json = JsonSerializer.Serialize(_entries, _opts);
            File.WriteAllText(_storePath, json, System.Text.Encoding.UTF8);
        }
        catch { /* Sessiz hata – kritik değil */ }
    }

    /// <summary>
    /// JSON dosyasından son açılan dosyaları yükler.
    /// SERIALIZATION: JSON → List&lt;RecentFileEntry&gt;.
    /// </summary>
    private void Load()
    {
        try
        {
            if (!File.Exists(_storePath)) return;

            string json = File.ReadAllText(_storePath, System.Text.Encoding.UTF8);
            var loaded  = JsonSerializer.Deserialize<List<RecentFileEntry>>(json, _opts);

            if (loaded is not null)
            {
                // Sadece hâlâ var olan dosyaları ekle
                _entries.AddRange(loaded.Where(e => File.Exists(e.FilePath)));
            }
        }
        catch { /* Sessiz hata – varsayılan boş liste */ }
    }
}

// ── RecentFileEntry Model ─────────────────────────────────────────
/// <summary>Son açılan dosya kaydı – serializasyon için tasarlandı.</summary>
public class RecentFileEntry
{
    public string   FilePath     { get; set; } = string.Empty;
    public string   FileName     { get; set; } = string.Empty;
    public DateTime LastOpenedAt { get; set; }

    public override string ToString() =>
        $"{FileName}  ({LastOpenedAt:dd.MM.yyyy HH:mm})";
}
