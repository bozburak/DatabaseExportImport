English Description
This script automates the process of exporting databases from an Azure SQL server, dropping them from a local SQL server, and then importing them back into the local server. It uses SqlPackage.exe for the export and import operations and System.Data.SqlClient for database management.

Configuration: Set up server names, login credentials, and paths.
Database List: Define the databases to be processed.
Directory Check: Ensure the export directory exists.
Export Databases: Loop through each database, export it to a .bacpac file.
Drop Local Databases: Remove the databases from the local server if they exist.
Import Databases: Import the .bacpac files back into the local server.


Türkçe Açıklama
Bu betik, Azure SQL sunucusundan veritabanlarını dışa aktarma, yerel SQL sunucusundan silme ve ardından yerel sunucuya geri aktarma işlemlerini otomatikleştirir. Dışa ve içe aktarma işlemleri için SqlPackage.exe ve veritabanı yönetimi için System.Data.SqlClient kullanır.

Yapılandırma: Sunucu adlarını, giriş bilgilerini ve yolları ayarla.
Veritabanı Listesi: İşlenecek veritabanlarını tanımla.
Dizin Kontrolü: Dışa aktarma dizininin var olduğundan emin ol.
Veritabanlarını Dışa Aktar: Her veritabanını döngüyle gezerek .bacpac dosyasına dışa aktar.
Yerel Veritabanlarını Sil: Yerel sunucudan veritabanlarını varsa sil.
Veritabanlarını İçe Aktar: .bacpac dosyalarını yerel sunucuya geri yükle.
