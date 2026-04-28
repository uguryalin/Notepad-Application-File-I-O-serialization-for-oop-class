// ============================================================
// Serialization/JsonDocSerializer.cs
// OOP Kavramı: ISerializer<T> implementasyonu
// Serialization: System.Text.Json ile JSON Serializasyon
// ============================================================
using System.Text;
using System.Text.Json;

namespace NotepadApp.Serialization;

/// <summary>
/// JSON formatında serializasyon sağlar.
/// OOP – Interface Implementation: ISerializer&lt;T&gt; arayüzünü uygular.
/// SERIALIZATION: System.Text.Json kütüphanesi kullanılır.
/// </summary>
public class JsonDocSerializer<T> : ISerializer<T> where T : class
{
    // ── Özellikler ────────────────────────────────────────────────
    public string FormatName => "JSON";

    private readonly JsonSerializerOptions _options;

    // ── Constructor ───────────────────────────────────────────────
    public JsonDocSerializer(bool prettyPrint = true)
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented        = prettyPrint,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // Döngüsel referansları yönet
            ReferenceHandler     = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            // Enum'ları string olarak serialize et
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
    }

    // ── Serialize ─────────────────────────────────────────────────
    /// <summary>
    /// Nesneyi JSON formatına dönüştürüp dosyaya yazar.
    /// FILE I/O + SERIALIZATION: Birlikte kullanım örneği.
    /// </summary>
    public void Serialize(T obj, string filePath)
    {
        // Nesneyi JSON string'e dönüştür (Serialization)
        string json = JsonSerializer.Serialize(obj, _options);

        // JSON'u dosyaya yaz (File I/O)
        File.WriteAllText(filePath, json, Encoding.UTF8);
    }

    // ── Deserialize ───────────────────────────────────────────────
    /// <summary>
    /// Dosyadan JSON okuyup nesneye dönüştürür.
    /// FILE I/O + SERIALIZATION: Birlikte kullanım örneği.
    /// </summary>
    public T? Deserialize(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Dosya bulunamadı: {filePath}");

        // Dosyadan JSON oku (File I/O)
        string json = File.ReadAllText(filePath, Encoding.UTF8);

        // JSON'u nesneye dönüştür (Deserialization)
        return JsonSerializer.Deserialize<T>(json, _options);
    }

    // ── String tabanlı ────────────────────────────────────────────
    public string SerializeToString(T obj)
        => JsonSerializer.Serialize(obj, _options);

    public T? DeserializeFromString(string data)
        => JsonSerializer.Deserialize<T>(data, _options);
}
