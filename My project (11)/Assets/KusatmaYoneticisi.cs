using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class KusatmaYoneticisi : MonoBehaviour
{
    public static KusatmaYoneticisi instance;

    [Header("Oyun İstatistikleri")]
    public int turSayisi = 1;
    public int toplamAsker = 50;
    public int bostaAsker = 50;
    public int yemek = 100;
    public int odun = 50;
    public int tas = 20;
    public int altin = 20;

    [Header("Arayüz (UI) Bağlantıları")]
    public TextMeshProUGUI kaynakTexti;

    [Header("Görev Paneli UI")]
    public GameObject gorevPaneli;
    public TextMeshProUGUI gorevBaslikText;
    public TextMeshProUGUI gorevDetayText;
    public GameObject gonderButonu;

    [Header("Sonuç Paneli UI")]
    public GameObject sonucPaneli;
    public TextMeshProUGUI sonucBaslikText;
    public TextMeshProUGUI sonucDetayText;

    [Header("Kaçakçı Sistemi UI")]
    public GameObject kacakciGemisiObje;
    public GameObject kacakciPaneli;
    public TextMeshProUGUI kacakciTeklifText;

    private KaynakNoktasi secilenNokta;
    private List<string> raporBasliklari = new List<string>();
    private List<string> raporMetinleri = new List<string>();

    // Kaçakçı arka plan değişkenleri
    private string teklifEdilenKaynak;
    private int teklifEdilenMiktar;
    private int istenenAltin;

    void Awake() { if (instance == null) instance = this; }
    void Start() { EkraniGuncelle(); }

    public void GunuBitir()
    {
        turSayisi++;
        yemek -= 10;
        if (yemek < 0) yemek = 0;

        KaynakNoktasi[] haritadakiNoktalar = FindObjectsOfType<KaynakNoktasi>();
        foreach (KaynakNoktasi nokta in haritadakiNoktalar) { nokta.TurAtla(); }

        // --- KAÇAKÇI YENİLEME ---
        if (kacakciGemisiObje != null) kacakciGemisiObje.SetActive(false);
        if (kacakciPaneli != null) kacakciPaneli.SetActive(false);

        // %30 ihtimalle denizde kaçakçı gemisi belirir
        if (Random.Range(1, 101) <= 30)
        {
            YeniKacakciTeklifiOlustur();
            if (kacakciGemisiObje != null) kacakciGemisiObje.SetActive(true);
        }

        EkraniGuncelle();
    }

    // --- KAÇAKÇI SİSTEMİ FONKSİYONLARI ---
    public void YeniKacakciTeklifiOlustur()
    {
        string[] kaynaklar = { "Tas", "Odun", "Yemek" };
        teklifEdilenKaynak = kaynaklar[Random.Range(0, 3)];

        // Kaçakçılar Altın karşılığı kaynak verir
        teklifEdilenMiktar = Random.Range(30, 70); // Vereceği kaynak miktarı
        istenenAltin = Random.Range(5, 15);        // İsteyeceği altın miktarı
    }

    public void KacakciPaneliniAc()
    {
        kacakciPaneli.SetActive(true);
        kacakciTeklifText.text = "Gizemli tüccarlar fısıldayarak konuşuyor:\n\n" +
                          "\"Sana <color=green>+" + teklifEdilenMiktar + " " + teklifEdilenKaynak + "</color> getirdik.\n" +
                          "Karşılığında sadece <color=#FFD700>-" + istenenAltin + " Altın</color> istiyoruz.\n" +
                          "Anlaşalım mı?\"";
    }

    public void TakasiKabulEt()
    {
        if (altin >= istenenAltin)
        {
            // Altını kes
            altin -= istenenAltin;

            // Kaynağı ver
            if (teklifEdilenKaynak == "Tas") tas += teklifEdilenMiktar;
            else if (teklifEdilenKaynak == "Odun") odun += teklifEdilenMiktar;
            else if (teklifEdilenKaynak == "Yemek") yemek += teklifEdilenMiktar;

            kacakciPaneli.SetActive(false);
            kacakciGemisiObje.SetActive(false); // Gemiyi gönder
            EkraniGuncelle();
        }
        else
        {
            kacakciTeklifText.text = "<color=red>Bizimle dalga mı geçiyorsun? Yeterli altının yok!</color>";
        }
    }

    public void TakasiReddet()
    {
        kacakciPaneli.SetActive(false);
        kacakciGemisiObje.SetActive(false); // Gemi gider
    }

    // --- GÖREV MENÜSÜ FONKSİYONLARI ---
    public void GorevPaneliAc(KaynakNoktasi tiklananNokta)
    {
        secilenNokta = tiklananNokta;
        gorevPaneli.SetActive(true);

        if (secilenNokta.kaynakTipi == "Tas") gorevBaslikText.text = "⛏️ Taş Ocağı Keşfi";
        else if (secilenNokta.kaynakTipi == "Odun") gorevBaslikText.text = "🌲 Yakın Orman";
        else if (secilenNokta.kaynakTipi == "Yemek") gorevBaslikText.text = "🏚️ Terk Edilmiş Köy";

        if (secilenNokta.islemde == true)
        {
            gorevDetayText.text = "Şu an bu bölgede askerlerimiz operasyon yürütüyor.\n\n⏳ Dönüşlerine Kalan Tur: " + secilenNokta.kalanTur;
            gonderButonu.SetActive(false);
        }
        else
        {
            string hikayeMetni = "";
            if (secilenNokta.kaynakTipi == "Tas") hikayeMetni = "Düşman mancınıkları surlarımızı dövüyor. Onarım için acilen taşa ihtiyacımız var. Taş ocağı düşman hattına çok yakın.";
            else if (secilenNokta.kaynakTipi == "Odun") hikayeMetni = "Ok ve yay yapımı, ayrıca yıkılan barikatların inşası için odun şart. Orman düşman izcileriyle kaynıyor.";
            else if (secilenNokta.kaynakTipi == "Yemek") hikayeMetni = "Kuşatma uzadıkça kilerlerimiz boşalıyor. Askerlerin savaşabilmesi için civardaki köylerde erzak aramalıyız.";

            gorevDetayText.text = "Gereken Asker: " + secilenNokta.gerekenAdam + "\n" +
                                  "Görev Süresi: " + secilenNokta.gorevSuresi + " Tur\n\n" +
                                  "<i>" + hikayeMetni + "</i>";
            gonderButonu.SetActive(true);
        }
    }

    public void PaneliKapat() { gorevPaneli.SetActive(false); }
    public void SonucPaneliniKapat() { SiradakiRaporuGoster(); }

    public void GoreveOnayVer()
    {
        if (secilenNokta != null && secilenNokta.islemde == false)
        {
            if (bostaAsker >= secilenNokta.gerekenAdam)
            {
                bostaAsker -= secilenNokta.gerekenAdam;
                secilenNokta.GoreviBaslat();
                EkraniGuncelle();
                PaneliKapat();
            }
            else gorevDetayText.text = "<color=red>Bu görev için yeterli boşta askerin yok!</color>";
        }
    }

    public void KesiftenDon(int donenSayisi, string kaynakTipi)
    {
        int pusuZari = Random.Range(1, 101);
        string geciciBaslik = "";
        string hikayeSonucu = "";

        if (pusuZari <= 15)
        {
            toplamAsker -= donenSayisi;
            if (kaynakTipi == "Tas") { geciciBaslik = "⛏️ Taş Ocağı Felaketi"; hikayeSonucu = "<color=red>KÖTÜ HABER:</color> Birlik pusuya düştü. " + donenSayisi + " askerimizi kaybettik..."; }
            else if (kaynakTipi == "Odun") { geciciBaslik = "🌲 Orman Felaketi"; hikayeSonucu = "<color=red>KÖTÜ HABER:</color> Birlikten haber yok. " + donenSayisi + " askerimiz geri dönmedi..."; }
            else if (kaynakTipi == "Yemek") { geciciBaslik = "🏚️ Köy Yağması Felaketi"; hikayeSonucu = "<color=red>KÖTÜ HABER:</color> Köy bir tuzaktı. " + donenSayisi + " yiğit askerimizi kaybettik..."; }
        }
        else
        {
            bostaAsker += donenSayisi;
            int anaKazanc = Random.Range(1, 101) <= 75 ? Random.Range(40, 60) : Random.Range(60, 81);
            int bonusZar = Random.Range(1, 101);
            int bonusKazanc = Random.Range(15, 30);

            if (kaynakTipi == "Tas")
            {
                geciciBaslik = "⛏️ Taş Ocağı Raporu";
                tas += anaKazanc;
                hikayeSonucu = "Sağ salim döndüler. <color=green>+" + anaKazanc + " Taş</color> getirdiler.";
                if (bonusZar <= 30) { yemek += bonusKazanc; hikayeSonucu += "\n\nDüşman kervanı yağmaladılar! <color=green>+" + bonusKazanc + " Erzak</color>."; }
                else if (bonusZar > 30 && bonusZar <= 60) { odun += bonusKazanc; hikayeSonucu += "\n\nBarikatları söküp getirdiler! <color=green>+" + bonusKazanc + " Odun</color>."; }
            }
            else if (kaynakTipi == "Odun")
            {
                geciciBaslik = "🌲 Orman Keşfi Raporu";
                odun += anaKazanc;
                hikayeSonucu = "Sağlam ağaçlar kestiler. <color=green>+" + anaKazanc + " Odun</color> kazandık.";
                if (bonusZar <= 30) { yemek += bonusKazanc; hikayeSonucu += "\n\nErzak zulası buldular! <color=green>+" + bonusKazanc + " Erzak</color>."; }
                else if (bonusZar > 30 && bonusZar <= 60) { tas += bonusKazanc; hikayeSonucu += "\n\nEski tapınağı söküp getirdiler! <color=green>+" + bonusKazanc + " Taş</color>."; }
            }
            else if (kaynakTipi == "Yemek")
            {
                geciciBaslik = "🏚️ Köy Yağması Raporu";
                yemek += anaKazanc;
                hikayeSonucu = "Gizli mahzenleri buldular. <color=green>+" + anaKazanc + " Erzak</color> getirdiler.";
                if (bonusZar <= 30) { odun += bonusKazanc; hikayeSonucu += "\n\nÇatı kirişlerini söküp taşıdılar! <color=green>+" + bonusKazanc + " Odun</color>."; }
                else if (bonusZar > 30 && bonusZar <= 60) { tas += bonusKazanc; hikayeSonucu += "\n\nKuyu taşlarını parçalayıp getirdiler! <color=green>+" + bonusKazanc + " Taş</color>."; }
            }

            // ALTIN (JACKPOT) ŞANSI
            if (Random.Range(1, 101) <= 20)
            {
                int bulunanAltin = Random.Range(5, 16);
                altin += bulunanAltin;
                hikayeSonucu += "\n\n<color=#FFD700>ŞANS YÜZÜMÜZE GÜLDÜ! Yıkıntıların arasında bir kese buldular. +" + bulunanAltin + " Altın!</color>";
            }
        }

        raporBasliklari.Add(geciciBaslik);
        raporMetinleri.Add(hikayeSonucu);

        if (sonucPaneli.activeSelf == false) SiradakiRaporuGoster();
        EkraniGuncelle();
    }

    public void SiradakiRaporuGoster()
    {
        if (raporBasliklari.Count > 0)
        {
            sonucBaslikText.text = raporBasliklari[0];
            sonucDetayText.text = raporMetinleri[0];
            raporBasliklari.RemoveAt(0);
            raporMetinleri.RemoveAt(0);
            sonucPaneli.SetActive(true);
        }
        else sonucPaneli.SetActive(false);
    }

    public void EkraniGuncelle()
    {
        if (kaynakTexti != null) kaynakTexti.text = "📅 Gün: " + turSayisi + "   |   ⚔️ Boşta Asker: " + bostaAsker + "/" + toplamAsker + "   |   🍞 Erzak: " + yemek + "   |   🪵 Odun: " + odun + "   |   🧱 Taş: " + tas + "   |   💰 Altın: " + altin;
    }
}