using System.Collections.Generic;

using UnityEngine;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GroundSpowner_script : MonoBehaviour
{
    public GameObject[] ground;
    public float yStart; // od koje visine o koje visine
    private float h;//4 je bilo
    public float siguranRazmak; // stavi 1 ako start ne radi osigrurava da 2. prepreka ne bude tacno iznad 1.
    private float previousEnd = 0;
    private float H = 45f;//90
    private List<int> boxChance;
    private int poslednjaPrepreka = -1;
    private int istiTipZaredom = 0;
    private int poslednjaGrupaPrepreke = -1;
    private int istaGrupaZaredom = 0;
    private int jakaPreprekaCooldown = 0;
    private int obaveznaNovaPrepreka = -1;
    private int ukupnoSpawnovanihPrepreka = 0;
    private int preprekeOdDopune = 0;
    private int preprekeZaSledecuDopunu = 7;
    private float coinHeight;
    private float diamonStashdHeight;
    private float diamondHeight;
    private float coinStashHeight;
    [SerializeField] public Transform FitToScreen;
    [SerializeField] public GameObject coin;
    [SerializeField] public GameObject diamond;
    [SerializeField] public GameObject coinStash;
    [SerializeField] public GameObject diamondStash;

    private int nivo = 0;


    private void Awake()
    {
        previousEnd = Camera.main.transform.position.y - Camera.main.orthographicSize;
        boxChance = new List<int>();
        coinHeight = coin.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        diamondHeight = diamond.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        coinStashHeight = coinStash.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        diamonStashdHeight = diamondStash.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        PostaviKutijuVerovatnoce(new int[] { 1, 0, 0 });
    }

    private void Start()
    {
        float worldWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        float baseWorldWidth = 4.5f; // primer za 16:9 ekran
        float siguranRazmakBase = 1.2f;

        siguranRazmak = siguranRazmakBase * (worldWidth / baseWorldWidth);
        Debug.Log("Sig raz" + siguranRazmak);
        h = (Camera.main.orthographicSize * 2) / 4;


    }
    private bool o = true;
    private void Update()
    {
        // print("radi");
        if (o == true)
        {
            // print("radi2");

            spawnMore(); o = false;
        }
    }

    public void spawnMore()
    {


        float newEnd = previousEnd + H;
        float donjaIvicaY = Camera.main.transform.position.y - Camera.main.orthographicSize;
        float leviKrajX = Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect;
        float desniKrajX = Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect;
        int choosenI;
        //int m = 1;
        float prevX = 10f;
        PodesiBrzinuPrepreka();
        while (previousEnd < newEnd)
        {
            choosenI = IzaberiPrepreku();

            GameObject prefab = ground[choosenI];
            //prefab.transform.SetParent(FitToScreen, false);
            //GameObject parentObject = prefab;
            float sirinaPrepreke = prefab.GetComponent<SpriteRenderer>().bounds.size.x / 2;
            float visinaPrepreke = prefab.GetComponent<SpriteRenderer>().bounds.size.y / 2;
            float x;
            float tmp = desniKrajX - leviKrajX;

            // Prva prepreka random bilo gde
            int ir = 0;
            while (true)
            {
                x = Random.Range(leviKrajX + sirinaPrepreke, desniKrajX - sirinaPrepreke);
                ir++;
                if (ir > 20) break;
                if (Mathf.Abs(x - prevX) > siguranRazmak) break;
            }
            prevX = x;
            GameObject instanca = Instantiate(prefab, new Vector3(x, previousEnd, 10f), Quaternion.identity);
            //instanca.transform.SetParent(FitToScreen, true);
            //  GameObject FitToScreen = GameObject.Find("FitToScreen");
            SpawnReward(nivo, x, previousEnd, visinaPrepreke, choosenI, instanca);


            //instanca.transform.SetParent(FitToScreen, false);
            previousEnd += h;
            ukupnoSpawnovanihPrepreka++;
            preprekeOdDopune++;
            DopuniKutijuAkoTreba();
        }


        H = 45f;
        //previousEnd = newEnd;

    }
    // ? HELPER FUNKCIJA (dodaj van while petlje)
    private void SpawnReward(int progressLevel, float x, float y, float obstacleHeight, int obstacleType, GameObject parent)
    {
        // Definiši šanse
        float coinChance, diamondChance, coinStashChance, diamondStashChance;
        float tezina = Mathf.Clamp01(progressLevel / 18f);
        float sredina = Mathf.Clamp01((progressLevel - 3) / 8f);
        float kraj = Mathf.Clamp01((progressLevel - 8) / 10f);

        coinChance = Mathf.Lerp(0.10f, 0.145f, tezina);
        diamondChance = Mathf.Lerp(0.018f, 0.032f, sredina);
        coinStashChance = progressLevel < 3 ? 0f : Mathf.Lerp(0.015f, 0.04f, sredina);
        diamondStashChance = progressLevel < 8 ? 0f : Mathf.Lerp(0.006f, 0.02f, kraj);

        if (JeJakaPrepreka(obstacleType))
        {
            coinChance += Mathf.Lerp(0.015f, 0.035f, tezina);
            diamondChance += 0.006f;
        }

        // Roll dice
        float roll = Random.value;

        // Check in order (most valuable first)
        if (roll < diamondStashChance)
            SpawnObject(diamondStash, x, y + obstacleHeight + diamonStashdHeight, obstacleType, parent, false);
        else if (roll < diamondStashChance + coinStashChance)
            SpawnObject(coinStash, x, y + obstacleHeight + coinStashHeight, obstacleType, parent, true);
        else if (roll < diamondStashChance + coinStashChance + diamondChance)
            SpawnObject(diamond, x, y + obstacleHeight + diamondHeight, obstacleType, parent, false);
        else if (roll < diamondStashChance + coinStashChance + diamondChance + coinChance)
            SpawnObject(coin, x, y + obstacleHeight + coinHeight, obstacleType, parent, true);
        // else: ništa se ne spawna
    }

    private void SpawnObject(GameObject prefab, float x, float y, int obstacleType, GameObject parent, bool disableAnimator)
    {
        GameObject obj = Instantiate(prefab, new Vector3(x, y, 10f), Quaternion.identity);

        // Ako je pokretna prepreka (3, 5, 7), attach na parent
        if (obstacleType == 3 || obstacleType == 5 || obstacleType == 7)
        {
            obj.transform.parent = parent.transform;
            if (disableAnimator && obj.GetComponent<Animator>() != null)
                obj.GetComponent<Animator>().enabled = false;
        }
    }
    //0-normalna
    //1-malaNormalna
    //2-Ledena
    //3-Pomerajuca
    //4-malaLedena
    //5-brza zlatna pomerajuca
    //6-nestajuca ljubicasta
    //7-neonska nestajuca pomerajuca kao 6 al se mrda kao 3


    private void dopuniKutijuVerovatnoce()
    {
        nivo++;
        PostaviKutijuPoTezini();
        PodesiBrzinuPrepreka();
    }

    private void DopuniKutijuAkoTreba()
    {
        if (preprekeOdDopune < preprekeZaSledecuDopunu)
            return;

        preprekeOdDopune = 0;
        dopuniKutijuVerovatnoce();
        preprekeZaSledecuDopunu = IzracunajPreprekeZaSledecuDopunu();
    }

    private void PostaviKutijuVerovatnoce(int[] noveSanse)
    {
        boxChance.Clear();
        boxChance.AddRange(noveSanse);
    }

    private void PostaviKutijuPoTezini()
    {
        float tezina = Mathf.Clamp01(Mathf.Max(nivo / 18f, ukupnoSpawnovanihPrepreka / 160f));
        float sredina = Mathf.Clamp01(Mathf.Max((nivo - 3) / 8f, (ukupnoSpawnovanihPrepreka - 35) / 90f));
        float kraj = Mathf.Clamp01(Mathf.Max((nivo - 9) / 12f, (ukupnoSpawnovanihPrepreka - 90) / 130f));

        boxChance.Clear();

        DodajPreprekuUTezinskuKutiju(0, Mathf.RoundToInt(Mathf.Lerp(5f, 0f, Mathf.Clamp01(tezina * 1.25f))));
        DodajPreprekuUTezinskuKutiju(1, Mathf.RoundToInt(Mathf.Lerp(6f, 1f, Mathf.Clamp01(tezina + kraj * 0.5f))));
        DodajPreprekuUTezinskuKutiju(2, Mathf.RoundToInt(Mathf.Lerp(4f, 1f, Mathf.Clamp01(sredina))));

        if (nivo >= 2)
        {
            DodajPreprekuUTezinskuKutiju(3, Mathf.RoundToInt(Mathf.Lerp(2f, 5f, tezina)));
            DodajPreprekuUTezinskuKutiju(4, Mathf.RoundToInt(Mathf.Lerp(1f, 4f, sredina)));
        }

        if (nivo >= 3)
            DodajPreprekuUTezinskuKutiju(5, Mathf.RoundToInt(Mathf.Lerp(1f, 3f, sredina)));

        if (nivo >= 4)
            DodajPreprekuUTezinskuKutiju(6, Mathf.RoundToInt(Mathf.Lerp(1f, 5f, Mathf.Clamp01((nivo - 4) / 12f))));

        if (nivo >= 6)
            DodajPreprekuUTezinskuKutiju(7, Mathf.RoundToInt(Mathf.Lerp(1f, 5f, Mathf.Clamp01((nivo - 6) / 12f))));

        if (nivo >= 12)
        {
            boxChance.Remove(0);

            if (boxChance.Contains(1) && Random.value < kraj)
                boxChance.Remove(1);
        }

        if (boxChance.Count == 0)
            PostaviKutijuVerovatnoce(new int[] { 3, 4, 5, 6, 7 });

        if (nivo == 1)
            obaveznaNovaPrepreka = 2;
        else if (nivo == 2)
            obaveznaNovaPrepreka = 3;
        else if (nivo == 3)
            obaveznaNovaPrepreka = 5;
        else if (nivo == 4)
            obaveznaNovaPrepreka = 6;
        else if (nivo == 6)
            obaveznaNovaPrepreka = 7;
    }

    private void DodajPreprekuUTezinskuKutiju(int tip, int kolikoPuta)
    {
        for (int i = 0; i < kolikoPuta; i++)
            boxChance.Add(tip);
    }

    private int IzracunajPreprekeZaSledecuDopunu()
    {
        if (nivo < 4)
            return 6;

        if (nivo < 10)
            return 7;

        if (nivo < 18)
            return 8;

        return 9;
    }

    private int IzaberiPrepreku()
    {
        if (obaveznaNovaPrepreka != -1 && MozeDaSeIzabere(obaveznaNovaPrepreka))
        {
            int izbor = obaveznaNovaPrepreka;
            obaveznaNovaPrepreka = -1;
            SacuvajIzabranuPrepreku(izbor);
            return izbor;
        }

        List<int> kandidati = new List<int>();

        for (int i = 0; i < boxChance.Count; i++)
        {
            int tip = boxChance[i];

            if (MozeDaSeIzabere(tip))
                kandidati.Add(tip);
        }

        List<int> izbori = kandidati.Count > 0 ? kandidati : boxChance;
        int izabranaPrepreka = izbori[Random.Range(0, izbori.Count)];

        if (izabranaPrepreka == obaveznaNovaPrepreka)
            obaveznaNovaPrepreka = -1;

        SacuvajIzabranuPrepreku(izabranaPrepreka);
        return izabranaPrepreka;
    }

    private bool MozeDaSeIzabere(int tip)
    {
        if (jakaPreprekaCooldown > 0 && JeJakaPrepreka(tip))
            return false;

        if (tip == poslednjaPrepreka && (JeJakaPrepreka(tip) || istiTipZaredom >= 2))
            return false;

        if (GrupaPrepreke(tip) == poslednjaGrupaPrepreke && istaGrupaZaredom >= 2)
            return false;

        return true;
    }

    private void SacuvajIzabranuPrepreku(int tip)
    {
        int grupa = GrupaPrepreke(tip);

        if (tip == poslednjaPrepreka)
            istiTipZaredom++;
        else
        {
            poslednjaPrepreka = tip;
            istiTipZaredom = 1;
        }

        if (grupa == poslednjaGrupaPrepreke)
            istaGrupaZaredom++;
        else
        {
            poslednjaGrupaPrepreke = grupa;
            istaGrupaZaredom = 1;
        }

        if (jakaPreprekaCooldown > 0)
            jakaPreprekaCooldown--;

        if (JeJakaPrepreka(tip))
            jakaPreprekaCooldown = 1;
    }

    private bool JeJakaPrepreka(int tip)
    {
        return tip == 5 || tip == 6 || tip == 7;
    }

    private int GrupaPrepreke(int tip)
    {
        if (tip == 0 || tip == 1)
            return 0;

        if (tip == 2 || tip == 4)
            return 1;

        if (tip == 3 || tip == 5)
            return 2;

        if (tip == 6 || tip == 7)
            return 3;

        return tip;
    }

    private void PodesiBrzinuPrepreka()
    {
        float tezina = Mathf.Clamp01(Mathf.Max(nivo / 20f, ukupnoSpawnovanihPrepreka / 180f));

        KretanjePomerajucePreprekeCrvene_script.brzinaPrepreke = Mathf.Lerp(2f, 3.2f, tezina);
        KretanjePomerajuceZlatne.brzinaPrepreke = Mathf.Lerp(3.5f, 5.3f, tezina);
    }

    public void OcistiStarePrepreke(float granica)
    {
        GameObject[] prepreke1 = GameObject.FindGameObjectsWithTag("Ground1");
        GameObject[] prepreke2 = GameObject.FindGameObjectsWithTag("Nestajuca");

        GameObject[] prepreke3 = GameObject.FindGameObjectsWithTag("Pomerajuca");

        GameObject[] prepreke4 = GameObject.FindGameObjectsWithTag("NeonskaNestajuca");

        //GameObject[] prepreke2 = GameObject.FindGameObjectsWithTag("Ground2");

        List<GameObject> svePrepreke = new List<GameObject>();

        svePrepreke.AddRange(prepreke1);
        svePrepreke.AddRange(prepreke2);
        svePrepreke.AddRange(prepreke3);
        svePrepreke.AddRange(prepreke4);

        //svePrepreke.AddRange(prepreke2);

        foreach (GameObject prepreka in svePrepreke)
        {
            if (prepreka != null && prepreka.transform.position.y < granica)
            {
                Destroy(prepreka);
            }
        }

    }
}
