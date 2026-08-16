using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_script : MonoBehaviour
{
    [SerializeField] private GameObject RestartPanel;

    [Header("Nema zivota (Level scene)")]
    [SerializeField] private GameObject noLivesPanel;              // elegantan panel kada nema zivota
    [SerializeField] private RewardedAdsManager rewardedAdsManager; // za "gledaj reklamu za zivot"
    [SerializeField] private TMPro.TextMeshProUGUI noLivesGemFeedback; // opciono: feedback kada nema dovoljno dijamanata
    private const int LIFE_GEM_COST = 10;

    public GameObject pausePanel;
    public void ReturnToGameMenue()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void TryRestart()
    {
        RestartPanel.SetActive(true);
    }

    public void closeResetPanel()
    {
        RestartPanel.SetActive(false);
    }
    public void Restart()
    {
        // 1️⃣ Prvo probaj da potrošiš jedan život
        if (LifeManager_script.Instance != null)
        {
            bool iskoriscenZivot = LifeManager_script.Instance.TryUseLife();

            // Ako nije uspeo da iskoristi život → nema više života → prikazi elegantan panel
            if (!iskoriscenZivot)
            {
                Debug.Log("Nema više života, prikazujem NoLives panel.");
                ShowNoLivesPanel();
                return;
            }
        }
        else
        {
            Debug.LogWarning("LifeManager_script.Instance je null! (nema LifeManager objekta u sceni)");
            // po želji ovde možeš isto da pošalješ igrača u meni:
            // ReturnToGameMenue();
            // return;
        }

        // 2️⃣ Ako smo ovde stigli, život je uspešno potrošen → restart igre
        var ball = GameObject.Find("Ball");
        if (ball != null)
            ball.GetComponent<BouncingBall>()?.ForceResetDirection();

        Debug.Log("Klik - restart levela, potrošen jedan život.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int getCurrentCoins() { return GameObject.Find("Ball").GetComponent<BouncingBall>().getCoins(); }
    public void OpenPausePanel()
    {
        if( pausePanel != null ) {
        pausePanel.SetActive(true);
      ScoreManager_script.instance.updateCoinOnPausePanel(getCurrentCoins());
        ScoreManager_script.instance.updateScoreOnPausePanel();

        Time.timeScale = 0f;  // pauzira igru
                               }
    }

    public void returntoLevelSelectionMenu()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(0);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Prona?i neaktivan panel po imenu
        string panelName = "LevelSelectionPanel"; // <- Promeni ovo u pravo ime panela
        int cnt = 0;
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name == panelName && go.GetComponent<CanvasRenderer>() != null)
            {
                cnt++;
                // Na?en UI panel (?esto imaju CanvasRenderer)
                go.SetActive(true);
                print("11111111");
                if (cnt == 7) break;
            }
            if ((go.name == "FreeJump" || go.name == "Levels_button" || go.name == "Shop" || go.name == "Infinity Jump" || go.name == "highScore") /*&& go.GetComponent<CanvasRenderer>() != null*/)
            {
                go.SetActive(false);
                cnt++;
                print("22222222");
                if (cnt == 7) break;
            }
        }
    }
    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;  // nastavlja igru
    }

    // ===================== NEMA ZIVOTA (Level scene) =====================

    /// <summary>
    /// Prikazuje elegantan "Nema zivota" panel. Ako panel nije dodeljen,
    /// kao rezervni scenario vraca u glavni meni (staro ponasanje).
    /// </summary>
    private void ShowNoLivesPanel()
    {
        Time.timeScale = 1f;
        if (noLivesPanel != null)
        {
            noLivesPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("noLivesPanel nije dodeljen! Vracam u meni.");
            ReturnToGameMenue();
        }
    }

    /// <summary>
    /// Dugme "Zatvori" na NoLives panelu.
    /// </summary>
    public void CloseNoLivesPanel()
    {
        if (noLivesPanel != null)
            noLivesPanel.SetActive(false);
    }

    /// <summary>
    /// Dugme "Gledaj reklamu za zivot" na NoLives panelu.
    /// </summary>
    public void WatchAdForLife()
    {
        if (rewardedAdsManager != null)
        {
            rewardedAdsManager.ShowRewardedAd(3);
        }
        else
        {
            Debug.LogWarning("RewardedAdsManager nije dodeljen na GameManager-u.");
        }
    }

    /// <summary>
    /// Poziva se iz RewardedAdsManager-a kada je reklama za zivot uspesno odgledana.
    /// Dodaje zivot i restartuje nivo.
    /// </summary>
    public void GrantLifeFromAd()
    {
        if (LifeManager_script.Instance != null)
            LifeManager_script.Instance.BuyLife();

        CloseNoLivesPanel();
        Restart();
    }

    /// <summary>
    /// Dugme "Kupi zivot dijamantima" na NoLives panelu.
    /// </summary>
    public void BuyLifeWithGems()
    {
        int gems = PlayerPrefs.GetInt("SavedDiamonds", 0);
        if (gems >= LIFE_GEM_COST)
        {
            PlayerPrefs.SetInt("SavedDiamonds", gems - LIFE_GEM_COST);
            PlayerPrefs.Save();

            if (LifeManager_script.Instance != null)
                LifeManager_script.Instance.BuyLife();

            CloseNoLivesPanel();
            Restart();
        }
        else
        {
            Debug.Log("Nema dovoljno dijamanata za kupovinu zivota.");
            if (noLivesGemFeedback != null)
                StartCoroutine(FlashGemFeedback());
        }
    }

    private System.Collections.IEnumerator FlashGemFeedback()
    {
        noLivesGemFeedback.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        if (noLivesGemFeedback != null)
            noLivesGemFeedback.gameObject.SetActive(false);
    }
}
