// ============================================================
// Serialization/ISerializer.cs
// OOP Kavramı: Generic Interface (Abstraction + Generics)
// ============================================================
namespace NotepadApp.Serialization;

/// <summary>
/// Serializasyon işlemleri için generic arayüz.
/// OOP – Interface: Sözleşme tanımlar, implementasyonu zorlamaz.
/// OOP – Generics: Tür bağımsız çalışır (T herhangi bir sınıf olabilir).
/// </summary>
public interface ISerializer<T> where T : class
{
    /// <summary>Nesneyi bir dosyaya serialize eder.</summary>
    void Serialize(T obj, string filePath);

    /// <summary>Dosyadan nesneyi deserialize eder.</summary>
    T? Deserialize(string filePath);

    /// <summary>Nesneyi string olarak serialize eder.</summary>
    string SerializeToString(T obj);

    /// <summary>String'den nesneyi deserialize eder.</summary>
    T? DeserializeFromString(string data);

    /// <summary>Serializasyonun desteklediği formatı döndürür.</summary>
    string FormatName { get; }
}
