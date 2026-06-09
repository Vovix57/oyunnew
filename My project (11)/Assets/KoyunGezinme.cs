using UnityEngine;

public class KoyunGezinme : MonoBehaviour
{
    public float gezinmeAlani = 10f; // Koyun merkezden en fazla ne kadar uzaklaşsın?
    public float hiz = 2f;
    private Vector3 merkezNokta;
    private Vector3 hedefPozisyon;

    void Start()
    {
        merkezNokta = transform.position; // Koyunun başladığı yeri merkez yap
        YeniHedefBelirle();
    }

    void Update()
    {
        // Hedefe doğru yürü
        transform.position = Vector3.MoveTowards(transform.position, hedefPozisyon, hiz * Time.deltaTime);

        // Hedefe ulaştıysan yeni hedef belirle
        if (Vector3.Distance(transform.position, hedefPozisyon) < 0.5f)
        {
            YeniHedefBelirle();
        }

        // Koyun rotasyonunu hedefe çevir
        if (hedefPozisyon != transform.position)
            transform.LookAt(hedefPozisyon);
    }

    void YeniHedefBelirle()
    {
        // Belirlenen alan içinde rastgele bir nokta seç
        Vector3 rastgeleYer = Random.insideUnitSphere * gezinmeAlani;
        rastgeleYer += merkezNokta;
        rastgeleYer.y = transform.position.y; // Yüksekliği sabit tut
        hedefPozisyon = rastgeleYer;
    }
}