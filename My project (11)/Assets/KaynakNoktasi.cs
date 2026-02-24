using UnityEngine;

public class KaynakNoktasi : MonoBehaviour
{
    [Header("Görev Ayarlarý")]
    public string kaynakTipi;
    public int gerekenAdam = 5;
    public int gorevSuresi = 3;

    // Public yapýyoruz ki KusatmaYoneticisi bunlarý okuyabilsin
    [HideInInspector] public bool islemde = false;
    [HideInInspector] public int kalanTur = 0;

    void OnMouseDown()
    {
        // Týklanýnca eskisi gibi hemen adam yollama, yöneticiden MENÜYÜ AÇ!
        KusatmaYoneticisi.instance.GorevPaneliAc(this);
    }

    // GameManager "Onay Ver" butonuna basýnca bunu çaðýracak
    public void GoreviBaslat()
    {
        islemde = true;
        kalanTur = gorevSuresi;
        Debug.Log(kaynakTipi + " görevi baþladý!");
    }

    public void TurAtla()
    {
        if (islemde == true)
        {
            kalanTur--;
            if (kalanTur <= 0)
            {
                islemde = false;
                KusatmaYoneticisi.instance.KesiftenDon(gerekenAdam, kaynakTipi);
            }
        }
    }
}