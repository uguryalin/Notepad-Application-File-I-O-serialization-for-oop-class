// ============================================================
// Models/AppSettings.cs
// OOP Kavramı: Singleton Design Pattern, Encapsulation
// Serialization: JSON ile ayarları diske kaydet/yükle
// ============================================================
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotepadApp.Models;

/// <summary>
/// Uygulama ayarları sınıfı.
/// OOP – Singleton Pattern: Tek bir örnek garanti edilir.
/// Serialization: Ayarlar JSON formatında diske yazılır/okunur.
/// </summary>
public sealed class AppSettings
{
    // ── Singleton ─────────────────────────────────────────────────
    private static AppSettings? _instance;
    private static readonly object _lock = new();

    [JsonIgnore]
    public static AppSettings Instance
    {
        get
        {
            if (_instance is null)
            {
                lock (_lock)
                {
                    _instance ??= Load();
                }
            }
            return _instance;
        }
    }

    // Private constructor – dışarıdan örneklemeyi engeller
    public AppSettings() { }

    // ── Ayar özellikleri (Properties) ─────────────────────────────
    public string  FontFamily      { get; set; } = "Consolas";
    public float   FontSize        { get; set; } = 12f;
    public bool    WordWrap        { get; set; } = true;
    public bool    ShowStatusBar   { get; set; } = true;
    public bool    ShowLineNumbers { get; set; } = false;
    public string  Theme           { get; set; } = "Dark";
    public int     TabSize         { get; set; } = 4;
    public string  DefaultEncoding { get; set; } = "UTF-8";

    // Son açılan dosyalar (maks 10)
    public List<string> RecentFiles { get; set; } = new();

    // Pencere durumu
    public int WindowWidth  { get; set; } = 1200;
    public int WindowHeight { get; set; } = 800;
    public int WindowX      { get; set; } = 100;
    public int WindowY      { get; set; } = 100;

    // ── Ayarlar dosya yolu ────────────────────────────────────────
    [JsonIgnore]
    private static string SettingsPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NotepadApp",
            "settings.json");

    // ── JSON Serializasyon / Deserializasyon ──────────────────────
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented        = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Ayarları JSON dosyasından yükler.
    /// SERIALIZATION: JSON Deserialization (dosyadan nesneye).
    /// </summary>
    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                // FILE I/O: Dosyadan okuma
                string json = File.ReadAllText(SettingsPath, System.Text.Encoding.UTF8);

                // SERIALIZATION: JSON string'i nesneye dönüştür
                var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
                return settings ?? new AppSettings();
            }
        }
        catch (Exception ex)
        {
            // Hata durumunda varsayılan ayarları kullan
            Console.WriteLine($"Ayarlar yüklenemedi: {ex.Message}");
        }

        return new AppSettings();
    }

    /// <summary>
    /// Ayarları JSON formatında diske kaydeder.
    /// SERIALIZATION: JSON Serialization (nesneden dosyaya).
    /// </summary>
    public void Save()
    {
        try
        {
            // Dizin yoksa oluştur
            string? dir = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            // SERIALIZATION: Nesneyi JSON string'e dönüştür
            string json = JsonSerializer.Serialize(this, _jsonOptions);

            // FILE I/O: Dosyaya yazma
            File.WriteAllText(SettingsPath, json, System.Text.Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ayarlar kaydedilemedi: {ex.Message}");
        }
    }

    /// <summary>Son açılan dosyaları günceller.</summary>
    public void AddRecentFile(string filePath)
    {
        RecentFiles.Remove(filePath);           // Varsa kaldır
        RecentFiles.Insert(0, filePath);        // Başa ekle
        if (RecentFiles.Count > 10)
            RecentFiles.RemoveRange(10, RecentFiles.Count - 10);
        Save();
    }

    /// <summary>Tema renklerini döndürür.</summary>
    [JsonIgnore]
    public (Color Background, Color Foreground, Color MenuBackground) ThemeColors =>
        Theme == "Dark"
            ? (Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(45, 45, 48))
            : (Color.White, Color.Black, Color.FromArgb(240, 240, 240));
}
