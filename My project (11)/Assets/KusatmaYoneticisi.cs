using UnityEngine;
using TMPro;
using System.Collections.Generic; // YENİ: Listeleri (Kuyruğu) kullanabilmek için şart!

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

    private KaynakNoktasi secilenNokta;

    // YENİ: Bekleyen raporları hafızada sıraya dizecek listeler
    private List<string> raporBasliklari = new List<string>();
    private List<string> raporMetinleri = new List<string>();

    void Awake() { if (instance == null) instance = this; }
    void Start() { EkraniGuncelle(); }

    public void GunuBitir()
    {
        turSayisi++;
        yemek -= 10;
        if (yemek < 0) yemek = 0;

        KaynakNoktasi[] haritadakiNoktalar = FindObjectsOfType<KaynakNoktasi>();
        foreach (KaynakNoktasi nokta in haritadakiNoktalar) { nokta.TurAtla(); }

        EkraniGuncelle();
    }

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
            if (secilenNokta.kaynakTipi == "Tas") hikayeMetni = "Düşman mancınıkları surlarımızı dövüyor. Onarım için acilen taşa ihtiyacımız var. Taş ocağı düşman hattına çok yakın, askerlerin neyle karşılaşacağı meçhul.";
            else if (secilenNokta.kaynakTipi == "Odun") hikayeMetni = "Ok ve yay yapımı, ayrıca yıkılan barikatların inşası için odun şart. Orman düşman izcileriyle kaynıyor, oraya girmek büyük risk.";
            else if (secilenNokta.kaynakTipi == "Yemek") hikayeMetni = "Kuşatma uzadıkça kilerlerimiz boşalıyor. Askerlerin savaşabilmesi için civardaki köylerde erzak aramalıyız. Aksi takdirde açlıktan kırılacağız.";

            gorevDetayText.text = "Gereken Asker: " + secilenNokta.gerekenAdam + "\n" +
                                  "Görev Süresi: " + secilenNokta.gorevSuresi + " Tur\n\n" +
                                  "<i>" + hikayeMetni + "</i>";
            gonderButonu.SetActive(true);
        }
    }

    public void PaneliKapat() { gorevPaneli.SetActive(false); }

    // YENİ: Tamam butonuna bastığımızda direkt kapatmak yerine sıradaki raporu çağıracak
    public void SonucPaneliniKapat()
    {
        SiradakiRaporuGoster();
    }

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

            if (kaynakTipi == "Tas")
            {
                geciciBaslik = "⛏️ Taş Ocağı Felaketi";
                hikayeSonucu = "<color=red>KÖTÜ HABER:</color> Taş ocağına giden birliğimiz düşman devriyeleri tarafından pusuya düşürüldü. Kimse sağ kurtulamadı. Göreve giden " + donenSayisi + " askerimizi kaybettik...";
            }
            else if (kaynakTipi == "Odun")
            {
                geciciBaslik = "🌲 Orman Felaketi";
                hikayeSonucu = "<color=red>KÖTÜ HABER:</color> Ormana gönderdiğimiz birlikten haber alınamıyor. Düşman okçularının onları avladığına dair söylentiler var. " + donenSayisi + " askerimiz geri dönmedi...";
            }
            else if (kaynakTipi == "Yemek")
            {
                geciciBaslik = "🏚️ Köy Yağması Felaketi";
                hikayeSonucu = "<color=red>KÖTÜ HABER:</color> Terk edilmiş köy meğer düşman için bir tuzakmış. Birliğimizin etrafı sarılıp yok edildi. " + donenSayisi + " yiğit askerimizi kaybettik...";
            }
        }
        else
        {
            bostaAsker += donenSayisi;

            int anaKazanc = 0;
            int sans = Random.Range(1, 101);
            if (sans <= 75) anaKazanc = Random.Range(40, 60);
            else anaKazanc = Random.Range(60, 81);

            int bonusZar = Random.Range(1, 101);
            int bonusKazanc = Random.Range(15, 30);

            if (kaynakTipi == "Tas")
            {
                geciciBaslik = "⛏️ Taş Ocağı Raporu";
                tas += anaKazanc;
                hikayeSonucu = "Askerlerimiz düşman devriyelerini atlatıp sağ salim döndü. <color=green>+" + anaKazanc + " Taş</color> getirdiler.";

                if (bonusZar <= 30)
                {
                    yemek += bonusKazanc;
                    hikayeSonucu += "\n\nEk olarak bir düşman kervanını yağmaladılar! <color=green>+" + bonusKazanc + " Erzak</color>.";
                }
                else if (bonusZar > 30 && bonusZar <= 60)
                {
                    odun += bonusKazanc;
                    hikayeSonucu += "\n\nYol üzerindeki barikatları söküp getirdiler! <color=green>+" + bonusKazanc + " Odun</color>.";
                }
            }
            else if (kaynakTipi == "Odun")
            {
                geciciBaslik = "🌲 Orman Keşfi Raporu";
                odun += anaKazanc;
                hikayeSonucu = "Ormanın derinliklerinden sağlam ağaçlar kesmeyi başardılar. Savunmamız için <color=green>+" + anaKazanc + " Odun</color> kazandık.";

                if (bonusZar <= 30)
                {
                    yemek += bonusKazanc;
                    hikayeSonucu += "\n\nOrmanda düşmanın erzak zulasını buldular! <color=green>+" + bonusKazanc + " Erzak</color>.";
                }
                else if (bonusZar > 30 && bonusZar <= 60)
                {
                    tas += bonusKazanc;
                    hikayeSonucu += "\n\nOrmanın içindeki eski bir tapınak kalıntısını söküp getirdiler! <color=green>+" + bonusKazanc + " Taş</color>.";
                }
            }
            else if (kaynakTipi == "Yemek")
            {
                geciciBaslik = "🏚️ Köy Yağması Raporu";
                yemek += anaKazanc;
                hikayeSonucu = "Köydeki gizli mahzenleri bulmayı başardılar. Kuşatmaya dayanmamız için <color=green>+" + anaKazanc + " Erzak</color> getirdiler.";

                if (bonusZar <= 30)
                {
                    odun += bonusKazanc;
                    hikayeSonucu += "\n\nKöydeki sağlam kalan çatı kirişlerini söküp kampa taşıdılar! <color=green>+" + bonusKazanc + " Odun</color>.";
                }
                else if (bonusZar > 30 && bonusZar <= 60)
                {
                    tas += bonusKazanc;
                    hikayeSonucu += "\n\nKöyün meydanındaki kuyu taşlarını parçalayıp surlar için getirdiler! <color=green>+" + bonusKazanc + " Taş</color>.";
                }
            }
        }

        // YENİ: Oluşan raporu direkt ekrana basmak yerine sıraya (Listeye) ekliyoruz
        raporBasliklari.Add(geciciBaslik);
        raporMetinleri.Add(hikayeSonucu);

        // YENİ: Eğer ekranda o an açık bir rapor yoksa, ilk raporu başlat
        if (sonucPaneli.activeSelf == false)
        {
            SiradakiRaporuGoster();
        }

        EkraniGuncelle();
    }

    // YENİ: Listede bekleyen raporları sırayla ekrana getiren ve bittiğinde paneli kapatan fonksiyon
    public void SiradakiRaporuGoster()
    {
        // Eğer listede bekleyen rapor varsa
        if (raporBasliklari.Count > 0)
        {
            // Listenin ilk elemanlarını ekrana yazdır
            sonucBaslikText.text = raporBasliklari[0];
            sonucDetayText.text = raporMetinleri[0];

            // Ekrana yazdırdığımız raporu listeden SİL Kİ bir sonrakine sıra gelsin
            raporBasliklari.RemoveAt(0);
            raporMetinleri.RemoveAt(0);

            sonucPaneli.SetActive(true);
        }
        else
        {
            // Listede okunacak rapor kalmadıysa paneli tamamen kapat
            sonucPaneli.SetActive(false);
        }
    }

    public void EkraniGuncelle()
    {
        if (kaynakTexti != null) kaynakTexti.text = "📅 Gün: " + turSayisi + "   |   ⚔️ Boşta Asker: " + bostaAsker + "/" + toplamAsker + "   |   🍞 Erzak: " + yemek + "   |   🪵 Odun: " + odun + "   |   🧱 Taş: " + tas;
    }
}