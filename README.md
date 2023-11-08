# SwapProject
Bu proje temel olarak bitcoin alış ve satış işlemleri yapan Backend kısmını tamamen API'ler üzerine kurduğum bir proje.
# Proje Hakkında
* Kullanıcı sisteme kayıt olurken kullanıcıya ait coinlerin cüzdanları ve bakiyeleri tanımlanıyor.
* Kullanıcı sisteme giriş yapmış ise şifresini değiştirebiliyor.
* Kullanıcı kayıt olurken email, kullanıcı adı ve telefon numarası sistemde var ise kayıt işlemi gerçekleştirmiyor.
* Başarılı bir şekilde kayıt olan kullanıcıya "member" rolü atanıyor.
* Kullanıcının oluşturduğu alış veya satış emirlerini statüsü aktif olan pariteler üzerinden gerçekleşiyor. Parite: iki kripto para biriminin karşılıklı değeridir. (USDT (baz/alınan) / TRY (karşıt/satılan) - BTC/ETH gibi.) Bu birimlerden alış veya satış yapılırken sol tarafta bulunan para birimi kazanılırken, sağ tarafta bulunacak olan para birimi düşürülecektir.
* Her bir paritenin benim belirlemiş olduğum kendine ait bir komisyon ücreti bulunmaktadır.
* Coin fiyatları için public olarak sunulan Binance apilerini kullandım. Bir kullanıcı bir satış veya alış emri oluşturduğunda Binance apisine istek atıp response’ta dönen fiyata göre alış veya satış emri verebiliyor.
* Bir alış emri veya bir satış emri oluşturulduğunda hesaplanan karşıt kripto para birimi miktarı kullanıcı cüzdanından düşülüyor, ve bir sonraki işlemini kalan bakiyesi kadar yapabiliyor.
* Bir alış emri veya bir satış emri ile eşleştikten sonra baz olan kripto para birimi komisyonu düşülerek cüzdanına ekleniyor.
* Şirket cüzdanı oluşturdum. İçerisinde tüm aktif kripto para birimleri bulunmaktadır. Her yapılan alış satış işleminden sonra kesilen komisyon miktarı şirket cüzdanındaki gerekli kripto para birimine ekleniyor. Örneğin; (ETH-USDT paritesinde kesilen komisyon USDT üzerinden olup bu komisyon şirketin USDT cüzdanına yansıyacaktır.)
* Admin sistem içerisindeki bütün alanlara erişebiliyor ve işlemleri yapma yetkisine sahip.
* Bir kullanıcı bir alış veya satış emri oluşturabilir veya iptal edebilir. Paritileri ve kripto para birimlerini görüntüleyebilir. Bütün al-sat geçmişine ait en yüksek miktarlı işlemleri paging yaparak görüntüleyebilir. Kendi al-sat geçmişini ve cüzdanını görüntüleyebilir. 
* Alış ve Satış emirlerinde statü durumu bulunmaktadır(Tamamlandı-Bekliyor-İptal edildi)
* Yapılan tüm listelemelerde filtreleme, search ve paging işlemlerini uyguladım.
* Proje içerisinde admin için ayrı, user için ayrı apiler yazıldı. Managerlar’ı da ayrı oluşturuldu.
# Kullanmış Olduğum Teknolojiler Ve Yapılar
* .Net Core 6.0
* Entity Framework Core
* DTO
* Repository Design Pattern
* Dependency Injection
* Autofac
* Magic String
* Enum
* N-Tier Architecture
* JWT
* Rol Yönetimi
* Pagination
* Error & Success Result Yapısı
* Web API
* Authentication & Authorization
* Fluent Validation
* Extension Yapısı

Veritabanı Olarak;
* MSSQL
* Database First
