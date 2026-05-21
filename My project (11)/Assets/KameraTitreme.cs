using UnityEngine;
using System.Collections;

public class KameraTitreme : MonoBehaviour
{
    public static KameraTitreme instance;
    private Vector3 orijinalPozisyon;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        orijinalPozisyon = transform.localPosition;
    }

    // Bu fonksiyonu diğer kodlardan çağırıp kamerayı sallayacağız
    public void Titret(float sure, float siddet)
    {
        StartCoroutine(TitremeCoroutine(sure, siddet));
    }

    private IEnumerator TitremeCoroutine(float sure, float siddet)
    {
        float gecenZaman = 0f;

        while (gecenZaman < sure)
        {
            float x = Random.Range(-1f, 1f) * siddet;
            float y = Random.Range(-1f, 1f) * siddet;

            transform.localPosition = new Vector3(orijinalPozisyon.x + x, orijinalPozisyon.y + y, orijinalPozisyon.z);

            gecenZaman += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = orijinalPozisyon;
    }
}