# 📝 NotepadApp – File I/O & Serialization Demo

> **A comprehensive Windows Forms application built with .NET 8, designed to demonstrate core Object-Oriented Programming (OOP) concepts, File I/O operations, and Serialization techniques in C#.**
>
> *.NET 8 ile geliştirilmiş, C# dilindeki temel Nesne Yönelimli Programlama (OOP) prensiplerini, Dosya Girdi/Çıktı (File I/O) işlemlerini ve Serileştirme (Serialization) tekniklerini gösteren kapsamlı bir Windows Forms uygulaması.*

---

## 🌎 Language / Dil
- [English (#english)](#-notepadapp--file-io--serialization-demo)
- [Türkçe (#türkçe)](#-notepadapp--dosya-girdiçıktı-ve-serileştirme-demosu)

---

# 🇬🇧 English

NotepadApp is a tabbed text editor showcasing how real-world applications employ clean architecture, design patterns, and C# features to manage documents, application configurations, and metadata serialization.

## 🚀 Features
- **Tabbed Interface:** Manage multiple text documents simultaneously.
- **File Operations (File I/O):** Create, open, save, and save text files with different encodings.
- **Find & Replace:** Search and replace text with case sensitivity support.
- **Recent Files List:** Persists a history of the last 10 opened documents in JSON format.
- **Application Settings:** Save preferences like font family, size, theme (Light/Dark), and window size.
- **Metadata Serialization:** Export and import document metadata (title, path, creation date, etc.) using:
  - **JSON** (`System.Text.Json`)
  - **XML** (`System.Xml.Serialization`)
  - **Binary** (`BinaryWriter` / `BinaryReader` with custom Reflection)

---

## 🏗️ Project Architecture & OOP Principles

This project serves as a practical guide for Object-Oriented Programming:

| OOP Concept | Class / Interface | Description |
| :--- | :--- | :--- |
| **Encapsulation** | `BaseDocument`, `AppSettings` | Restricts direct access to fields using properties with custom validation and change-tracking. |
| **Inheritance** | `NoteDocument` | Inherits common document states and behaviors from the abstract class `BaseDocument`. |
| **Polymorphism** | `NoteDocument` | Overrides abstract methods `Save()`, `Load()`, and `GetMimeType()` to implement specific text behaviors. |
| **Abstraction** | `ISerializer<T>`, `BaseDocument` | Defines clear contracts and abstract templates without exposing execution details. |
| **Generics** | `ISerializer<T>` | Supports type-independent serialization of generic models (`T where T : class`). |
| **Singleton Pattern** | `AppSettings` | Guarantees a single application-wide instance of configurations using thread-safe locking. |
| **Composition** | `MainForm` | Uses instances of helper managers (`RecentFilesManager`, `FileManager`) to delegate responsibilities. |

---

## 📂 File & Directory Structure

```
Notepad-Application-File-I-O-serialization/
│
├── NotepadApp.csproj       # Project configuration file
├── Program.cs             # Application entry point containing Main()
├── nuget.config           # Local package restore source configurations
│
├── Models/                # Data structures and classes representing the state
│   ├── BaseDocument.cs    # Abstract class for all document types
│   ├── NoteDocument.cs    # Concrete implementation of a text document (DTO included)
│   └── AppSettings.cs     # Configuration settings (Singleton pattern)
│
├── Managers/              # Service classes handling system interactions
│   ├── FileManager.cs     # Centralized File I/O operations (Lazy reading, binary bytes)
│   └── RecentFilesManager.cs # Handles history of recently opened files
│
├── Serialization/         # Serializers converting objects to/from various formats
│   ├── ISerializer.cs     # Generic contract for serializers
│   ├── JsonDocSerializer.cs # JSON serialization implementation
│   ├── XmlDocSerializer.cs  # XML serialization implementation
│   └── BinaryDocSerializer.cs # Reflection-based custom binary serialization
│
├── Forms/                 # Windows Forms User Interfaces
│   ├── MainForm.cs        # Main editor window with menus and status bars
│   ├── SettingsForm.cs    # Application configuration menu
│   ├── FindReplaceForm.cs # Search and replace dialog
│   └── AboutForm.cs       # Information window describing OOP concepts
│
└── Resources/             # Asset files
    └── notepad.ico        # Main application icon
```

---

## 🛠️ Build and Run Instructions

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows OS (required for Windows Forms)

### Commands
1. Clone the repository and navigate to the project directory:
   ```bash
   cd Notepad-Application-File-I-O-serialization
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Build the application:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

---

<br/>

# 🇹🇷 Türkçe

# 📝 NotepadApp – Dosya Girdi/Çıktı ve Serileştirme Demosu

NotepadApp, temiz mimari, tasarım desenleri ve C# özelliklerinin; belgeleri yönetmek, uygulama yapılandırmalarını kontrol etmek ve belge üstverilerini serileştirmek için gerçek dünyada nasıl kullanıldığını gösteren sekmeli bir metin editörüdür.

## 🚀 Özellikler
- **Sekmeli Arayüz:** Aynı anda birden fazla metin belgesiyle çalışabilme.
- **Dosya İşlemleri (File I/O):** Farklı kodlamalarla metin dosyası oluşturma, açma, kaydetme ve farklı kaydetme.
- **Bul & Değiştir:** Büyük/küçük harf duyarlılığı destekli metin arama ve değiştirme.
- **Son Açılan Dosyalar Listesi:** Son açılan 10 belgenin geçmişini JSON formatında saklar.
- **Uygulama Ayarları:** Yazı tipi, boyutu, tema (Koyu/Açık) ve pencere konumunu kaydeder.
- **Üstveri Serileştirmesi:** Belge bilgilerini (başlık, dosya yolu, oluşturulma tarihi vb.) dışa ve içe aktarma:
  - **JSON** (`System.Text.Json`)
  - **XML** (`System.Xml.Serialization`)
  - **Binary** (Özel Reflection mekanizmasıyla `BinaryWriter` / `BinaryReader`)

---

## 🏗️ Proje Mimarisi ve OOP Prensipleri

Bu proje, Nesne Yönelimli Programlama dersi için pratik bir rehber niteliğindedir:

| OOP Kavramı | Sınıf / Arayüz | Açıklama |
| :--- | :--- | :--- |
| **Encapsulation (Kapsülleme)** | `BaseDocument`, `AppSettings` | Alanlara doğrudan erişimi engelleyerek kontrollü özellikler (Properties) ve değişiklik takibi sağlar. |
| **Inheritance (Kalıtım)** | `NoteDocument` | Ortak belge durum ve davranışlarını soyut `BaseDocument` sınıfından devralır. |
| **Polymorphism (Çok Biçimlilik)** | `NoteDocument` | Soyut `Save()`, `Load()` ve `GetMimeType()` metotlarını ezerek (override) metin dosyalarına özel davranış kazandırır. |
| **Abstraction (Soyutlama)** | `ISerializer<T>`, `BaseDocument` | Kodun iç detaylarını gizleyerek net arayüzler ve şablonlar tanımlar. |
| **Generics (Genel Tipler)** | `ISerializer<T>` | Tip bağımsız serileştirme işlemleri gerçekleştirmeyi sağlar (`T where T : class`). |
| **Singleton Deseni** | `AppSettings` | Uygulama genelinde ayarların tek bir örneğinin (instance) kullanılmasını iş parçacığı güvenli (thread-safe) şekilde garanti eder. |
| **Composition (Bileşim)** | `MainForm` | `RecentFilesManager` ve `FileManager` örneklerini barındırarak işlerin ilgili sınıflara devredilmesini sağlar. |

---

## 📂 Dosya ve Dizin Yapısı

```
Notepad-Application-File-I-O-serialization/
│
├── NotepadApp.csproj       # Proje yapılandırma dosyası
├── Program.cs             # Uygulama giriş noktası (Main metodu)
├── nuget.config           # Yerel NuGet paket kaynağı yapılandırması
│
├── Models/                # Durumu temsil eden veri yapıları
│   ├── BaseDocument.cs    # Tüm belge türleri için soyut temel sınıf
│   ├── NoteDocument.cs    # Düz metin belgesi somut sınıfı (DTO içerir)
│   └── AppSettings.cs     # Uygulama ayarları sınıfı (Singleton deseni)
│
├── Managers/              # Sistem etkileşimlerini yöneten servis sınıfları
│   ├── FileManager.cs     # Merkezi Dosya Girdi/Çıktı (Lazy okuma, binary)
│   └── RecentFilesManager.cs # Son açılan dosyaların geçmişini yönetir
│
├── Serialization/         # Nesne dönüşümlerini yapan sınıflar
│   ├── ISerializer.cs     # Serileştiriciler için genel arayüz sözleşmesi
│   ├── JsonDocSerializer.cs # JSON serileştirme uygulaması
│   ├── XmlDocSerializer.cs  # XML serileştirme uygulaması
│   └── BinaryDocSerializer.cs # Reflection tabanlı özel binary serileştirme
│
├── Forms/                 # Windows Forms Kullanıcı Arayüzleri
│   ├── MainForm.cs        # Ana editör penceresi, menüler ve durum çubukları
│   ├── SettingsForm.cs    # Uygulama yapılandırma arayüzü
│   ├── FindReplaceForm.cs # Bul ve Değiştir iletişim penceresi
│   └── AboutForm.cs       # OOP kavramlarını açıklayan bilgi penceresi
│
└── Resources/             # Görsel varlıklar
    └── notepad.ico        # Ana uygulama ikon dosyası
```

---

## 🛠️ Derleme ve Çalıştırma Talimatları

### Gereksinimler
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows İşletim Sistemi (Windows Forms kullanımı için gereklidir)

### Komutlar
1. Depoyu klonlayın ve proje dizinine gidin:
   ```bash
   cd Notepad-Application-File-I-O-serialization
   ```
2. Bağımlılıkları geri yükleyin:
   ```bash
   dotnet restore
   ```
3. Uygulamayı derleyin:
   ```bash
   dotnet build
   ```
4. Uygulamayı çalıştırın:
   ```bash
   dotnet run
   ```
