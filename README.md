# TaskSurvey - Aplikasi Web Blazor

Aplikasi manajemen survey modern yang dibangun dengan ASP.NET Blazor Web Framework, menampilkan arsitektur yang bersih dan antarmuka pengguna yang interaktif.

## 📋 Deskripsi

TaskSurvey adalah aplikasi survey berbasis web yang memanfaatkan kekuatan Blazor untuk memberikan pengalaman yang mulus dan interaktif dalam membuat, mengelola, dan menganalisis survey. Aplikasi ini menggunakan C# untuk logika client dan server-side, memungkinkan pengembangan full-stack dengan satu bahasa pemrograman.

## ✨ Fitur

- **Pembuatan Survey Interaktif** - Membangun survey dinamis dengan berbagai tipe pertanyaan
- **Update Real-time** - Rendering real-time Blazor untuk pengalaman pengguna yang smooth
- **State Management** - Layanan state terpusat untuk aliran data yang konsisten
- **Dukungan Docker** - Deployment mudah dengan containerization

## 🏗️ Arsitektur

Proyek ini mengikuti arsitektur yang terorganisir dengan baik dengan pemisahan concern yang jelas:

```
task-survey-blazor/
├── Components/          # Komponen Blazor yang dapat digunakan kembali
├── Infrastructure/      # Infrastruktur inti dan data layer
├── StateServices/       # Manajemen state aplikasi
├── wwwroot/            # Aset statis (CSS, JS, gambar)
├── Properties/         # Properties proyek dan pengaturan launch
├── Program.cs          # Entry point aplikasi
└── appsettings.json    # Pengaturan konfigurasi
```

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core Blazor
- **Bahasa**: C# (56.0%) & HTML (42.9%)
- **Runtime**: .NET 8.0
- **Containerization**: Docker
- **Development**: Visual Studio Code

## 📦 Persyaratan

Sebelum menjalankan aplikasi ini, pastikan Anda telah menginstal:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) (direkomendasikan) atau Visual Studio
- [Docker](https://www.docker.com/) (opsional, untuk deployment dengan container)

## 🚀 Cara Memulai

### Development Lokal

1. **Clone repository**
   ```bash
   git clone https://github.com/Mosuvy/task-survey-blazor.git
   cd task-survey-blazor
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Jalankan aplikasi**
   ```bash
   dotnet run
   ```

4. **Buka browser**
   
   Navigasi ke `https://localhost:5215`

### Deployment dengan Docker

1. **Build dan jalankan dengan Docker Compose**
   ```bash
   docker-compose up --build
   ```

## 🔧 Konfigurasi

Aplikasi dapat dikonfigurasi melalui file `appsettings.json` dan `appsettings.Development.json`. Opsi konfigurasi utama meliputi:

- Connection string database
- Level logging
- Pengaturan spesifik aplikasi

## 📁 Struktur Proyek

### Components
Berisi semua komponen Blazor yang dapat digunakan kembali yang membentuk UI aplikasi. Komponen-komponen ini dirancang agar modular dan mudah dipelihara.

### Infrastructure
Menampung kode infrastruktur inti termasuk:
- Layer akses data
- Pattern repository
- Context database
- Integrasi layanan eksternal

### StateServices
Mengimplementasikan layanan manajemen state terpusat untuk mempertahankan state aplikasi di berbagai komponen dan halaman.

### wwwroot
File statis termasuk:
- Stylesheet CSS
- File JavaScript
- Gambar dan ikon
- Library pihak ketiga

## 🧪 Development

### Build Proyek
```bash
dotnet build
```

### Menjalankan Test
```bash
dotnet test
```

### Watch Mode (Auto-rebuild)
```bash
dotnet watch run
```

## 🎨 Code Style

Proyek ini mengikuti konvensi coding C# standar:
- PascalCase untuk nama class dan member publik
- camelCase untuk field private dan variabel lokal
- Penggunaan async/await untuk operasi asynchronous
- Arsitektur berbasis komponen dengan komponen Blazor

## 👤 Author

**Mosuvy**
- GitHub: [@Mosuvy](https://github.com/Mosuvy)

## 🙏 Acknowledgments

- Dibangun dengan [ASP.NET Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- Terima kasih kepada komunitas .NET untuk dokumentasi dan dukungan yang luar biasa

---
