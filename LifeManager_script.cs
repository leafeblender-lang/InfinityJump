using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
public class LifeManager_script : MonoBehaviour
{
    public static LifeManager_script Instance;

    private const int maxLives = 3;
    private const int regenMinutes = 5;//stavi 30posle
    public TextMeshProUGUI timerText;

    private int currentLives;
    private DateTime? life1RegainTime;
    private DateTime? life2RegainTime;
    private DateTime? life3RegainTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLives();
        }
        else
        {
            Destroy(gameObject);
        }
    }
  
    private void LoadLives()
    {
        currentLives = PlayerPrefs.GetInt("CurrentLives", maxLives);

        if (PlayerPrefs.HasKey("Life1Regen"))
            life1RegainTime = DateTime.Parse(PlayerPrefs.GetString("Life1Regen"));
        if (PlayerPrefs.HasKey("Life2Regen"))
            life2RegainTime = DateTime.Parse(PlayerPrefs.GetString("Life2Regen"));
        if (PlayerPrefs.HasKey("Life3Regen"))
            life3RegainTime = DateTime.Parse(PlayerPrefs.GetString("Life3Regen"));

        UpdateLives();
    }

    private void SaveLives()
    {
        PlayerPrefs.SetInt("CurrentLives", currentLives);

        if (life1RegainTime.HasValue)
            PlayerPrefs.SetString("Life1Regen", life1RegainTime.Value.ToString());
        if (life2RegainTime.HasValue)
            PlayerPrefs.SetString("Life2Regen", life2RegainTime.Value.ToString());
        if (life3RegainTime.HasValue)
            PlayerPrefs.SetString("Life3Regen", life3RegainTime.Value.ToString());

        PlayerPrefs.Save();
    }

    public void UpdateLives()
    {
        DateTime now = DateTime.Now;
       
        if (life1RegainTime.HasValue && now >= life1RegainTime.Value)
        {
           
            currentLives = Mathf.Min(currentLives + 1, maxLives);
            Debug.Log("trenutno " + currentLives + " Zivota life1");
            life1RegainTime = null;
        }
        if (life2RegainTime.HasValue && now >= life2RegainTime.Value)
        {
            currentLives = Mathf.Min(currentLives + 1, maxLives);

            Debug.Log("trenutno " + currentLives + " Zivota life2");
            life2RegainTime = null;
        }
        if (life3RegainTime.HasValue && now >= life3RegainTime.Value)
        {
            currentLives = Mathf.Min(currentLives + 1, maxLives);

            Debug.Log("trenutno " + currentLives + " Zivota life3");
            life3RegainTime = null;
        }

        SaveLives();
        
    }

    public bool HasLives()
    {
        UpdateLives();
        return currentLives > 0;
    }

    public int GetCurrentLives()
    {
        UpdateLives();
        return currentLives;
    }

    public bool TryUseLife()
    {
        UpdateLives();

        if (currentLives <= 0)
            return false;

        currentLives--;

        DateTime regenTime = DateTime.Now.AddMinutes(regenMinutes);

        if (!life1RegainTime.HasValue)
            life1RegainTime = regenTime;
        else if (!life2RegainTime.HasValue)
            life2RegainTime = regenTime;
        else if (!life3RegainTime.HasValue)
            life3RegainTime = regenTime;

        SaveLives();
        return true;
    }

    public void BuyLife()
    {
        UpdateLives();

        if (currentLives >= maxLives)
        {
            Debug.Log("Ve? imaš maksimalan broj života.");
            return;
        }

        currentLives = Mathf.Min(currentLives + 1, maxLives);

        // Uklanjamo tajmer koji je najbliži kraju
        if (life1RegainTime.HasValue && life2RegainTime.HasValue && life3RegainTime.HasValue)
        {
            if (life1RegainTime.Value <= life2RegainTime.Value && life1RegainTime.Value <= life3RegainTime.Value)
                life1RegainTime = null;
            else if (life2RegainTime.Value <= life3RegainTime.Value)
                life2RegainTime = null;
            else
                life3RegainTime = null;
        }
        else if (life1RegainTime.HasValue && life2RegainTime.HasValue)
        {
            if (life1RegainTime.Value <= life2RegainTime.Value)
                life1RegainTime = null;
            else
                life2RegainTime = null;
        }
        else if (life1RegainTime.HasValue && life3RegainTime.HasValue)
        {
            if (life1RegainTime.Value <= life3RegainTime.Value)
                life1RegainTime = null;
            else
                life3RegainTime = null;
        }
        else if (life2RegainTime.HasValue && life3RegainTime.HasValue)
        {
            if (life2RegainTime.Value <= life3RegainTime.Value)
                life2RegainTime = null;
            else
                life3RegainTime = null;
        }
        else if (life1RegainTime.HasValue)
            life1RegainTime = null;
        else if (life2RegainTime.HasValue)
            life2RegainTime = null;
        else if (life3RegainTime.HasValue)
            life3RegainTime = null;

        SaveLives();
        Debug.Log("Kupljen najbliži život. Trenutno: " + currentLives);
    }

    public TimeSpan? GetNextLifeTime()
    {
        UpdateLives();

        DateTime now = DateTime.Now;

        TimeSpan?[] vremena = new[] {
            life1RegainTime.HasValue ? (TimeSpan?)(life1RegainTime.Value - now) : null,
            life2RegainTime.HasValue ? (TimeSpan?)(life2RegainTime.Value - now) : null,
            life3RegainTime.HasValue ? (TimeSpan?)(life3RegainTime.Value - now) : null
        };

        TimeSpan? najmanje = null;

        foreach (var v in vremena)
        {
            if (v.HasValue && (najmanje == null || v.Value < najmanje.Value))
                najmanje = v;
        }

        return najmanje;
    }

    public void UbaciUTextMeshPro()
    {
        TextMeshProUGUI[] sviTekstovi = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

        foreach (var tekst in sviTekstovi)
        {
            if (tekst.gameObject.name == "TimerText")
            {
                timerText = tekst;
                break;
            }
        }
    }
    // ?? NOVA METODA - Instant full lives
    public void RestoreAllLives()
    {
        currentLives = maxLives;

        // Poništi sve tajmere
        life1RegainTime = null;
        life2RegainTime = null;
        life3RegainTime = null;

        SaveLives();

        Debug.Log("? Svi životi vra?eni na maksimum!");

        // Ažuriraj UI ako je potrebno
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject.Find("ManagerPrikazaMaina")?.GetComponent<prikazMaina>()?.changeColorOfHearts();
        }
    }
    private void Update()
    {
        TimeSpan? vreme = GetNextLifeTime();

        if (timerText != null)
        {
            if (vreme.HasValue)
            {
                TimeSpan ts = vreme.Value;
                timerText.text = $"wait: {ts.Minutes:D2}:{ts.Seconds:D2}";
                if(ts.Minutes==0 && ts.Seconds == 0)
                {
                    StartCoroutine(CekajJednuSekundu());
                   
                }
            }
            else
            {
                timerText.text = "Svi životi su puni!";
            }
        
        }
    }
    private IEnumerator CekajJednuSekundu()
    {
        Debug.Log("Po?etak ?ekanja...");
        yield return new WaitForSeconds(1f);
        this.UpdateLives();
        Debug.Log("TajmerIsteko");
        //dodaj ako je aktivna scena 0

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject.Find("ManagerPrikazaMaina")?.GetComponent<prikazMaina>()?.changeColorOfHearts();
        }
    }
}
