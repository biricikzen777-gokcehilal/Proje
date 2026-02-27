using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    static string menuDosya = "menu.json";
    static string sepetDosya = "sepet.json";

    static Menu menu;
    static Sepet sepet;

    static void Main()
    {
        MenuYukleVeyaOlustur();
        SepetiYukleVeyaOlustur();
        SecenekGoster();
    }

    // ---------------- MENU ----------------
    static void MenuYukleVeyaOlustur()
    {
        if (File.Exists(menuDosya))
        {
            string okunan = File.ReadAllText(menuDosya);

            if (!string.IsNullOrWhiteSpace(okunan))
            {
                menu = JsonSerializer.Deserialize<Menu>(okunan);
            }
        }

        // menu yoksa veya içi boş geldiyse
        if (menu == null || menu.Categories == null)
        {
            menu = OrnekMenuOlustur();
            MenuKaydet();
        }
    }

    static Menu OrnekMenuOlustur()
    {
        Menu m = new Menu();
        m.RestaurantName = "Test Restoran";
        m.Categories = new List<Kategori>();

        Kategori pideler = new Kategori();
        pideler.CategoryName = "Pideler";
        pideler.Products = new List<Urun>();
        pideler.Products.Add(new Urun { Id = 1, Name = "Kıymalı Pide", Price = 150 });
        pideler.Products.Add(new Urun { Id = 2, Name = "Kuşbaşılı Pide", Price = 180 });
        pideler.Products.Add(new Urun { Id = 3, Name = "Kaşarlı Pide", Price = 130 });

        Kategori icecekler = new Kategori();
        icecekler.CategoryName = "İçecekler";
        icecekler.Products = new List<Urun>();
        icecekler.Products.Add(new Urun { Id = 10, Name = "Ayran", Price = 30 });
        icecekler.Products.Add(new Urun { Id = 11, Name = "Kola", Price = 45 });
        icecekler.Products.Add(new Urun { Id = 12, Name = "Su", Price = 15 });

        m.Categories.Add(pideler);
        m.Categories.Add(icecekler);

        return m;
    }

    static void MenuKaydet()
    {
        string json = JsonSerializer.Serialize(menu);
        File.WriteAllText(menuDosya, json);
    }

    static void MenuGoster()
    {
        Console.WriteLine();
        Console.WriteLine("Restoran: " + menu.RestaurantName);
        Console.WriteLine("---- MENÜ ----");

        for (int i = 0; i < menu.Categories.Count; i++)
        {
            Console.WriteLine("Kategori: " + menu.Categories[i].CategoryName);

            for (int j = 0; j < menu.Categories[i].Products.Count; j++)
            {
                Urun u = menu.Categories[i].Products[j];
                Console.WriteLine(u.Id + " - " + u.Name + " (" + u.Price + " TL)");
            }

            Console.WriteLine();
        }
    }

    static Urun UrunBul(int id)
    {
        for (int i = 0; i < menu.Categories.Count; i++)
        {
            for (int j = 0; j < menu.Categories[i].Products.Count; j++)
            {
                if (menu.Categories[i].Products[j].Id == id)
                    return menu.Categories[i].Products[j];
            }
        }
        return null;
    }

    // ---------------- SEPET ----------------
    static void SepetiYukleVeyaOlustur()
    {
        static void SepetiYukleVeyaOlustur()
        {
            if (File.Exists(sepetDosya))
            {
                string okunan = File.ReadAllText(sepetDosya);
                sepet = JsonSerializer.Deserialize<Sepet>(okunan);
            }

            if (sepet == null)
            {
                sepet = new Sepet();
                sepet.Items = new List<SepetItem>();
                SepetiKaydet();
            }
        }

        if (sepet == null || sepet.Items == null)
        {
            sepet = new Sepet();
            sepet.Items = new List<SepetItem>();
            SepetiKaydet();
        }
    }

    static void SepetiKaydet()
    {
        string json = JsonSerializer.Serialize(sepet);
        File.WriteAllText(sepetDosya, json);
    }

    static void SepeteEkle()
    {
        MenuGoster();

        Console.Write("Ürün id gir: ");
        string idText = Console.ReadLine();

        int id;
        if (!int.TryParse(idText, out id))
        {
            Console.WriteLine("Id sayı olmalı.");
            return;
        }

        Urun urun = UrunBul(id);
        if (urun == null)
        {
            Console.WriteLine("Ürün bulunamadı.");
            return;
        }

        Console.Write("Kaç adet? ");
        string adetText = Console.ReadLine();

        int adet;
        if (!int.TryParse(adetText, out adet) || adet <= 0)
        {
            Console.WriteLine("Adet 1 veya daha büyük olmalı.");
            return;
        }

        // Sepette varsa adet arttır
        bool bulundu = false;
        for (int i = 0; i < sepet.Items.Count; i++)
        {
            if (sepet.Items[i].ProductId == urun.Id)
            {
                sepet.Items[i].Quantity += adet;
                bulundu = true;
                break;
            }
        }

        // Yoksa yeni ekle
        if (!bulundu)
        {
            SepetItem item = new SepetItem();
            item.ProductId = urun.Id;
            item.Name = urun.Name;
            item.UnitPrice = urun.Price;
            item.Quantity = adet;
            sepet.Items.Add(item);
        }

        SepetiKaydet();
        Console.WriteLine("Sepete eklendi: " + urun.Name + " x" + adet);
    }

    static double SepetToplam()
    {
        double toplam = 0;
        for (int i = 0; i < sepet.Items.Count; i++)
        {
            toplam += sepet.Items[i].UnitPrice * sepet.Items[i].Quantity;
        }
        return toplam;
    }

    static void SepetiListele()
    {
        Console.WriteLine();
        Console.WriteLine("---- SEPET ----");

        if (sepet.Items.Count == 0)
        {
            Console.WriteLine("Sepet boş.");
            return;
        }

        for (int i = 0; i < sepet.Items.Count; i++)
        {
            SepetItem it = sepet.Items[i];
            double satirToplam = it.UnitPrice * it.Quantity;
            Console.WriteLine((i + 1) + ") " + it.Name + " | Adet: " + it.Quantity + " | Birim: " + it.UnitPrice + " | Toplam: " + satirToplam);
        }

        Console.WriteLine("Genel Toplam: " + SepetToplam() + " TL");
    }

    static void SepettenSilVeyaAzalt()
    {
        SepetiListele();
        if (sepet.Items.Count == 0) return;

        Console.Write("Silmek/Azaltmak istediğin ürünün id'si: ");
        string idText = Console.ReadLine();

        int id;
        if (!int.TryParse(idText, out id))
        {
            Console.WriteLine("Id sayı olmalı.");
            return;
        }

        int index = -1;
        for (int i = 0; i < sepet.Items.Count; i++)
        {
            if (sepet.Items[i].ProductId == id)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Console.WriteLine("Sepette bu id yok.");
            return;
        }

        Console.Write("Kaç adet silinsin? (Hepsi için 0 yaz): ");
        string adetText = Console.ReadLine();

        int adet;
        if (!int.TryParse(adetText, out adet))
        {
            Console.WriteLine("Sayı girmelisin.");
            return;
        }

        if (adet == 0)
        {
            Console.WriteLine("Silindi: " + sepet.Items[index].Name);
            sepet.Items.RemoveAt(index);
            SepetiKaydet();
            return;
        }

        if (adet < 0)
        {
            Console.WriteLine("Negatif olmaz.");
            return;
        }

        sepet.Items[index].Quantity -= adet;

        if (sepet.Items[index].Quantity <= 0)
        {
            Console.WriteLine("Ürün tamamen silindi: " + sepet.Items[index].Name);
            sepet.Items.RemoveAt(index);
        }
        else
        {
            Console.WriteLine("Güncellendi: " + sepet.Items[index].Name + " yeni adet: " + sepet.Items[index].Quantity);
        }

        SepetiKaydet();
    }

    static void SepetiTemizle()
    {
        sepet.Items.Clear();
        SepetiKaydet();
        Console.WriteLine("Sepet temizlendi.");
    }

    static void ToplamGoster()
    {
        Console.WriteLine("Genel Toplam: " + SepetToplam() + " TL");
    }

    // ---------------- MENÜ (KONSOL) ----------------
    static void SecenekGoster()
    {
        Console.WriteLine();
        Console.WriteLine("=== " + menu.RestaurantName + " ===");
        Console.WriteLine("1) Menüyü Göster");
        Console.WriteLine("2) Sepete Ürün Ekle (id ile)");
        Console.WriteLine("3) Sepeti Listele");
        Console.WriteLine("4) Sepetten Ürün Sil / Azalt");
        Console.WriteLine("5) Sepeti Temizle");
        Console.WriteLine("6) Toplam Tutarı Göster");
        Console.WriteLine("0) Çıkış");

        string secim = Console.ReadLine();

        if (secim == "1") MenuGoster();
        else if (secim == "2") SepeteEkle();
        else if (secim == "3") SepetiListele();
        else if (secim == "4") SepettenSilVeyaAzalt();
        else if (secim == "5") SepetiTemizle();
        else if (secim == "6") ToplamGoster();
        else if (secim == "0") return;
        else Console.WriteLine("Hatalı seçim.");

        SecenekGoster();
    }
}

// ------------ MODELLER (get/set ile, options yok) ------------

class Menu
{
    public string RestaurantName { get; set; }
    public List<Kategori> Categories { get; set; }
}

class Kategori
{
    public string CategoryName { get; set; }
    public List<Urun> Products { get; set; }
}

class Urun
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
}

class Sepet
{
    public List<SepetItem> Items { get; set; }
}

class SepetItem
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }
}