using UnityEngine;

public class pomeranjePozadine : MonoBehaviour
{
    public Transform kamera;              // Podesi u Inspectoru (ako ostaviš prazno, u Start() će uzeti Camera.main)
    private Vector3 offset;               // Trenutni offset u odnosu na kameru
    private GameObject player;

    private bool staniAktivan = false;    // Interna kontrola da li pratimo kameru
    private bool prethodnoStanje = false; // Čuva prethodnu vrednost player.stani da bismo otkrili promenu

    void Start()
    {
        if (kamera == null)
        {
            if (Camera.main != null)
                kamera = Camera.main.transform;
            else
                Debug.LogError("Nema kamere dodeljene i nema Camera.main!");
        }

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Nije pronađen objekat sa tagom 'Player'!");
            enabled = false;
            return;
        }

        // Inicijalno stanje
        bool pocetnoStanje = player.GetComponent<BouncingBall>().stani; //na pocetku je stani false
        if (!pocetnoStanje)
        {
            AktivirajPraćenje(); // Ako je već true na početku
        }
        else
        {
            staniAktivan = false;
        }

        prethodnoStanje = pocetnoStanje;
    }

    void LateUpdate()
    {
        // Čitamo trenutno stanje iz skripte na playeru
        bool sada = player.GetComponent<BouncingBall>().stani;
        print("stani je sada " + sada);
        int nivo = player.GetComponent<BouncingBall>().nivo;
        if (!sada)
        {
            Vector3 novaPozicija = kamera.position - new Vector3(0f, 12 * nivo, 0f) + offset;
            transform.position = new Vector3(transform.position.x, novaPozicija.y, transform.position.z);
        }
    }

    public void AktivirajPraćenje()
    {
        // Re-kalkuliši offset DA SAD (u ovom trenutku), da bi izbegao skok
        offset = transform.position - kamera.position;
        staniAktivan = true;
        // (Po želji: Debug.Log("Aktivirano praćenje pozadine. Novi offsetY = " + offset.y); )
    }
}