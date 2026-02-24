using UnityEngine;

public class KameraKontrol : MonoBehaviour
{
    [Header("Kaydýrma (Pan) Ayarlarý")]
    public float kaydirmaHizi = 0.5f;

    [Header("Yakýnlaþtýrma (Zoom) Ayarlarý")]
    public float zoomHizi = 5f;       // Tekerlek hassasiyeti
    public float minZoom = 3f;        // Ne kadar YAKINA girebileceði (Küçük deðer = Yakýn)
    public float maxZoom = 15f;       // Ne kadar UZAÐA çýkabileceði (Büyük deðer = Uzak)

    [Header("Harita Sýnýrlarý")]
    public float sinirX = 40f;
    public float sinirZ = 40f;

    private Vector3 sonFarePozisyonu;
    private Camera cam; // Kameranýn kendisine ulaþmak için

    void Start()
    {
        // Scriptin takýlý olduðu objeden (Main Camera) Camera bileþenini al
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // --- 1. KAYDIRMA (SAÐ TIK) ---
        if (Input.GetMouseButtonDown(1))
        {
            sonFarePozisyonu = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 fark = Input.mousePosition - sonFarePozisyonu;

            Vector3 ileri = transform.forward;
            Vector3 sag = transform.right;
            ileri.y = 0;
            sag.y = 0;
            ileri.Normalize();
            sag.Normalize();

            Vector3 hareket = (-sag * fark.x) + (-ileri * fark.y);

            // Yakýnlaþtýkça kaydýrma hýzýnýn yavaþlamasý, uzaklaþtýkça hýzlanmasý için ufak bir matematik hilesi:
            float dinamikHiz = kaydirmaHizi * (cam.orthographicSize / 5f);

            Vector3 yeniPozisyon = transform.position + (hareket * dinamikHiz * Time.deltaTime);

            yeniPozisyon.x = Mathf.Clamp(yeniPozisyon.x, -sinirX, sinirX);
            yeniPozisyon.z = Mathf.Clamp(yeniPozisyon.z, -sinirZ, sinirZ);

            transform.position = yeniPozisyon;
            sonFarePozisyonu = Input.mousePosition;
        }

        // --- 2. YAKINLAÞTIRMA (FARE TEKERLEÐÝ) ---
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Tekerlek hareketini al (-1 veya 1)

        if (scroll != 0f)
        {
            // Ortografik boyutu tekerlek hareketiyle deðiþtir (Eksi yapýyoruz ki ileri itince yaklaþsýn)
            cam.orthographicSize -= scroll * zoomHizi;

            // Kameranýn çok fazla yakýna veya uzaða gitmesini engelle
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}