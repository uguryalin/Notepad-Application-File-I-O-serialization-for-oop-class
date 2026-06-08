// ============================================================
// Serialization/XmlDocSerializer.cs
// OOP Kavramı: ISerializer<T> – XML implementasyonu
// Serialization: System.Xml.Serialization ile XML Serializasyon
// ============================================================
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NotepadApp.Serialization;

/// <summary>
/// XML formatında serializasyon sağlar.
/// OOP – Interface Implementation: ISerializer&lt;T&gt; arayüzünü uygular.
/// SERIALIZATION: System.Xml.Serialization kütüphanesi kullanılır.
/// </summary>
public class XmlDocSerializer<T> : ISerializer<T> where T : class
{
    // ── Özellikler ────────────────────────────────────────────────
    public string FormatName => "XML";

    private readonly XmlSerializer _xmlSerializer;

    // ── Constructor ───────────────────────────────────────────────
    public XmlDocSerializer()
    {
        // XmlSerializer, tür bilgisine göre oluşturulur (Generic)
        _xmlSerializer = new XmlSerializer(typeof(T));
    }

    // ── Serialize ─────────────────────────────────────────────────
    /// <summary>
    /// Nesneyi XML formatına dönüştürüp dosyaya yazar.
    /// SERIALIZATION: XmlSerializer.Serialize() kullanılır.
    /// </summary>
    public void Serialize(T obj, string filePath)
    {
        var xmlSettings = new XmlWriterSettings
        {
            Indent             = true,
            IndentChars        = "  ",
            Encoding           = Encoding.UTF8,
            OmitXmlDeclaration = false
        };

        // FILE I/O: XmlWriter ile dosyaya yazma
        using var writer = XmlWriter.Create(filePath, xmlSettings);
        _xmlSerializer.Serialize(writer, obj);
    }

    // ── Deserialize ───────────────────────────────────────────────
    /// <summary>
    /// XML dosyasından nesneyi okur.
    /// SERIALIZATION: XmlSerializer.Deserialize() kullanılır.
    /// </summary>
    public T? Deserialize(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"XML dosyası bulunamadı: {filePath}");

        // FILE I/O: XmlReader ile dosyadan okuma
        using var reader = XmlReader.Create(filePath);
        return _xmlSerializer.Deserialize(reader) as T;
    }

    // ── String tabanlı ────────────────────────────────────────────
    public string SerializeToString(T obj)
    {
        var sb = new StringBuilder();
        var xmlSettings = new XmlWriterSettings
        {
            Indent      = true,
            IndentChars = "  ",
            Encoding    = Encoding.UTF8
        };

        using var writer = XmlWriter.Create(sb, xmlSettings);
        _xmlSerializer.Serialize(writer, obj);
        return sb.ToString();
    }

    public T? DeserializeFromString(string data)
    {
        using var reader = new StringReader(data);
        return _xmlSerializer.Deserialize(reader) as T;
    }
}
