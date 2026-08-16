using System.Collections;
//using UnityEditor.Search;
using UnityEngine;

public class KretanjePomerajuceZlatne : MonoBehaviour
{
    private Rigidbody2D rb;
    public static float brzinaPrepreke = 4.5f;//3;
    //[SerializeField] GameObject leviSin;
    // [SerializeField] GameObject desniSin;
    private float leviKrajX, desniKrajX;
    public float usporenje = 0f;
    private float sirina; //sirina 1 bloka 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocityX = brzinaPrepreke-usporenje;
        sirina = GetComponent<SpriteRenderer>().bounds.size.x;
        leviKrajX = Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect;
        desniKrajX = Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect;

        int x = Random.Range(0, 2);
        if (x == 1) brzinaPrepreke *= -1;
    }

    private void OkreniZnakBrzine()
    {
        rb.linearVelocityX *= -1;

    }
    private bool cekanje = false;
    void Update()
    {
        if (!cekanje && (transform.position.x - sirina / 2 <= leviKrajX || transform.position.x + sirina / 2 >= desniKrajX))
        {
            OkreniZnakBrzine();
            cekanje = true;
            StartCoroutine(CekajMalo());
            //kosristimo da ne bi u trenutnku poziva leviS....x -sirina/2 <leviKraj
            //a u sledecem frejmu bas zapalu u slucaj<=
            //simetrcno i sto se tice desnog kraja
        }
    }

    private IEnumerator CekajMalo()
    {
        yield return new WaitForSeconds(0.5f);
        cekanje = false;
    }
}
