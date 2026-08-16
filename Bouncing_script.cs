// Bouncing_script.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Keyboard = UnityEngine.InputSystem.Keyboard;

public class BouncingBall : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject spawner;
    public float BrzinaPada = 0f;
    private float maxBrzinaPada;
    public float ubrzanje = 28f;
    private float trenVisina = 0;
    public float targetHeight;
    public bool up = false;
    private float H_pravljenja = 35f;
    private GameObject obj;
    private GameObject scoreManager;
    private bool movingLeft = false;
    private bool movingRight = false;
    public bool token = false;
    private bool gameOver = false;
    private int coinCnt = 0;
    private int diamondCnt = 0;
    public Sprite[] listaSpritova;
    [SerializeField] public GameObject gameOverPanel;
    [SerializeField] private LevelGameOverUI levelGameOverUI; // Game Over panel za nivoe (Restart + Exit)
    [SerializeField] private GameObject WatchAdPanel;
    [SerializeField] private float moveSpeed = 2.4f;
    [SerializeField, Min(0.01f)] private float referentnaSirinaSveta = 5.625f;
    // Sirina sveta za orthographic size 6.09375 na referentnoj rezoluciji 9:20.
    private const float REFERENTNA_SIRINA_SVETA_OSTALIH_SCENA = 6.09375f * 2f * (9f / 19.5f);
    private float osnovnaHorizontalnaBrzina;
    private float prethodnaSirinaSveta = -1f;
    private bool videoClicked = false;
    private GameObject poslednajaPrepreka;
    [SerializeField] private GameObject regenerirajucaPrepreka;
    [SerializeField] public GameObject LvLCompletePanel;
    private int scenaIndex;
    private float pom_camera;
    public bool stani = false;
    private int oos = 0;
    private bool yy = true;
    private Collider2D[] sviKolajderiLoptice = Array.Empty<Collider2D>();
    private SpriteRenderer prikazLoptice;
    private SpriteRenderer prikazWrapKopije;
    // ✅ NOVO: Minimalni ugao za validnu koliziju (lopta mora da padne ODOZGO)
    private const float MIN_COLLISION_ANGLE = 0.3f;
    private const float TOP_CONTACT_TOLERANCE = 0.03f;
    private const float BOUNCE_SNAP_OFFSET = 0.005f;
    private const float SIDE_COLLISION_MARGIN = 0.02f;
    private readonly HashSet<Collider2D> ignorisaniBocniCollideri = new HashSet<Collider2D>();

    void Awake()
    {
        Application.targetFrameRate = 120;
        stani = false;
        prikazLoptice = GetComponent<SpriteRenderer>();
        string choose = PlayerPrefs.GetString("SelektovanSprite", "WhiteBall");
        if (HesmapaSkinova.mojaMapa != null && HesmapaSkinova.mojaMapa.ContainsKey(choose))
        {
            prikazLoptice.sprite = listaSpritova[HesmapaSkinova.mojaMapa[choose]];
        }
        else
        {
            prikazLoptice.sprite = listaSpritova[0];
        }
        NapraviWrapKopiju();
    }

    public int nivo = 0;

    void Start()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        up = false;
        scenaIndex = SceneManager.GetActiveScene().buildIndex;
        if (scenaIndex == 1)
        {
            pom_camera = (Camera.main.orthographicSize * 2) / 4;
            targetHeight = (Camera.main.orthographicSize * 2f) / 6 + 1.8f;
        }
        else
        {
            pom_camera = 0;
        }
        Time.timeScale = 1f;
        this.transform.position = new Vector3(transform.position.x, -3.4f);
        maxBrzinaPada = -Mathf.Sqrt(2 * (ubrzanje * targetHeight));
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        rb.gravityScale = 0;

        scoreManager = GameObject.Find("ScoreManager");
        spawner = GameObject.Find("GroundSpawner");
        obj = GameObject.Find("KretanjeKamere");
        sviKolajderiLoptice = GetComponents<Collider2D>();
        osnovnaHorizontalnaBrzina = moveSpeed;
        OsveziHorizontalnuBrzinu();

    }

    private void OsveziHorizontalnuBrzinu()
    {
        float trenutnaSirinaSveta = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        float referentnaSirina = scenaIndex == 1
            ? referentnaSirinaSveta
            : REFERENTNA_SIRINA_SVETA_OSTALIH_SCENA;

        if (Mathf.Approximately(trenutnaSirinaSveta, prethodnaSirinaSveta))
            return;

        moveSpeed = osnovnaHorizontalnaBrzina * (trenutnaSirinaSveta / Mathf.Max(referentnaSirina, 0.01f));
        prethodnaSirinaSveta = trenutnaSirinaSveta;
    }

    public bool getDirectionUpDowno() { return up; }

    public void odgledaoVideoZaDupliranje()
    {
        WatchAdPanel.SetActive(false);
        Button btn = gameOverPanel.GetComponentInChildren<Button>();
        if (btn != null)
        {
            btn.gameObject.SetActive(false);
        }
        ScoreManager_script.instance.updateCoinOnQuitePanel(coinCnt, 1);
        ScoreManager_script.instance.updateDiamondOnQuitePanel(diamondCnt, 1);
    }

    public void vratiUbrzanje() { ubrzanje = 28f; }

    public void odgledaoVideoZaPonovnoIgranje()
    {
        videoClicked = true;
        WatchAdPanel.SetActive(false);
        float x = poslednajaPrepreka.transform.position.x;
        float y = poslednajaPrepreka.transform.position.y;

        GameObject instanca = Instantiate(regenerirajucaPrepreka, new Vector3(x, y, 10f), Quaternion.identity);
        Vector3 respawnPozicija = new Vector3(x, y + 1f);
        this.transform.position = respawnPozicija;
        if (rb != null)
        {
            rb.position = respawnPozicija;
            rb.linearVelocity = Vector2.zero;
        }
        vratiUbrzanje();
        gameOver = false;
        up = false; // ✅ FIXED: Resetuj smer pri respawnu
        BrzinaPada = 0f; // ✅ FIXED: Resetuj brzinu
        Destroy(poslednajaPrepreka);
    }

    public int getCoins() { return coinCnt; }

    private IEnumerator ShrinkAndMoveToCenter(Transform target)
    {
        float duration = 1.1f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 0.1f;
        Vector3 startPos = transform.position;
        Vector3 endPos = target.position;

        while (elapsed < duration)
        {
            if (target == null) yield break;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;
        transform.position = endPos;
        LvLCompletePanel.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D triger)
    {
        if (triger.gameObject.tag == "KrajNivoa")
        {
            StartCoroutine(ShrinkAndMoveToCenter(triger.gameObject.transform));
            LevelManager_script.Instance.levelCompleted(scenaIndex - 1);
            return;
        }
        if (triger.CompareTag("AktivirajucaSuma"))
        {
            triger.gameObject.GetComponent<PojaviNestajucu1>().Aktiviraj();
            return;
        }
        if (triger.gameObject.CompareTag("AktivirajDeaktiviraj"))
        {
            if (triger.gameObject.TryGetComponent<AktivirajIDeaktivirajNestajucu11>(out var skripta))
            {
                skripta.Pokreni();
            }
            else
            {
                Debug.LogWarning("Objekat ima tag AktivirajDeaktiviraj, ali nema skriptu AktivirajIDeaktivirajNestajucu1!");
            }

            return;
        }

        if (triger.CompareTag("Coin"))
        {
            coinCnt++;
            triger.enabled = false;
            Destroy(triger.gameObject);
            SoundManager.instance.ZvukPrikupljenogCoina();
            return;
        }
        else if (triger.CompareTag("coinStash"))
        {
            coinCnt += 3;
            triger.enabled = false;
            Destroy(triger.gameObject);
            SoundManager.instance.ZvukPrikupljenogCoina();
            return;
        }
        else if (triger.CompareTag("diamond"))
        {
            diamondCnt += 1;
            triger.enabled = false;
            Destroy(triger.gameObject);
            SoundManager.instance.ZvukPrikupljenogCoina();
            return;
        }
        else if (triger.CompareTag("diamondStash"))
        {
            diamondCnt += 3;
            triger.enabled = false;
            Destroy(triger.gameObject);
            SoundManager.instance.ZvukPrikupljenogCoina();
            return;
        }
    }

    Collider2D prevColilider;
    private int granicaZaPozadinu = 220;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ObradiSudarSaPlatformom(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ObradiSudarSaPlatformom(collision);
    }

    private void ObradiSudarSaPlatformom(Collision2D collision)
    {
        if (collision.contacts.Length == 0 || gameOver) return;
        Collider2D aktivniKolajderLoptice = PronadjiKolajderZaSudar(collision);
        if (aktivniKolajderLoptice == null)
            return;

        bool udaraOdozgo = UdaraOdozgo(collision, aktivniKolajderLoptice);

        // Platform Effector rešava prolazak odozdo, mi samo obrađujemo odskok
        if (!up && udaraOdozgo)
        {
            // KOREKCIJA PENETRACIJE
            float platformTop = collision.collider.bounds.max.y;
            float ballBottom = aktivniKolajderLoptice.bounds.min.y;

            if (ballBottom < platformTop)
            {
                Vector3 pos = transform.position;
                pos.y += (platformTop - ballBottom) + BOUNCE_SNAP_OFFSET;
                transform.position = pos;
                rb.position = pos;
            }

            // ODSKOČI
            BrzinaPada = -Mathf.Sqrt(2 * (ubrzanje * targetHeight));
            up = true;
            StartCoroutine(ChangeUp());

            if (prevColilider != null)
            {
                if (collision.gameObject.tag == "NeonskaNestajuca")
                {
                    collision.gameObject.GetComponent<TreperucaSkripta>().neonskoNestani();
                }
                if (collision.gameObject.tag == "Nestajuca" || collision.gameObject.tag == "NestajucaAktivirajuca")
                {
                    collision.gameObject.GetComponent<OtapanjeIceBloka_scripta>().pocniOtapanje();
                }
                if (collision.gameObject.tag == "Aktivirajuca" || collision.gameObject.tag == "NestajucaAktivirajuca")
                {
                    collision.gameObject.GetComponent<PojaviNestajucu1>().Aktiviraj();
                }
               
              
                if (prevColilider != collision.collider)
                {
                    if (trenVisina > 1f)
                    {
                        if (obj != null)
                        {
                            PracenjeKamere skripta = obj.GetComponent<PracenjeKamere>();
                            if (skripta != null)
                            {
                                oos++;
                                if (oos > granicaZaPozadinu || yy == false)
                                {
                                    stani = true;
                                    if (yy)
                                    {
                                        oos = 0;
                                        yy = false;
                                    }
                                    if (oos == 5)
                                    {
                                        GameObject obj = GameObject.Find("poDINA_0");
                                        if (obj != null)
                                        {
                                            Destroy(obj);
                                        }
                                        else
                                        {
                                            GameObject obj2 = GameObject.Find("poDINA_1");
                                            if (obj2 != null)
                                            {
                                                Destroy(obj2);
                                            }
                                        }
                                        if (nivo < 2)
                                            nivo++;
                                        stani = false;
                                        yy = true;
                                        if (granicaZaPozadinu < 250)
                                            granicaZaPozadinu += 500;
                                        else granicaZaPozadinu += 1000000;
                                    }
                                }
                                skripta.pomeriKameru(pom_camera);
                                prevColilider = collision.collider;
                                if (SceneManager.GetActiveScene().buildIndex == 1)
                                {
                                    ScoreManager_script.instance.updateScore(1);
                                }
                                poslednajaPrepreka = collision.collider.gameObject;
                            }
                        }
                    }
                }
            }
            else
            {
                prevColilider = collision.collider;
            }
            trenVisina = 0;
        }
        else if (!udaraOdozgo)
        {
            // Ako udar nije sa gornje strane, privremeno ignoriši koliziju.
            // U padu guramo lopticu bočno da se ne zalepi za ivicu prepreke.
            // U usponu (ispod prepreke) ne guramo je bočno da bi prirodno prošla naviše.
            IgnorisiSudarPrivremeno(collision.collider, aktivniKolajderLoptice, izgurajBocno: !up);
        }
        // Nema else if blokova - Platform Effector rešava ostalo
    }

    private void IgnorisiSudarPrivremeno(Collider2D platformCollider, Collider2D kolajderLoptice, bool izgurajBocno)
    {
        if (platformCollider == null || ignorisaniBocniCollideri.Contains(platformCollider))
            return;

        if (izgurajBocno)
        {
            IzgurajLopticuIzBocnogPreklopa(kolajderLoptice, platformCollider);
        }

        foreach (Collider2D kolajder in UcitajAktivneKolajdereLoptice())
        {
            if (kolajder == null || !kolajder.enabled || kolajder.isTrigger)
                continue;

            Physics2D.IgnoreCollision(kolajder, platformCollider, true);
        }
        ignorisaniBocniCollideri.Add(platformCollider);
        StartCoroutine(VratiKolizijuKadaSeRazdvoje(platformCollider));
    }

    private void IzgurajLopticuIzBocnogPreklopa(Collider2D kolajderLoptice, Collider2D platformCollider)
    {
        if (kolajderLoptice == null)
            return;

        Bounds ballBounds = kolajderLoptice.bounds;
        Bounds platformBounds = platformCollider.bounds;

        if (!ballBounds.Intersects(platformBounds))
            return;

        Vector3 pos = transform.position;

        if (ballBounds.center.x < platformBounds.center.x)
        {
            float overlap = ballBounds.max.x - platformBounds.min.x;
            pos.x -= overlap + SIDE_COLLISION_MARGIN;
        }
        else
        {
            float overlap = platformBounds.max.x - ballBounds.min.x;
            pos.x += overlap + SIDE_COLLISION_MARGIN;
        }

        transform.position = pos;
        rb.position = pos;
    }

    private bool UdaraOdozgo(Collision2D collision, Collider2D kolajderLoptice)
    {
        if (kolajderLoptice == null)
            return false;

        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.y > MIN_COLLISION_ANGLE)
                return true;
        }

        float platformTop = collision.collider.bounds.max.y;
        float ballBottom = kolajderLoptice.bounds.min.y;
        float ballCenter = kolajderLoptice.bounds.center.y;
        float tolerancija = Mathf.Max(TOP_CONTACT_TOLERANCE, Mathf.Abs(BrzinaPada) * Time.fixedDeltaTime + BOUNCE_SNAP_OFFSET);

        return ballCenter > platformTop && ballBottom >= platformTop - tolerancija;
    }

    public bool getStani() { return stani; }

    private IEnumerator VratiKolizijuKadaSeRazdvoje(Collider2D platformCollider)
    {
        yield return new WaitForFixedUpdate();

        while (platformCollider != null && DaLiSePreklapaSaBiloKojimKolajderom(platformCollider))
        {
            yield return new WaitForFixedUpdate();
        }

        if (platformCollider != null) // ✅ FIXED: Null check
        {
            foreach (Collider2D kolajder in UcitajAktivneKolajdereLoptice())
            {
                if (kolajder == null || !kolajder.enabled || kolajder.isTrigger)
                    continue;

                Physics2D.IgnoreCollision(kolajder, platformCollider, false);
            }
        }

        ignorisaniBocniCollideri.Remove(platformCollider);
        ignorisaniBocniCollideri.RemoveWhere(c => c == null);
    }

    private Collider2D PronadjiKolajderZaSudar(Collision2D collision)
    {
        if (collision != null && collision.otherCollider != null && collision.otherCollider.enabled)
            return collision.otherCollider;

        return PronadjiAktivniKolajderLoptice();
    }

    private Collider2D PronadjiAktivniKolajderLoptice()
    {
        Collider2D[] aktivniKolajderi = UcitajAktivneKolajdereLoptice();
        for (int i = 0; i < aktivniKolajderi.Length; i++)
        {
            if (aktivniKolajderi[i] != null && aktivniKolajderi[i].enabled && !aktivniKolajderi[i].isTrigger)
                return aktivniKolajderi[i];
        }

        for (int i = 0; i < aktivniKolajderi.Length; i++)
        {
            if (aktivniKolajderi[i] != null)
                return aktivniKolajderi[i];
        }

        return null;
    }

    private Collider2D[] UcitajAktivneKolajdereLoptice()
    {
        if (sviKolajderiLoptice == null || sviKolajderiLoptice.Length == 0)
            sviKolajderiLoptice = GetComponents<Collider2D>();

        return sviKolajderiLoptice;
    }

    private bool DaLiSePreklapaSaBiloKojimKolajderom(Collider2D platformCollider)
    {
        if (platformCollider == null)
            return false;

        Collider2D[] aktivniKolajderi = UcitajAktivneKolajdereLoptice();
        for (int i = 0; i < aktivniKolajderi.Length; i++)
        {
            Collider2D kolajder = aktivniKolajderi[i];
            if (kolajder == null || !kolajder.enabled)
                continue;

            if (BoundsSePreklapaju(kolajder.bounds, platformCollider.bounds))
                return true;
        }

        return false;
    }

    private bool BoundsSePreklapaju(Bounds a, Bounds b)
    {
        return a.min.x < b.max.x + SIDE_COLLISION_MARGIN &&
               a.max.x > b.min.x - SIDE_COLLISION_MARGIN &&
               a.min.y < b.max.y + SIDE_COLLISION_MARGIN &&
               a.max.y > b.min.y - SIDE_COLLISION_MARGIN;
    }

    private bool PrimeniHorizontalniWrap(ref Vector2 worldPos)
    {
        Camera cam = Camera.main;
        if (cam == null)
            return false;

        float halfWidth = cam.orthographicSize * cam.aspect;
        float left = cam.transform.position.x - halfWidth;
        float right = cam.transform.position.x + halfWidth;
        Collider2D aktivniKolajder = PronadjiAktivniKolajderLoptice();
        Bounds graniceLoptice = prikazLoptice != null
            ? prikazLoptice.bounds
            : aktivniKolajder != null
                ? aktivniKolajder.bounds
                : new Bounds(transform.position, Vector3.zero);
        float pomerajCentraX = graniceLoptice.center.x - transform.position.x;
        float centarLopticeX = worldPos.x + pomerajCentraX;
        float sirinaWrapPutanje = right - left;

        if (centarLopticeX < left)
        {
            worldPos.x += sirinaWrapPutanje;
            return true;
        }
        else if (centarLopticeX > right)
        {
            worldPos.x -= sirinaWrapPutanje;
            return true;
        }

        return false;
    }

    private void NapraviWrapKopiju()
    {
        if (prikazLoptice == null)
            return;

        GameObject wrapKopija = new GameObject("WrapVizuelnaKopija");
        wrapKopija.layer = gameObject.layer;
        wrapKopija.transform.SetParent(transform.parent, false);
        prikazWrapKopije = wrapKopija.AddComponent<SpriteRenderer>();
        prikazWrapKopije.enabled = false;
    }

    private void LateUpdate()
    {
        OsveziWrapKopiju();
    }

    private void OsveziWrapKopiju()
    {
        if (prikazLoptice == null || prikazWrapKopije == null)
            return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            prikazWrapKopije.enabled = false;
            return;
        }

        float halfWidth = cam.orthographicSize * cam.aspect;
        float left = cam.transform.position.x - halfWidth;
        float right = cam.transform.position.x + halfWidth;
        float sirinaEkrana = right - left;
        Bounds graniceLoptice = prikazLoptice.bounds;
        float pomerajX = 0f;

        if (graniceLoptice.min.x < left)
            pomerajX = sirinaEkrana;
        else if (graniceLoptice.max.x > right)
            pomerajX = -sirinaEkrana;

        if (Mathf.Approximately(pomerajX, 0f))
        {
            prikazWrapKopije.enabled = false;
            return;
        }

        Transform wrapTransform = prikazWrapKopije.transform;
        wrapTransform.position = transform.position + Vector3.right * pomerajX;
        wrapTransform.rotation = transform.rotation;
        wrapTransform.localScale = transform.localScale;

        prikazWrapKopije.sprite = prikazLoptice.sprite;
        prikazWrapKopije.color = prikazLoptice.color;
        prikazWrapKopije.flipX = prikazLoptice.flipX;
        prikazWrapKopije.flipY = prikazLoptice.flipY;
        prikazWrapKopije.sharedMaterial = prikazLoptice.sharedMaterial;
        prikazWrapKopije.sortingLayerID = prikazLoptice.sortingLayerID;
        prikazWrapKopije.sortingOrder = prikazLoptice.sortingOrder;
        prikazWrapKopije.enabled = prikazLoptice.enabled;
    }

    private void OnDestroy()
    {
        if (prikazWrapKopije != null)
            Destroy(prikazWrapKopije.gameObject);
    }

    void Update()
    {
        OsveziHorizontalnuBrzinu();

        if (transform.position.y > H_pravljenja)
        {
            spawner.GetComponent<GroundSpowner_script>().spawnMore();
            spawner.GetComponent<GroundSpowner_script>().OcistiStarePrepreke(H_pravljenja - 10f);
            H_pravljenja += 35f;
        }

        movingLeft = false;
        movingRight = false;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            movingLeft = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
            movingRight = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;
        }

        if (Touch.activeTouches.Count > 0)
        {
            movingLeft = false;
            movingRight = false;

            Touch lastTouch = Touch.activeTouches[0];
            foreach (var t in Touch.activeTouches)
            {
                if (t.startTime > lastTouch.startTime)
                    lastTouch = t;
            }

            Vector2 touchPos = lastTouch.screenPosition;
            if (touchPos.x < Screen.width / 2f)
                movingLeft = true;
            else
                movingRight = true;
        }

        IsGameOver();
    }

    private void IsGameOver()
    {
        if (!gameOver)
        {
            float camBottomY = Camera.main.transform.position.y - Camera.main.orthographicSize;

            if (transform.position.y < camBottomY - 0.2f)
            {
                this.BrzinaPada = 0f;
                this.ubrzanje = 0;

                // FreePlay (buildIndex == 1) zadrzava Watch-Ad tok.
                // Nivoi idu direktno na Game Over (Restart + Exit).
                bool jeFreePlay = SceneManager.GetActiveScene().buildIndex == 1;

                if (jeFreePlay && videoClicked == false && WatchAdPanel != null)
                {
                    StartCoroutine(VieoPanelAktiviraj());
                }
                else
                {
                    StartCoroutine(GameOverSekvenca());
                    gameOver = true;
                }
            }
        }
    }

    [SerializeField] private UnityEngine.UI.Slider timerSlider;

    private IEnumerator VieoPanelAktiviraj()
    {
        WatchAdPanel.SetActive(true);

        float vreme = 5f;
        timerSlider.maxValue = 5f;
        timerSlider.value = 5f;

        while (vreme > 0)
        {
            vreme -= Time.deltaTime;
            timerSlider.value = vreme;
            yield return null;
        }

        WatchAdPanel.SetActive(false);
        if (videoClicked == true) { }
        else
        {
            StartCoroutine(GameOverSekvenca());
        }
    }

    private IEnumerator GameOverSekvenca()
    {
        if (gameOver == false)
        {
            gameOver = true;
            scoreManager.GetComponent<ScoreManager_script>().postaviNoviHighScore();

            bool jeFreePlay = SceneManager.GetActiveScene().buildIndex == 1;
            if (jeFreePlay)
            {
                ScoreManager_script.instance.updateScoreOnQuitePanel();
                ScoreManager_script.instance.updateCoinOnQuitePanel(coinCnt);
                ScoreManager_script.instance.updateDiamondOnQuitePanel(diamondCnt);
            }
            else
            {
                // Nivo: bankuj sakupljenu valutu bez diranja QuitePanel-a (koji ne postoji u levelima)
                ScoreManager_script.instance.updateSavedCoins(coinCnt);
                ScoreManager_script.instance.updateSavedDiamond(diamondCnt);
            }

            BrzinaPada = 0f;
            ubrzanje = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;

            yield return new WaitForSeconds(0.5f);

            if (!jeFreePlay && levelGameOverUI != null)
            {
                levelGameOverUI.Show(ScoreManager_script.instance.score, coinCnt, diamondCnt);
            }
            else if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("GameOverPanel nije dodeljen u Inspectoru!");
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 novaPozicija = rb.position;

        if (movingLeft)
        {
            novaPozicija.x -= moveSpeed * Time.fixedDeltaTime;
        }
        else if (movingRight)
        {
            novaPozicija.x += moveSpeed * Time.fixedDeltaTime;
        }

        if (up)
        {
            BrzinaPada += ubrzanje * Time.fixedDeltaTime;
            float deltaS = Mathf.Abs(BrzinaPada) * Time.fixedDeltaTime;

            novaPozicija.y += deltaS;
            trenVisina += deltaS;

            if (trenVisina >= targetHeight)
            {
                up = false;
                StartCoroutine(ChangeUp());
            }
        }
        else
        {
            if (Math.Abs(BrzinaPada) <= Math.Abs(maxBrzinaPada))
            {
                BrzinaPada += ubrzanje * Time.fixedDeltaTime;
            }
            float deltaS = Mathf.Abs(BrzinaPada * Time.fixedDeltaTime);

            novaPozicija.y -= deltaS;
        }

        bool preslaNaDruguStranu = PrimeniHorizontalniWrap(ref novaPozicija);
        if (preslaNaDruguStranu)
        {
            // MovePosition bi interpolirao skok preko celog ekrana kao obicno kretanje.
            rb.position = novaPozicija;
            transform.position = new Vector3(novaPozicija.x, novaPozicija.y, transform.position.z);
        }
        else
        {
            rb.MovePosition(novaPozicija);
        }
    }

    private IEnumerator ChangeUp()
    {
        yield return new WaitForSeconds(0.2f);
        token = !token;
    }

    public void ForceResetDirection()
    {
        up = false;
    }
}
