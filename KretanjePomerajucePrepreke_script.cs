using System.Collections;
//using UnityEditor.Search;
using UnityEngine;

public class KretanjePomerajucePreprekeCrvene_script : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float brzinaPreprekeUInspectoru = 3f;
    [SerializeField] private bool koristiBrzinuIzInspectora;

    public static float brzinaPrepreke=3f;
    private float leviKrajX, desniKrajX;
    private float sirina; //sirina 1 bloka 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        float pocetnaBrzina = koristiBrzinuIzInspectora
            ? brzinaPreprekeUInspectoru
            : brzinaPrepreke;

        if (Random.Range(0, 2) == 1)
            pocetnaBrzina *= -1;

        rb.linearVelocityX = pocetnaBrzina;
        sirina = GetComponent<SpriteRenderer>().bounds.size.x;
        leviKrajX = Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect;
        desniKrajX = Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect;
    }
    public float dohvatiBrzinu() { return rb.linearVelocityX; }
    private void OkreniZnakBrzine()
    {
        rb.linearVelocityX *= -1;
    }
    private bool cekanje = false;

    private void PromeniSmerAkoMoze()
    {
        if (cekanje) return;

        OkreniZnakBrzine();
        cekanje = true;
        StartCoroutine(CekajMalo());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<BouncingBall>() != null)
            return;

        PromeniSmerAkoMoze();
    }

    void Update()
    {
        if (!cekanje && (transform.position.x -sirina/2 <= leviKrajX || transform.position.x + sirina / 2 >= desniKrajX))
        {
            PromeniSmerAkoMoze();
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

