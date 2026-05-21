using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class KusatmaYoneticisi : MonoBehaviour
{
    public static KusatmaYoneticisi instance;

    [Header("Oyun İstatistikleri")]
    public int turSayisi = 1;
    public int moral = 100;
    public int toplamAsker = 5;
    public int bostaAsker = 5;
    public int yaraliAsker = 0;

    public int yemek = 50;
    public int odun = 50;
    public int tas = 20;
    public int altin = 20;

    [Header("Gelişmiş Mekanikler")]
    public int komutan = 0;
    public int mancinikSaboteTuru = 0;
    public bool karantinaYapildi = false;
    public int bekleyenKararSayisi = 0;
    private int aktifKararID = 0;

    // YENİ: Oyunun son karşılaştığı olayları aklında tutacağı liste
    private List<int> sonYasananKararlar = new List<int>();

    [Header("Atölye Geliştirmeleri")]
    public bool elArabasiAlindi = false;
    public bool surGuclendirildi = false;
    public bool mahzenYapildi = false;

    [Header("Arayüz (UI) Bağlantıları")]
    public TextMeshProUGUI kaynakTexti;

    [Header("UI Panelleri")]
    public GameObject anaMenuPaneli;
    public GameObject komutanPaneli;
    public GameObject gorevPaneli; public TextMeshProUGUI gorevBaslikText; public TextMeshProUGUI gorevDetayText; public GameObject gonderButonu;
    public GameObject sonucPaneli; public TextMeshProUGUI sonucBaslikText; public TextMeshProUGUI sonucDetayText;
    public GameObject kacakciGemisiObje; public GameObject kacakciPaneli; public TextMeshProUGUI kacakciTeklifText;
    public GameObject olayPaneli; public TextMeshProUGUI olayText; public GameObject onarButonu;
    public TextMeshProUGUI onarButonText; public TextMeshProUGUI riskButonText;
    public GameObject tayinPaneli;
    public GameObject atolyePaneli; public GameObject arabaButonu; public GameObject surButonu; public GameObject mahzenButonu;
    public GameObject karantinaButonu;

    private KaynakNoktasi secilenNokta;
    private List<string> raporBasliklari = new List<string>();
    private List<string> raporMetinleri = new List<string>();

    private string teklifEdilenKaynak; private int teklifEdilenMiktar; private int istenenAltin;
    private string aktifOlayTipi; private int gerekenKaynakMiktari;
    private bool oyunBittiMi = false;

    void Awake() { if (instance == null) instance = this; }

    void Start()
    {
        EkraniGuncelle();
        if (komutanPaneli != null && anaMenuPaneli == null) komutanPaneli.SetActive(true);
    }

    public void OyunaBasla()
    {
        if (anaMenuPaneli != null) anaMenuPaneli.SetActive(false);
        if (komutanPaneli != null) komutanPaneli.SetActive(true);
    }

    public void KomutanSec(int secim)
    {
        komutan = secim;
        if (komutanPaneli != null) komutanPaneli.SetActive(false);
        EkraniGuncelle();
    }

    public void OyunuYenidenBaslat() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void OyundanCik() { Application.Quit(); }

    public void AtolyePaneliniAc()
    {
        if ((anaMenuPaneli != null && anaMenuPaneli.activeSelf) || (komutanPaneli != null && komutanPaneli.activeSelf) || gorevPaneli.activeSelf || sonucPaneli.activeSelf || kacakciPaneli.activeSelf || olayPaneli.activeSelf || tayinPaneli.activeSelf) return;
        atolyePaneli.SetActive(true);
    }
    public void AtolyePaneliniKapat() { atolyePaneli.SetActive(false); }

    public void ElArabasiSatinAl() { int bedel = (komutan == 1) ? 75 : 100; if (!elArabasiAlindi && odun >= bedel) { odun -= bedel; elArabasiAlindi = true; arabaButonu.SetActive(false); EkraniGuncelle(); } }
    public void SurGuclendir() { int bedel = (komutan == 1) ? 75 : 100; if (!surGuclendirildi && tas >= bedel) { tas -= bedel; surGuclendirildi = true; surButonu.SetActive(false); EkraniGuncelle(); } }
    public void MahzenInsaEt() { int bedel = (komutan == 1) ? 60 : 80; if (!mahzenYapildi && tas >= bedel && odun >= bedel) { tas -= bedel; odun -= bedel; mahzenYapildi = true; mahzenButonu.SetActive(false); EkraniGuncelle(); } }
    public void KarantinaInsaEt() { int bedel = (komutan == 1) ? 35 : 50; if (!karantinaYapildi && tas >= bedel && odun >= bedel) { tas -= bedel; odun -= bedel; karantinaYapildi = true; karantinaButonu.SetActive(false); EkraniGuncelle(); } }

    public void GunuBitir()
    {
        if (oyunBittiMi) return;
        if (anaMenuPaneli != null && anaMenuPaneli.activeSelf) return;
        if (komutanPaneli != null && komutanPaneli.activeSelf) return;
        if (gorevPaneli.activeSelf || sonucPaneli.activeSelf || kacakciPaneli.activeSelf || olayPaneli.activeSelf || atolyePaneli.activeSelf) return;
        tayinPaneli.SetActive(true);
    }

    public void TayinSec(int secim)
    {
        tayinPaneli.SetActive(false); turSayisi++;

        bekleyenKararSayisi = Random.Range(1, 3);

        if (secim == 0) { yemek -= 5; moral -= 10; }
        else if (secim == 1) { yemek -= 10; }
        else if (secim == 2) { yemek -= 20; moral += 10; }
        else if (secim == 3) { moral -= 30; }

        if (komutan == 2 && yaraliAsker > 0) { yaraliAsker--; bostaAsker++; raporBasliklari.Add("✨ Şifacının Lütfu"); raporMetinleri.Add("Şifacı komutanımız 1 yaralı askerimizi tamamen iyileştirdi."); }

        if (yaraliAsker > 0)
        {
            int iyilesen = 0; int olenYarali = 0;
            if (secim == 2) iyilesen = Mathf.Min(yaraliAsker, 2); else if (secim == 1) iyilesen = Mathf.Min(yaraliAsker, 1); else if (secim == 3) olenYarali = Mathf.Min(yaraliAsker, 1);
            if (iyilesen > 0) { yaraliAsker -= iyilesen; bostaAsker += iyilesen; raporBasliklari.Add("❤️ Hastane"); raporMetinleri.Add("<color=green>" + iyilesen + " yaralı</color> iyileşti."); }
            if (olenYarali > 0) { yaraliAsker -= olenYarali; toplamAsker -= olenYarali; raporBasliklari.Add("☠️ Hastane"); raporMetinleri.Add("<color=red>" + olenYarali + " yaralı</color> can verdi..."); }
        }

        if (yaraliAsker >= 3 && !karantinaYapildi && bostaAsker > 0)
        {
            if (Random.Range(1, 101) <= 40) { bostaAsker--; yaraliAsker++; raporBasliklari.Add("🦠 SALGIN YAYILIYOR!"); raporMetinleri.Add("Karantina Çadırımız yok! <color=red>1 sağlıklı asker daha yatağa düştü.</color>"); }
        }

        if (mancinikSaboteTuru > 0) mancinikSaboteTuru--;
        if (yemek < 0) { yemek = 0; moral -= 20; raporBasliklari.Add("⚠️ AÇLIK BAŞLADI!"); raporMetinleri.Add("Kalede yiyecek kalmadı! İsyan sesleri yükseliyor."); }
        if (moral > 100) moral = 100;

        if (moral <= 0) { OyunSonu(false, "Moral sıfıra indi! Askerler isyan çıkardı. KAYBETTİNİZ..."); return; }
        if (toplamAsker <= 0) { OyunSonu(false, "Surları savunacak asker kalmadı. KAYBETTİNİZ..."); return; }
        if (turSayisi == 50) { EkraniGuncelle(); FinalKusatmasiniTetikle(); return; }

        if (turSayisi % 4 == 0 && turSayisi != 50) { int gelenAsker = Random.Range(2, 4); toplamAsker += gelenAsker; bostaAsker += gelenAsker; raporBasliklari.Add("🎺 Gönüllüler"); raporMetinleri.Add("<color=green>+" + gelenAsker + " savaşçı</color> saflarımıza katıldı!"); }

        KaynakNoktasi[] haritadakiNoktalar = FindObjectsByType<KaynakNoktasi>(FindObjectsSortMode.None);
        foreach (KaynakNoktasi nokta in haritadakiNoktalar) { nokta.TurAtla(); }

        if (kacakciGemisiObje != null) kacakciGemisiObje.SetActive(false); if (kacakciPaneli != null) kacakciPaneli.SetActive(false);
        if (Random.Range(1, 101) <= 30) { YeniKacakciTeklifiOlustur(); if (kacakciGemisiObje != null) kacakciGemisiObje.SetActive(true); }

        EkraniGuncelle();
        RastgeleOlayTetikle();
    }

    public void OyunSonu(bool kazandiMi, string mesaj)
    {
        oyunBittiMi = true; sonucPaneli.SetActive(true);
        sonucBaslikText.text = kazandiMi ? "👑 ZAFER!" : "☠️ MAĞLUBİYET"; sonucDetayText.text = mesaj;
        if (!kazandiMi && KameraTitreme.instance != null) KameraTitreme.instance.Titret(1.5f, 0.4f);
    }

    public void FinalKusatmasiniTetikle()
    {
        aktifOlayTipi = "Final"; olayPaneli.SetActive(true);
        olayText.text = "⚔️ <color=red>SON HÜCUM!</color>\n\nUfukta Kraliyet Ordusu göründü! Ancak düşman bunu fark etti ve gitmeden önce kaleyi yıkmak için TÜM GÜCÜYLE saldırıyor!\n\nGereken: <color=orange>80 Taş ve 80 Odun</color>\n\nEğer yeterli malzemen yoksa, askerlerine <color=red>ÖLÜMÜNE SAVAŞ</color> emri vermek zorundasın!";
        onarButonu.SetActive(tas >= 80 && odun >= 80);
        if (onarButonText != null) onarButonText.text = "Surları Kur\n(-80 Taş/Odun)"; if (riskButonText != null) riskButonText.text = "Ölümüne Savaş\n(15 Asker Feda Et)";
        if (KameraTitreme.instance != null) KameraTitreme.instance.Titret(1f, 0.3f);
    }

    public void GunlukKararGoster()
    {
        olayPaneli.SetActive(true);
        aktifOlayTipi = "Karar";

        // YENİ: Hafızalı (Cooldown) Zar Atma Sistemi
        int guvenlikSayaci = 0;
        do
        {
            aktifKararID = Random.Range(1, 16);
            guvenlikSayaci++;
            // Sonsuz döngüye girmemesi için önlem
            if (guvenlikSayaci > 50) break;
        } while (sonYasananKararlar.Contains(aktifKararID));

        // Çıkan kararı hafızaya al
        sonYasananKararlar.Add(aktifKararID);
        // Eğer hafızadaki karar sayısı 5'i geçtiyse, en eskisini unut
        if (sonYasananKararlar.Count > 5)
        {
            sonYasananKararlar.RemoveAt(0);
        }

        switch (aktifKararID)
        {
            case 1:
                olayText.text = "⛺ <color=yellow>HALKIN TALEBİ</color>\n\nBarınakların çatısı akıtıyor. İnsanlar hasta olacak. Onarmak için odun harcayalım mı?"; onarButonu.SetActive(odun >= 15);
                if (onarButonText != null) onarButonText.text = "Onar (-15 Odun)"; if (riskButonText != null) riskButonText.text = "Umursama (-10 Moral)"; break;
            case 2:
                olayText.text = "💰 <color=yellow>GİZEMLİ TÜCCAR</color>\n\nSurlara yaklaşan bir tüccar, biraz erzak karşılığında bize yapı taşı verebileceğini söylüyor."; onarButonu.SetActive(yemek >= 15);
                if (onarButonText != null) onarButonText.text = "Takas Et (-15 Yemek, +20 Taş)"; if (riskButonText != null) riskButonText.text = "Kov Gitsin (Bir şey olmaz)"; break;
            case 3:
                olayText.text = "🎲 <color=yellow>ASABİ ASKERLER</color>\n\nAskerler gece kumarda altınlarını kaybetmiş, çok gerginler. Onlara altın dağıtıp morallerini düzeltelim mi?"; onarButonu.SetActive(altin >= 10);
                if (onarButonText != null) onarButonText.text = "Altın Dağıt (-10 Altın, +10 Moral)"; if (riskButonText != null) riskButonText.text = "Disiplin Cezası (-15 Moral)"; break;
            case 4:
                olayText.text = "🛒 <color=yellow>SAHİPSİZ KERVAN</color>\n\nSurların uzağında sahipsiz bir kervan bulduk. Çok erzak var ama bir tuzak da olabilir. Yağmalayalım mı?"; onarButonu.SetActive(true);
                if (onarButonText != null) onarButonText.text = "Yağmala (+20 Yemek, -10 Moral)"; if (riskButonText != null) riskButonText.text = "Bulaşma (+5 Moral)"; break;
            case 5:
                olayText.text = "⛪ <color=yellow>RAHİPLERİN İSTEĞİ</color>\n\nKaledeki rahipler dua etmek için küçük bir sunak inşa etmek istiyor. Bizden taş talep ediyorlar."; onarButonu.SetActive(tas >= 15);
                if (onarButonText != null) onarButonText.text = "İzin Ver (-15 Taş, +15 Moral)"; if (riskButonText != null) riskButonText.text = "Reddet (-15 Moral)"; break;
            case 6:
                olayText.text = "🎵 <color=yellow>GEZGİN OZAN</color>\n\nKampa neşeli bir ozan geldi. Ona biraz altın verirsek gece boyunca şarkı söyleyip herkese moral verecek."; onarButonu.SetActive(altin >= 5);
                if (onarButonText != null) onarButonText.text = "Altın Ver (-5 Altın, +20 Moral)"; if (riskButonText != null) riskButonText.text = "Kapıdan Çevir (-5 Moral)"; break;
            case 7:
                olayText.text = "🥩 <color=yellow>ÇÜRÜK ERZAKLAR</color>\n\nAşçı, erzak deposunun bir kısmının küflendiğini söylüyor. Çürük kısımları çöpe mi atalım, yoksa risk alıp askerlere mi yedirelim?"; onarButonu.SetActive(yemek >= 15);
                if (onarButonText != null) onarButonText.text = "Çöpe At (-15 Yemek)"; if (riskButonText != null) riskButonText.text = "Yedir (Hastalanma Riski!)"; break;
            case 8:
                olayText.text = "🏃‍♂️ <color=yellow>FİRARİ ASKER</color>\n\nGece karanlığında surlardan atlayıp kaçmaya çalışan bir asker yakaladık. Asalım mı, yoksa hapse mi atalım?"; onarButonu.SetActive(bostaAsker >= 1);
                if (onarButonText != null) onarButonText.text = "İdam Et (-1 Asker, -10 Moral)"; if (riskButonText != null) riskButonText.text = "Hapse At (-20 Yemek)"; break;
            case 9:
                olayText.text = "👑 <color=yellow>GİZLİ ZULA</color>\n\nKazı yapan işçiler eski bir altın zulası buldu. Bu altınlara ordu adına el mi koyalım, yoksa işçilere mi bırakalım?"; onarButonu.SetActive(true);
                if (onarButonText != null) onarButonText.text = "El Koy (+20 Altın, -15 Moral)"; if (riskButonText != null) riskButonText.text = "Halka Bırak (+15 Moral)"; break;
            case 10:
                olayText.text = "🐺 <color=yellow>YABANİ KÖPEKLER</color>\n\nAç kalmış köpekler kampa girdi. Askerler onları vurup yemek istiyor. İzin verelim mi, yoksa barınak mı yapalım?"; onarButonu.SetActive(true);
                if (onarButonText != null) onarButonText.text = "Avla ve Ye (+15 Yemek, -5 Moral)"; if (riskButonText != null) riskButonText.text = "Barınak Yap (-10 Odun, +10 Moral)"; break;
            case 11:
                olayText.text = "❄️ <color=yellow>DONDURUCU SOĞUK</color>\n\nBu gece hava aniden buz kesti. Nöbetçiler donmamak için devasa ateşler yakmak istiyor."; onarButonu.SetActive(odun >= 20);
                if (onarButonText != null) onarButonText.text = "İzin Ver (-20 Odun, +5 Moral)"; if (riskButonText != null) riskButonText.text = "Odunu Sakla (-15 Moral)"; break;
            case 12:
                olayText.text = "😷 <color=yellow>HASTALIKLI MÜLTECİ</color>\n\nSurlara yaklaşan hasta bir adam ilaç için altın yalvarıyor. Ona yardım edelim mi?"; onarButonu.SetActive(altin >= 10);
                if (onarButonText != null) onarButonText.text = "İlaç Al (-10 Altın, +10 Moral)"; if (riskButonText != null) riskButonText.text = "Ok Atıp Kov (-10 Moral)"; break;
            case 13:
                olayText.text = "🚧 <color=yellow>ÇÖKEN BARİKAT</color>\n\nSurlardaki zayıf bir nokta kendi kendine çöktü. Hemen onarmak çok malzeme ister ama onarmazsak halk paniğe kapılır."; onarButonu.SetActive(odun >= 20 && tas >= 10);
                if (onarButonText != null) onarButonText.text = "Acil Onar (-20 Odun, -10 Taş)"; if (riskButonText != null) riskButonText.text = "Beklet (-15 Moral)"; break;
            case 14:
                olayText.text = "🏅 <color=yellow>KAHRAMAN ASKER</color>\n\nNöbetçilerden biri tek başına sızmaya çalışan bir düşman casusunu hakladı. Onu altınla ödüllendirelim mi?"; onarButonu.SetActive(altin >= 15);
                if (onarButonText != null) onarButonText.text = "Ödüllendir (-15 Altın, +20 Moral)"; if (riskButonText != null) riskButonText.text = "Sadece Tebrik Et (-5 Moral)"; break;
            case 15:
                olayText.text = "📜 <color=yellow>DÜŞMAN ELÇİSİ</color>\n\nDüşmandan gizli bir elçi geldi. Ona rüşvet verirsek bize biraz gizli erzak getirebileceğini söylüyor."; onarButonu.SetActive(altin >= 20);
                if (onarButonText != null) onarButonText.text = "Rüşvet Ver (-20 Altın, +20 Yemek)"; if (riskButonText != null) riskButonText.text = "Kov Gitsin (+10 Moral)"; break;
        }
    }

    public void OlayiOnar()
    {
        if (aktifOlayTipi.StartsWith("Karar_") || aktifOlayTipi == "Karar")
        {
            switch (aktifKararID)
            {
                case 1: odun -= 15; moral += 10; break;
                case 2: yemek -= 15; tas += 20; break;
                case 3: altin -= 10; moral += 10; break;
                case 4: yemek += 20; moral -= 10; if (KameraTitreme.instance != null) KameraTitreme.instance.Titret(0.3f, 0.1f); break;
                case 5: tas -= 15; moral += 15; break;
                case 6: altin -= 5; moral += 20; break;
                case 7: yemek -= 15; break;
                case 8: bostaAsker--; toplamAsker--; moral -= 10; break;
                case 9: altin += 20; moral -= 15; break;
                case 10: yemek += 15; moral -= 5; break;
                case 11: odun -= 20; moral += 5; break;
                case 12: altin -= 10; moral += 10; break;
                case 13: odun -= 20; tas -= 10; break;
                case 14: altin -= 15; moral += 20; break;
                case 15: altin -= 20; yemek += 20; break;
            }
            KararSonrasiKontrol();
            return;
        }

        if (aktifOlayTipi == "Final") { tas -= 80; odun -= 80; olayPaneli.SetActive(false); OyunSonu(true, "Kusursuz Savunma! Güçlü surlarımız düşman dalgasını kırdı. Kraliyet ordusu yetişti ve kuşatma sona erdi. KAHRAMANSINIZ!"); return; }
        if (aktifOlayTipi == "Tas") tas -= gerekenKaynakMiktari;
        else if (aktifOlayTipi == "Odun") odun -= gerekenKaynakMiktari;
        else if (aktifOlayTipi == "Yemek") yemek -= gerekenKaynakMiktari;
        else if (aktifOlayTipi == "HayattaKalanlar") { yemek -= gerekenKaynakMiktari; toplamAsker += 3; bostaAsker += 3; raporBasliklari.Insert(0, "🫂 Yeni Yoldaşlar"); raporMetinleri.Insert(0, "Mültecileri kampa aldık! <color=green>+3 Asker</color>"); }

        EkraniGuncelle(); olayPaneli.SetActive(false); if (raporBasliklari.Count > 0) SiradakiRaporuGoster(); else if (bekleyenKararSayisi > 0) GunlukKararGoster();
    }

    public void OlaydaRiskAl()
    {
        if (aktifOlayTipi.StartsWith("Karar_") || aktifOlayTipi == "Karar")
        {
            switch (aktifKararID)
            {
                case 1: moral -= 10; break;
                case 2: break;
                case 3: moral -= 15; break;
                case 4: moral += 5; break;
                case 5: moral -= 15; break;
                case 6: moral -= 5; break;
                case 7:
                    if (bostaAsker > 0) { bostaAsker--; yaraliAsker++; moral -= 10; }
                    break;
                case 8: yemek -= 20; break;
                case 9: moral += 15; break;
                case 10: odun -= 10; moral += 10; break;
                case 11: moral -= 15; break;
                case 12: moral -= 10; break;
                case 13: moral -= 15; break;
                case 14: moral -= 5; break;
                case 15: moral += 10; break;
            }
            KararSonrasiKontrol();
            return;
        }

        olayPaneli.SetActive(false);
        if (aktifOlayTipi == "Final") { if (bostaAsker >= 15) { OyunSonu(true, "Kanlı Zafer! 15 yiğit kaybettik ama kaleyi savunduk!"); } else { OyunSonu(false, "Katliam... Düşman kaleyi ezip geçti."); } return; }
        if (aktifOlayTipi == "HayattaKalanlar") { moral -= 5; raporBasliklari.Insert(0, "🚪 Kapı Dışarı"); raporMetinleri.Insert(0, "Mültecileri almadık... Moral düştü."); EkraniGuncelle(); SiradakiRaporuGoster(); return; }

        int riskZari = Random.Range(1, 101); string baslik = "⚠️ Risk Sonucu"; string metin = "";
        if (riskZari <= 50) { metin = "Saldırıyı <color=green>hiç kayıp vermeden</color> atlattık!"; moral += 5; }
        else { int kayipAsker = Random.Range(2, 5); if (bostaAsker < kayipAsker) kayipAsker = bostaAsker; if (kayipAsker == 0) { metin = "Düşman ortalığı dağıttı!"; moral -= 15; } else { int olen = kayipAsker / 2; if (olen == 0) olen = 1; int yarali = kayipAsker - olen; bostaAsker -= kayipAsker; toplamAsker -= olen; yaraliAsker += yarali; moral -= 15; metin = "Risk felaketle sonuçlandı! <color=red>" + olen + " öldü, " + yarali + " yaralandı!</color>"; if (KameraTitreme.instance != null) KameraTitreme.instance.Titret(0.6f, 0.2f); } }
        raporBasliklari.Insert(0, baslik); raporMetinleri.Insert(0, metin); EkraniGuncelle(); SiradakiRaporuGoster();
    }

    private void KararSonrasiKontrol()
    {
        bekleyenKararSayisi--;
        EkraniGuncelle();
        if (moral <= 0) { OyunSonu(false, "Moral sıfıra indi! Askerler isyan çıkardı. KAYBETTİNİZ..."); return; }
        if (bekleyenKararSayisi > 0) GunlukKararGoster(); else olayPaneli.SetActive(false);
    }

    public void RastgeleOlayTetikle()
    {
        bool olayOlduMu = false;
        if (turSayisi % 10 == 0) olayOlduMu = true; else if (Random.Range(1, 101) <= 4) olayOlduMu = true;

        if (olayOlduMu == false) { if (sonucPaneli.activeSelf == false && raporBasliklari.Count > 0) SiradakiRaporuGoster(); else if (bekleyenKararSayisi > 0) GunlukKararGoster(); return; }

        int olayTipiZari = Random.Range(1, 101); olayPaneli.SetActive(true);
        if (olayTipiZari <= 35) { if (mancinikSaboteTuru > 0) { raporBasliklari.Insert(0, "🛡️ Sessiz Gece"); raporMetinleri.Insert(0, "Düşman mancınıklarını yaktığımız için bu gece güvendeyiz!"); olayPaneli.SetActive(false); SiradakiRaporuGoster(); return; } aktifOlayTipi = "Tas"; gerekenKaynakMiktari = surGuclendirildi ? 15 : 30; olayText.text = "🔥 <color=red>MANCINIK SALDIRISI!</color>\n\nDüşman surlarımızı dövüyor.\n\nGereken: <color=orange>" + gerekenKaynakMiktari + " Taş</color>"; onarButonu.SetActive(tas >= gerekenKaynakMiktari); if (onarButonText != null) onarButonText.text = "Surları Onar"; if (riskButonText != null) riskButonText.text = "Risk Al"; if (KameraTitreme.instance != null) KameraTitreme.instance.Titret(0.5f, 0.15f); }
        else if (olayTipiZari <= 70) { aktifOlayTipi = "Odun"; gerekenKaynakMiktari = 25; olayText.text = "🏹 <color=red>ATEŞLİ OK YAĞMURU!</color>\n\nAlevli oklar barikatları yaktı!\n\nGereken: <color=orange>25 Odun</color>"; onarButonu.SetActive(odun >= 25); if (onarButonText != null) onarButonText.text = "Barikat Kur"; if (riskButonText != null) riskButonText.text = "Risk Al"; }
        else if (olayTipiZari <= 85) { aktifOlayTipi = "Yemek"; gerekenKaynakMiktari = 30; if (mahzenYapildi) { raporBasliklari.Insert(0, "🛡️ Fareler Engellendi!"); raporMetinleri.Insert(0, "Fareler geldi ama erzaklarımız <color=green>Gizli Mahzen</color>'de olduğu için hiçbir şey yiyemediler!"); olayPaneli.SetActive(false); SiradakiRaporuGoster(); return; } olayText.text = "🐀 <color=red>FARE İSTİLASI!</color>\n\nFareler erzaklara dadandı.\n\nGereken: <color=orange>30 Erzak</color>"; onarButonu.SetActive(yemek >= 30); if (onarButonText != null) onarButonText.text = "Erzak Feda Et"; if (riskButonText != null) riskButonText.text = "Risk Al"; }
        else { aktifOlayTipi = "HayattaKalanlar"; gerekenKaynakMiktari = 10; olayText.text = "🫂 <color=green>MÜLTECİLER!</color>\n\nBize katılmak istiyorlar.\n\nGereken: <color=orange>10 Erzak</color>"; onarButonu.SetActive(yemek >= 10); if (onarButonText != null) onarButonText.text = "İçeri Al"; if (riskButonText != null) riskButonText.text = "Geri Çevir"; }
    }

    public void KesiftenDon(int donenSayisi, string kaynakTipi)
    {
        if (kaynakTipi == "Baskin") { int basariSansi = (komutan == 3) ? 80 : 50; if (Random.Range(1, 101) <= basariSansi) { bostaAsker += donenSayisi; mancinikSaboteTuru += 5; moral += 15; raporBasliklari.Add("🗡️ Kanlı ve Sessiz"); raporMetinleri.Add("Baskın ekibimiz düşman kampına sızdı ve <color=green>mancınıkları ateşe verdi!</color> Sonraki 5 gün mancınık saldırısı olmayacak!"); } else { toplamAsker -= donenSayisi; moral -= 20; raporBasliklari.Add("☠️ İntihar Görevi"); raporMetinleri.Add("Baskın başarısız oldu... Düşman onları fark etti. <color=red>Gönderilen " + donenSayisi + " yiğit askerimizin hepsi öldürüldü.</color>"); if (KameraTitreme.instance != null) KameraTitreme.instance.Titret(0.7f, 0.3f); } return; }
        int pusuZari = Random.Range(1, 101); if (komutan == 3) pusuZari = 100; string geciciBaslik = ""; string hikayeSonucu = "";
        if (pusuZari <= 15) { int olen = donenSayisi / 2; if (olen == 0) olen = 1; int yaralanan = donenSayisi - olen; toplamAsker -= olen; yaraliAsker += yaralanan; moral -= 10; geciciBaslik = "⚠️ Pusuya Düştük!"; hikayeSonucu = "<color=red>KÖTÜ HABER:</color> Birliğimiz pusuya düştü. <color=red>" + olen + " asker öldü</color>"; if (yaralanan > 0) hikayeSonucu += ", " + yaralanan + " asker ağır yaralı."; else hikayeSonucu += "."; if (KameraTitreme.instance != null) KameraTitreme.instance.Titret(0.5f, 0.2f); }
        else { bostaAsker += donenSayisi; if (kaynakTipi == "Gozcu") { geciciBaslik = "👁️ Keşif Raporu"; int bulunanAsker = Random.Range(1, 4); toplamAsker += bulunanAsker; bostaAsker += bulunanAsker; hikayeSonucu = "Gözcülerimiz harabelerde insanlar buldu. <color=green>+" + bulunanAsker + " Asker</color>."; } else { int anaKazanc = Random.Range(1, 101) <= 75 ? Random.Range(40, 60) : Random.Range(60, 81); if (elArabasiAlindi) anaKazanc = Mathf.RoundToInt(anaKazanc * 1.3f); int bonusZar = Random.Range(1, 101); int bonusKazanc = Random.Range(15, 30); if (kaynakTipi == "Tas") { geciciBaslik = "⛏️ Taş Raporu"; tas += anaKazanc; hikayeSonucu = "Döndüler. <color=green>+" + anaKazanc + " Taş</color>."; if (bonusZar <= 30) { yemek += bonusKazanc; hikayeSonucu += "\n\n<color=green>+" + bonusKazanc + " Erzak</color>."; } else if (bonusZar > 30 && bonusZar <= 60) { odun += bonusKazanc; hikayeSonucu += "\n\n<color=green>+" + bonusKazanc + " Odun</color>."; } } else if (kaynakTipi == "Odun") { geciciBaslik = "🌲 Orman Raporu"; odun += anaKazanc; hikayeSonucu = "Döndüler. <color=green>+" + anaKazanc + " Odun</color>."; if (bonusZar <= 30) { yemek += bonusKazanc; hikayeSonucu += "\n\n<color=green>+" + bonusKazanc + " Erzak</color>."; } else if (bonusZar > 30 && bonusZar <= 60) { tas += bonusKazanc; hikayeSonucu += "\n\n<color=green>+" + bonusKazanc + " Taş</color>."; } } else if (kaynakTipi == "Yemek") { geciciBaslik = "🏚️ Köy Raporu"; yemek += anaKazanc; hikayeSonucu = "Döndüler. <color=green>+" + anaKazanc + " Erzak</color>."; if (bonusZar <= 30) { odun += bonusKazanc; hikayeSonucu += "\n\n<color=green>+" + bonusKazanc + " Odun</color>."; } else if (bonusZar > 30 && bonusZar <= 60) { tas += bonusKazanc; hikayeSonucu += "\n\n<color=green>+" + bonusKazanc + " Taş</color>."; } } if (Random.Range(1, 101) <= 20) { int bulunanAltin = Random.Range(5, 16); altin += bulunanAltin; hikayeSonucu += "\n\n<color=#FFD700>ŞANS! +" + bulunanAltin + " Altın!</color>"; moral += 5; } } }
        raporBasliklari.Add(geciciBaslik); raporMetinleri.Add(hikayeSonucu);
    }
    public void AskerKaybet(int kayip) { toplamAsker -= kayip; bostaAsker -= kayip; if (bostaAsker < 0) bostaAsker = 0; if (toplamAsker < 0) toplamAsker = 0; }
    public void YeniKacakciTeklifiOlustur() { string[] kaynaklar = { "Tas", "Odun", "Yemek" }; teklifEdilenKaynak = kaynaklar[Random.Range(0, 3)]; teklifEdilenMiktar = Random.Range(30, 70); istenenAltin = Random.Range(5, 15); }
    public void KacakciPaneliniAc() { kacakciPaneli.SetActive(true); kacakciTeklifText.text = "\"Sana <color=green>+" + teklifEdilenMiktar + " " + teklifEdilenKaynak + "</color> getirdik.\nKarşılığında <color=#FFD700>-" + istenenAltin + " Altın</color> istiyoruz.\nAnlaşalım mı?\""; }
    public void TakasiKabulEt() { if (altin >= istenenAltin) { altin -= istenenAltin; if (teklifEdilenKaynak == "Tas") tas += teklifEdilenMiktar; else if (teklifEdilenKaynak == "Odun") odun += teklifEdilenMiktar; else if (teklifEdilenKaynak == "Yemek") yemek += teklifEdilenMiktar; kacakciPaneli.SetActive(false); kacakciGemisiObje.SetActive(false); EkraniGuncelle(); } else { kacakciTeklifText.text = "<color=red>Yeterli altının yok!</color>"; } }
    public void TakasiReddet() { kacakciPaneli.SetActive(false); kacakciGemisiObje.SetActive(false); }
    public void GorevPaneliAc(KaynakNoktasi tiklananNokta) { secilenNokta = tiklananNokta; gorevPaneli.SetActive(true); if (secilenNokta.kaynakTipi == "Tas") gorevBaslikText.text = "⛏️ Taş Ocağı"; else if (secilenNokta.kaynakTipi == "Odun") gorevBaslikText.text = "🌲 Yakın Orman"; else if (secilenNokta.kaynakTipi == "Yemek") gorevBaslikText.text = "🏚️ Köy"; else if (secilenNokta.kaynakTipi == "Gozcu") gorevBaslikText.text = "👁️ Keşif Kolu"; else if (secilenNokta.kaynakTipi == "Baskin") gorevBaslikText.text = "🗡️ Gece Baskını"; if (secilenNokta.islemde == true) { gorevDetayText.text = "Şu an bu bölgede askerlerimiz operasyon yürütüyor.\n\n⏳ Dönüşlerine Kalan Tur: " + secilenNokta.kalanTur; gonderButonu.SetActive(false); } else { string hikaye = ""; if (secilenNokta.kaynakTipi == "Gozcu") hikaye = "Harabelerde hayatta kalan başkaları olabilir. Etrafı araştırıp saflarımıza yeni yoldaşlar katmalıyız."; else if (secilenNokta.kaynakTipi == "Baskin") hikaye = "Düşman uyurken kampa sızıp mancınıkları ateşe vereceğiz. Son derece kanlı ve tehlikeli bir görev!"; else hikaye = "Askerleri gönderip kaynak toplamalıyız."; gorevDetayText.text = "Gereken Asker: " + secilenNokta.gerekenAdam + "\nGörev Süresi: " + secilenNokta.gorevSuresi + " Tur\n\n<i>" + hikaye + "</i>"; gonderButonu.SetActive(true); } }
    public void PaneliKapat() { gorevPaneli.SetActive(false); }

    public void SonucPaneliniKapat() { SiradakiRaporuGoster(); }
    public void SiradakiRaporuGoster()
    {
        if (oyunBittiMi) return;
        if (raporBasliklari.Count > 0)
        {
            sonucBaslikText.text = raporBasliklari[0]; sonucDetayText.text = raporMetinleri[0]; raporBasliklari.RemoveAt(0); raporMetinleri.RemoveAt(0); sonucPaneli.SetActive(true);
        }
        else
        {
            sonucPaneli.SetActive(false);
            if (bekleyenKararSayisi > 0) GunlukKararGoster();
        }
    }

    public void GoreveOnayVer() { if (secilenNokta != null && secilenNokta.islemde == false) { if (bostaAsker >= secilenNokta.gerekenAdam) { bostaAsker -= secilenNokta.gerekenAdam; secilenNokta.GoreviBaslat(); EkraniGuncelle(); PaneliKapat(); } else gorevDetayText.text = "<color=red>Yeterli boşta askerin yok!</color>"; } }
    public void EkraniGuncelle() { if (kaynakTexti != null) kaynakTexti.text = "📅 Gün: " + turSayisi + "  |  ❤️ Moral: " + moral + "  |  ⚔️ Asker: " + bostaAsker + "/" + toplamAsker + " (<color=red>Yaralı: " + yaraliAsker + "</color>)  |  🍞 Yemek: " + yemek + "  |  🪵 Odun: " + odun + "  |  🧱 Taş: " + tas + "  |  💰 Altın: " + altin; }
}