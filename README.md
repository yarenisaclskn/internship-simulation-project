3D Flight Simulation & Geospatial Data Management

Bu proje; ASP.NET MVC mimarisi üzerine kurulu, CesiumJS kütüphanesi ile 3D harita görselleştirmesi sunan ve uçuş simülasyonu gerçekleştiren kapsamlı bir Coğrafi Bilgi Sistemi (CBS) uygulamasıdır. Kullanıcılar, 3D modelleri (.glb) harita üzerinde konumlandırabilir, karmaşık rotalar oluşturabilir ve bu süreçlerin sonunda raporlar alabilirler.


🌟 Ana Özellikler

3D Model Entegrasyonu: .glb formatındaki 3D modelleri koordinat (Lat/Lon/Alt) ve eğim (Pitch) bilgileriyle haritaya ekleme.


Dinamik Uçuş Simülasyonu: Belirlenen rotalar üzerinde, hız ve irtifa verilerine bağlı olarak nesnelerin 3D simülasyonu.


Rota ve Durak Yönetimi: Nesnelere özel rota verileri atama ve simülasyon duraklarını gerçek zamanlı takip etme.


Otomatik Raporlama: Tamamlanan simülasyonlar için PDF, Excel ve Word formatlarında uçuş raporu (ortalama hız, mesafe, irtifa vb.) oluşturma.


Gelişmiş CMS Paneli: Harita nesnelerinin görünürlüğünü, konumunu ve modellerini yönetmek için kullanıcı dostu arayüz.


🛠️ Teknik Altyapı

Backend: ASP.NET MVC, C#.


Veritabanı: PostgreSQL (Npgsql sürücüsü ile asenkron yönetim).


Harita Motoru: CesiumJS (3D Globe Rendering).


Frontend: JavaScript (ES6+), jQuery, Bootstrap, CSS3.


Raporlama Araçları: html2pdf, XLSX.js, html-to-docx, PDF.js.


📁 Veri Modelleri

HaritaNesnesi: Nesnenin adı, koordinatları, 3D dosya yolu ve görünürlük durumunu tutar.


HaritaNesnesiRota: Rota isimlerini ve rotaya ait koordinat dizilerini (JSON formatında) saklar.


⚙️ Kurulum ve Başlatma
Veritabanı Yapılandırması: * PostgreSQL üzerinde harita_nesneleri ve harita_nesneleri_rota tablolarını oluşturun.

Web.config dosyasındaki PostgreConnection bağlantı dizesini güncelleyin.


3D Model Dizini: Proje ana dizininde ~/Models/3D/ klasörünün yazma izinlerine sahip olduğundan emin olun.

NuGet Paketleri: Npgsql ve Newtonsoft.Json paketlerini yükleyin.

Çalıştırma: Visual Studio ile projeyi derleyin ve tarayıcıda index sayfasını açın.
