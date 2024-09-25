# Net HealthCheck Proof Projesi

### Projenin Amacı

Bu proje, PostgreSQL, Redis ve üçüncü parti bir API için HealthCheck mekanizmasının nasıl kullanılabileceğini gösteren
bir örnek projedir. Ayrıca, projede GitHub Actions kullanılarak unit testler hem bir pull request oluşturulduğunda hem
de master branşına birleştirme yapıldığında otomatik olarak çalıştırılır. Bu sayede, kodun her iki durumda da sağlıklı
ve hatasız olduğundan emin olunur.

### Proje Yapısı

Bu proje aşağıdaki katmanlardan oluşur:

* Api.Controllers: API uç noktalarını tanımlar ve HTTP isteklerini işler.
* Api.Application: İş mantığını içerir ve uygulama servislerini sağlar.
* Api.Infrastructure: Veri erişimi ve veri modeli yapılandırması ile ilgili kodları içerir.
* Api.Domain: Veri modeli sınıflarını tanımlar.
* Api.Integration: Üçüncü parti API'ye erişim sağlar.

### Kurulum

Projeyi çalıştırmak için aşağıdaki adımları izleyebilirsiniz:

1. Gereksinimler:

- .NET Core SDK 7.0
- Docker

2. Proje Dosyalarını İndirin:

```
https://github.com/BerkayMehmetSert/net.HealthCheck.Proof.git
cd net.HealthCheck.Proof
```

3. Bağımlılıkları Yükleyin:

```
dotnet restore
```

4. Projeyi Çalıştırın:

```
dotnet run
```

### Kullanım

#### API Uç Noktaları

**Get All Posts**

* GET /api/posts
* Açıklama: Tüm postları getirir.
* Yanıt: 200 OK ve post listesi.

**Get Post By ID**

* GET /api/posts/{id}
* Açıklama: Belirtilen ID'ye sahip postu getirir.
* Parametreler: id (postun ID'si)
* Yanıt: 200 OK ve post bilgileri.

### HealthCheck

Bu proje, PostgreSQL, Redis ve üçüncü parti bir API için HealthCheck mekanizmasını kullanır. PostgreSQL ve Redis sağlık
durumunu kontrol etmek için `HealthCheck` kütüphanesini kullanır.

#### HealthCheck Uç Noktaları

UI üzerinden sağlık durumunu kontrol etmek için aşağıdaki uç noktaları kullanabilirsiniz:

* /status

JSON formatında sağlık durumunu kontrol etmek için aşağıdaki uç noktaları kullanabilirsiniz:

* /health

### GitHub Actions

Proje, GitHub Actions kullanarak sürekli entegrasyon sağlar. Her pull request ve doğrudan master branşına yapılan
birleştirme işlemleri sırasında unit testler otomatik olarak çalıştırılır. Bu, kod kalitesini ve işlevselliğini korumaya
yardımcı olur.

#### GitHub Actions Yapılandırması

`actions.yml`: Testlerin otomatik olarak çalıştırılmasını sağlayan GitHub Actions iş akışını tanımlar. build ve test
adımlarını içerir.