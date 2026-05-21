using UnityEngine;

public class KaleTiklama : MonoBehaviour
{
    // Farenin sol tuşuyla kaleye tıklandığında çalışır
    void OnMouseDown()
    {
        KusatmaYoneticisi.instance.AtolyePaneliniAc();
    }
}