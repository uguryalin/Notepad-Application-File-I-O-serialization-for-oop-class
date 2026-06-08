// ============================================================
// Serialization/BinaryDocSerializer.cs
// OOP Kavramı: ISerializer<T> – Binary implementasyonu
// Serialization: BinaryWriter/BinaryReader ile Binary Serializasyon
// ============================================================
using System.Text;

namespace NotepadApp.Serialization;

/// <summary>
/// Binary formatında serializasyon sağlar.
/// OOP – Interface Implementation: ISerializer&lt;T&gt; arayüzünü uygular.
/// SERIALIZATION: BinaryWriter / BinaryReader ile binary yazma/okuma.
/// NOT: Bu implementasyon DocumentDto için özelleştirilmiştir.
/// </summary>
public class BinaryDocSerializer<T> : ISerializer<T> where T : class
{
    // ── Özellikler ────────────────────────────────────────────────
    public string FormatName => "Binary";

    // ── Serialize ─────────────────────────────────────────────────
    /// <summary>
    /// Nesneyi binary formatına dönüştürüp dosyaya yazar.
    /// SERIALIZATION: BinaryWriter ile ilkel tipleri binary olarak yazar.
    /// </summary>
    public void Serialize(T obj, string filePath)
    {
        // FILE I/O: FileStream + BinaryWriter ile dosyaya binary yazma
        using var fs     = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        using var writer = new BinaryWriter(fs, Encoding.UTF8);

        // Nesneyi reflection ile serialize et
        SerializeObject(writer, obj);
    }

    // ── Deserialize ───────────────────────────────────────────────
    /// <summary>
    /// Binary dosyasından nesneyi okur.
    /// SERIALIZATION: BinaryReader ile binary veriyi okur.
    /// </summary>
    public T? Deserialize(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Binary dosyası bulunamadı: {filePath}");

        // FILE I/O: FileStream + BinaryReader ile dosyadan binary okuma
        using var fs     = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(fs, Encoding.UTF8);

        return DeserializeObject(reader);
    }

    // ── String tabanlı ────────────────────────────────────────────
    public string SerializeToString(T obj)
    {
        using var ms     = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.UTF8);
        SerializeObject(writer, obj);
        // Base64 encode – binary veriyi string olarak temsil et
        return Convert.ToBase64String(ms.ToArray());
    }

    public T? DeserializeFromString(string data)
    {
        byte[] bytes = Convert.FromBase64String(data);
        using var ms     = new MemoryStream(bytes);
        using var reader = new BinaryReader(ms, Encoding.UTF8);
        return DeserializeObject(reader);
    }

    // ── Reflection tabanlı yardımcı metotlar ─────────────────────
    private static void SerializeObject(BinaryWriter writer, T obj)
    {
        // Tüm public string ve değer tipi özellikleri yaz
        var props = typeof(T).GetProperties(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        writer.Write(props.Length);

        foreach (var prop in props)
        {
            writer.Write(prop.Name);
            object? val = prop.GetValue(obj);

            if (val is null)
            {
                writer.Write("__NULL__");
            }
            else if (val is string s)
            {
                writer.Write(s);
            }
            else if (val is int i)
            {
                writer.Write(i.ToString());
            }
            else if (val is float f)
            {
                writer.Write(f.ToString("R"));
            }
            else if (val is bool b)
            {
                writer.Write(b.ToString());
            }
            else if (val is DateTime dt)
            {
                writer.Write(dt.ToString("O"));   // ISO 8601
            }
            else
            {
                writer.Write(val.ToString() ?? "__NULL__");
            }
        }
    }

    private static T? DeserializeObject(BinaryReader reader)
    {
        var instance = Activator.CreateInstance<T>();
        var props    = typeof(T).GetProperties(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        int count = reader.ReadInt32();

        for (int idx = 0; idx < count; idx++)
        {
            string propName = reader.ReadString();
            string rawVal   = reader.ReadString();

            var prop = Array.Find(props, p => p.Name == propName);
            if (prop is null || !prop.CanWrite || rawVal == "__NULL__")
                continue;

            try
            {
                object? converted = prop.PropertyType switch
                {
                    var t when t == typeof(string)   => rawVal,
                    var t when t == typeof(int)      => int.Parse(rawVal),
                    var t when t == typeof(float)    => float.Parse(rawVal),
                    var t when t == typeof(bool)     => bool.Parse(rawVal),
                    var t when t == typeof(DateTime) => DateTime.Parse(rawVal),
                    _                                => null
                };

                if (converted is not null)
                    prop.SetValue(instance, converted);
            }
            catch
            {
                // Dönüştürme hatası – atla
            }
        }

        return instance;
    }
}
