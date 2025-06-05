using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Arena_savascilari isimli bir namespace (isim alanı) oluşturuluyor.
// Bu, kodun organize edilmesine ve diğer projelerle çakışmasının önlenmesine yardımcı olur.
namespace Arena_savascilari
{
    // Karakter adında bir temel sınıf (base class) tanımlanıyor.
    public class Karakter
    {
        // Karakterin adı
        public string Isim { get; set; }

        // Karakterin can puanı
        public int Can { get; set; }

        // Karakterin saldırı gücü
        public int SaldiriGucu { get; set; }
        
        // Karakterin mana (özel güç kullanımı için)
        public int Mana { get; set; }

        // Karakter yaşıyor mu kontrolü (can > 0 ise true döner)
        public bool Yasiyor => Can > 0;

        // Saldırı sayısını tüm karakterler için ortak olarak tutar (static)
        public static int ToplamSaldiriSayisi = 0;

        // Karakter sınıfının kurucu metodu (constructor)
        public Karakter(string isim, int can, int guc, int mana)
        {
            Isim = isim;
            Can = can;
            SaldiriGucu = guc;
            Mana = mana;
        }

        // Karakterin başka bir karaktere saldırmasını sağlar
        public void Saldir(Karakter hedef)
        {
            hedef.Can -= SaldiriGucu; // Hedefin canı saldırı gücü kadar azalır
            ToplamSaldiriSayisi++;    // Saldırı sayısı arttırılır
            Console.WriteLine($"{Isim} saldırdı! {hedef.Isim} {SaldiriGucu} hasar aldı.");
        }

        // Karakterin manasını yenilemesini sağlar
        public void ManaYenile()
        {
            Mana += 25;
            Console.WriteLine($"{Isim} mana yeniledi. Yeni mana: {Mana}");
        }

        // Karakter bilgilerini ekrana yazdırır
        public void BilgiGoster()
        {
            Console.WriteLine($"İsim: {Isim} | Can: {Can} | Mana: {Mana}");
        }
    }

    // Oyuncu adında Karakter sınıfından türeyen bir sınıf (inheritance)
    public class Oyuncu : Karakter
    {
        // Oyuncu sınıfının kurucu metodu, sabit değerler ile çağırılır
        public Oyuncu(string isim) : base(isim, 120, 18, 60) { }

        // Oyuncuya özel saldırı fonksiyonu
        public void OzelSaldiri(Karakter hedef)
        {
            if (Mana >= 30)
            {
                int hasar = SaldiriGucu * 2 + 5; // Özel saldırı daha fazla hasar verir
                hedef.Can -= hasar;
                Mana -= 30; // Mana harcanır
                ToplamSaldiriSayisi++;
                Console.WriteLine($"{Isim} özel saldırı yaptı! {hedef.Isim} {hasar} hasar aldı.");
            }
            else
            {
                Console.WriteLine("Yeterli mana yok!");
            }
        }
    }

    // Düşman sınıfı, Karakter'den türetilmiştir
    public class Dusman : Karakter
    {
        public Dusman(string isim, int can, int guc, int mana) : base(isim, can, guc, mana) { }

        // Düşmana özel saldırı fonksiyonu
        public void OzelSaldiri(Karakter hedef)
        {
            if (Mana >= 20)
            {
                int hasar = SaldiriGucu + 10;
                hedef.Can -= hasar;
                Mana -= 20;
                ToplamSaldiriSayisi++;
                Console.WriteLine($"{Isim} özel düşman saldırısı yaptı! {hedef.Isim} {hasar} hasar aldı.");
            }
            else
            {
                // Mana yetmezse normal saldırı yapar
                Saldir(hedef);
            }
        }
    }

    // Program sınıfı, uygulamanın başlangıç noktasıdır (Main metodu)
    class Program
    {
        static void Main(string[] args)
        {
            // Yeni bir oyuncu oluşturuluyor
            Oyuncu oyuncu = new Oyuncu("Yudum");

            // Üç düşmandan oluşan bir liste oluşturuluyor
            List<Dusman> dusmanlar = new List<Dusman>
            {
                new Dusman("Zombi", 80, 12, 35),
                new Dusman("Goblin", 90, 15, 45),
                new Dusman("Ejderha", 130, 25, 70)
            };

            // Her düşman için savaş döngüsü
            foreach (var dusman in dusmanlar)
            {
                Console.WriteLine($"\n Yeni düşman: {dusman.Isim}");

                // Oyuncu ve düşman hayattaysa savaş devam eder
                while (oyuncu.Yasiyor && dusman.Yasiyor)
                {
                    Console.WriteLine("\n Tur Başladı ");
                    oyuncu.BilgiGoster(); // Oyuncu bilgileri
                    dusman.BilgiGoster(); // Düşman bilgileri

                    // Oyuncunun seçim yapması isteniyor
                    Console.WriteLine("\nSeçim yap:\n1. Saldırı\n2. Özel Saldırı\n3. Mana Yenile");
                    string secim = Console.ReadLine();

                    // Oyuncunun seçimine göre işlem yapılır
                    switch (secim)
                    {
                        case "1":
                            oyuncu.Saldir(dusman);
                            break;
                        case "2":
                            oyuncu.OzelSaldiri(dusman);
                            break;
                        case "3":
                            oyuncu.ManaYenile();
                            break;
                        default:
                            Console.WriteLine("Geçersiz seçim.");
                            continue;
                    }

                    // Düşman hayattaysa saldırı yapar
                    if (dusman.Yasiyor)
                    {
                        Random r = new Random();
                        int hamle = r.Next(0, 2); // 0 ya da 1: rastgele hamle

                        if (hamle == 0)
                            dusman.Saldir(oyuncu);
                        else
                            dusman.OzelSaldiri(oyuncu);
                    }
                }

                // Savaşın sonucu kontrol edilir
                if (!oyuncu.Yasiyor)
                {
                    Console.WriteLine(" Oyuncu öldü. Oyun bitti!");
                    break;
                }
                else
                {
                    Console.WriteLine($" {dusman.Isim} yenildi!");
                }
            }

            // Oyunun sonunda toplam saldırı sayısı gösterilir
            Console.WriteLine($"\n Toplam Saldırı Sayısı: {Karakter.ToplamSaldiriSayisi}");
        }
    }
}

