using UnityEngine;

public class KameraKontrol : MonoBehaviour
{
    [Header("Kaydýrma (Pan) Ayarlarý")]
    public float kaydirmaHizi = 0.5f;

    [Header("Yakýnlaþtýrma (Zoom) Ayarlarý")]
    public float zoomHizi = 5f;
    public float minZoom = 3f;
    public float maxZoom = 15f;

    [Header("Harita Sýnýrlarý")]
    public float sinirX = 40f;
    public float sinirZ = 40f;

    private Vector3 sonFarePozisyonuSag;
    private Camera cam;

    private Vector3 baslangicPozisyonu;
    private Quaternion baslangicRotasyonu;

    void Start()
    {
        cam = GetComponent<Camera>();

        // 1. Oyuna maksimum uzaklýkta baþla
        cam.orthographicSize = maxZoom;

        // 2. Baþlangýç pozisyonunu ve açýsýný hafýzaya al
        baslangicPozisyonu = transform.position;
        baslangicRotasyonu = transform.rotation;
    }

    void Update()
    {
        // --- 1. KAYDIRMA (SAÐ TIK) ---
        if (Input.GetMouseButtonDown(1))
        {
            sonFarePozisyonuSag = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 fark = Input.mousePosition - sonFarePozisyonuSag;

            Vector3 ileri = transform.forward;
            Vector3 sag = transform.right;
            ileri.y = 0;
            sag.y = 0;
            ileri.Normalize();
            sag.Normalize();

            Vector3 hareket = (-sag * fark.x) + (-ileri * fark.y);
            float dinamikHiz = kaydirmaHizi * (cam.orthographicSize / 5f);
            Vector3 yeniPozisyon = transform.position + (hareket * dinamikHiz * Time.deltaTime);

            yeniPozisyon.x = Mathf.Clamp(yeniPozisyon.x, -sinirX, sinirX);
            yeniPozisyon.z = Mathf.Clamp(yeniPozisyon.z, -sinirZ, sinirZ);

            transform.position = yeniPozisyon;
            sonFarePozisyonuSag = Input.mousePosition;
        }

        // --- 2. YAKINLAÞTIRMA (FARE TEKERLEÐÝ) ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomHizi;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }

        // --- 3. MERKEZE / KALEYE DÖNME (BOÞLUK TUÞU) ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Pozisyonu, açýyý ve zoomu oyunun en baþýndaki kusursuz haline getir
            transform.position = baslangicPozisyonu;
            transform.rotation = baslangicRotasyonu;
            cam.orthographicSize = maxZoom;
        }
    }
}