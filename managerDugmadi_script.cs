using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System;
public class managerDugmadi_script : MonoBehaviour
{
    [SerializeField] GameObject ScenaBiranjaLevela;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject dugmeInfo;
    [SerializeField] private GameObject dugmeSett;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject panelReferisanja;

    [SerializeField] private GameObject panelPreporuke;
    [SerializeField] private GameObject LevelPanel;
    [SerializeField] private GameObject dugme1;
    [SerializeField] private GameObject dugme2;
    [SerializeField] private GameObject dugme3;
    [SerializeField] private GameObject tekst;
    
    [SerializeField] private GameObject tekst2;
    
    [SerializeField] private GameObject diamondtext;
    [SerializeField] private GameObject OutOfLifePanel;
    [SerializeField] private GameObject prikazMenijaObj;

    [SerializeField] private GameObject scoreManager;
    private bool vecJeUKorutini1=false;//korutina za treskanje katanca
    private bool vecJeUKorutini2 = false;//korutina za treskanje dijamanata ako nemas

    /// <summary>
    [SerializeField] private TextMeshProUGUI codeText;   // ✅ DODAJ OVO gore sa ostalim SerializeField-ovima
    public Button copyButton;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI originalCopytekst;
    public float feedbackDuration = 1.5f;
    private void Start()
    {
        feedbackText.gameObject.SetActive(false);
        Referisanje referisanjeScript = FindObjectOfType<Referisanje>();

        if (referisanjeScript != null)
        {
            // 🆕 Sačekaj da se korisnik učita, PA ONDA postavi kod
            referisanjeScript.OnUserLoaded += () =>
            {
                codeText.text = referisanjeScript.GetMyCode();
                Debug.Log("✅ Kod postavljen: " + codeText.text);
            };
        }
        else
        {
            Debug.LogWarning("Ne mogu da nađem Referisanje skriptu!");
        }

        copyButton.onClick.AddListener(CopyCode);
    }

    public void CopyCode()
    {
        if (codeText != null)
        {
            string code = codeText.text;

#if UNITY_ANDROID && !UNITY_EDITOR
            // Android clipboard kopiranje
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject clipboardManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "clipboard");
            
            AndroidJavaClass clipDataClass = new AndroidJavaClass("android.content.ClipData");
            AndroidJavaObject clipData = clipDataClass.CallStatic<AndroidJavaObject>("newPlainText", "Referral Code", code);
            clipboardManager.Call("setPrimaryClip", clipData);
            
            Debug.Log("✅ Kopirano na Android: " + code);
            
#elif UNITY_IOS && !UNITY_EDITOR
            // iOS clipboard kopiranje
            GUIUtility.systemCopyBuffer = code;
            Debug.Log("✅ Kopirano na iOS: " + code);
            
#else
            // Desktop/Editor
            GUIUtility.systemCopyBuffer = code;
            Debug.Log("✅ Kopirano (Desktop): " + code);
#endif

            // Prikaži "Copied!" feedback
            StopAllCoroutines();
            StartCoroutine(ShowFeedback());
        }
    }
    private IEnumerator ShowFeedback()
    {
        feedbackText.gameObject.SetActive(true);
        originalCopytekst.gameObject.SetActive(false);
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.gameObject.SetActive(false);

        originalCopytekst.gameObject.SetActive(true);
    }
    /// </summary>

    public void onClickBack()
    {
        ScenaBiranjaLevela.SetActive(false);
    }

    public void ChooseLevel()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        if (clickedObject == null) return;

        Button clickedButton = clickedObject.GetComponent<Button>();
        if (clickedButton == null) return;

        Debug.Log("Kliknuto dugme: " + clickedButton.name);

        // Pronađi ikonice po imenu
        Transform openIcon = clickedButton.transform.Find("OpenLevel");
        Transform semiLockIcon = clickedButton.transform.Find("SemiLock");
        Transform lockIcon = clickedButton.transform.Find("LockedLevel");

        if (openIcon != null && openIcon.gameObject.activeSelf)
        {
            Debug.Log("uso1");
       
            // Pronađi dete sa tekstom unutar OpenLevel
            TextMeshProUGUI levelText = openIcon.GetComponentInChildren<TextMeshProUGUI>();
            if (levelText != null)
            {
                Debug.Log("uso2");
                string levelNumber = levelText.text.Trim();
                string levelName = "Level" + levelNumber;
                Debug.Log("Učitavam scenu: " + levelName);
                bool uspeh = LifeManager_script.Instance.TryUseLife();
                if (uspeh)
                {
                    Debug.Log("Život uspešno potrošen, učitavam nivo.");
                    SceneManager.LoadScene(levelName);
                }
                else
                {
                    OutOfLifePanel.SetActive(true);
                    statiziraj.Instance.VratiTransparentnost();
                    // Ovde možeš dodati prikaz poruke igraču ili neki UI feedback
                }
               // SceneManager.LoadScene(levelName);
            }
            else
            {
                Debug.LogWarning("Nije pronađen tekst sa brojem nivoa.");
            }
        }
        else if (semiLockIcon != null && semiLockIcon.gameObject.activeSelf)
        {
            TextMeshProUGUI cenaText = semiLockIcon.GetComponentInChildren<TextMeshProUGUI>();
            int cena = int.Parse(cenaText.text.Trim());

            Debug.Log("Nivo je poluotključan. Prikaži preview.");
            if (PlayerPrefs.GetInt("SavedDiamonds", 999) < cena)
            {
                if (!vecJeUKorutini2)
                { vecJeUKorutini2 = true;
                    StartCoroutine(FlashTextColor(diamondtext.GetComponentInChildren<TextMeshProUGUI>(), Color.red, 0.5f)); }

            }
            else
            {
                int i = PlayerPrefs.GetInt("SavedDiamonds");
                MeniScoreManager_script.instance.updateDiamondOnQuitePanel(-cena);
                int newUnlock= PlayerPrefs.GetInt("UnlockedLevel", 1);
                newUnlock++;
                PlayerPrefs.SetInt("UnlockedLevel", newUnlock);
                semiLockIcon.gameObject.SetActive(false);
                openIcon.gameObject.SetActive(true);
                PlayerPrefs.Save();
            }
        }
        else if (lockIcon != null && lockIcon.gameObject.activeSelf)
        {
            Debug.Log("Nivo je zaključan. Ne može se igrati.");

            // Pronađi dete "Locked" unutar "LockedLevel"
            Transform lockedImageTransform = lockIcon.transform.Find("Lock");
            if (lockedImageTransform != null)
            {
                Debug.Log("uso1");
                Image lockedImage = lockedImageTransform.GetComponent<Image>();

                if (lockedImage != null)
                {
                    Debug.Log("uso2");
                    if (!vecJeUKorutini1)
                    { StartCoroutine(FlashLockedImage(lockedImage, Color.red, 0.15f)); }
                }
            }
            else
            {
                Debug.LogWarning("Nije pronađen 'Locked' Image unutar 'LockedLevel'.");
            }

        }
        else
        {
            Debug.LogWarning("Nijedna ikonica nije aktivna — nešto nije u redu.");
        }
    }


    public void BuyLifeWithDiamond(GameObject cenaObj)
    {
        TextMeshProUGUI cenaTMP = cenaObj.GetComponent<TextMeshProUGUI>();
        int cenaInt;
        if (int.TryParse(cenaTMP.text, out cenaInt))
        {
            int wallet = PlayerPrefs.GetInt("SavedDiamonds", 0);

            if (cenaInt <= wallet)
            {
                wallet -= cenaInt;
                PlayerPrefs.SetInt("SavedDiamonds", wallet);
                PlayerPrefs.Save();
                scoreManager.GetComponent<MeniScoreManager_script>().updateCoin();
                Debug.Log("Kupovina uspešna, preostalo: " + wallet);
                LifeManager_script.Instance.BuyLife();
                prikazMenijaObj.GetComponent<prikazMaina>().changeColorOfHearts();
                CloseOutOfLifePanel();
            }
            else
            {
                if (!vecJeUKorutini2)
                {
                    vecJeUKorutini2 = true;
                    StartCoroutine(FlashTextColor(diamondtext.GetComponentInChildren<TextMeshProUGUI>(), Color.red, 0.5f));
                }
            }
        }
        else
        {
            Debug.LogError("Cena nije validan broj: " + cenaTMP.text);
        }

    }
    private IEnumerator FlashTextColor(TextMeshProUGUI text, Color flashColor, float duration)
    {
        
        Color originalColor = text.color;
        Vector3 originalPos = text.transform.localPosition;
        float shakeAmount = 1f;
        text.color = flashColor;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Shake pozicija: nasumični pomak oko originala
            float offsetX = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            float offsetY = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            text.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null; // čekaj sledeći frame
        }

        // Vrati boju i poziciju na original
        text.color = originalColor;
        text.transform.localPosition = originalPos;
        vecJeUKorutini2 = false;

    }

    public void CloseOutOfLifePanel()
    {
        statiziraj.Instance.NulirajTransparentnost();
        OutOfLifePanel.SetActive(false);
    }
    private IEnumerator FlashLockedImage(Image img, Color flashColor, float duration)
    {
        vecJeUKorutini1 = true;
        Color originalColor = img.color;
        Vector3 originalPos = img.transform.localPosition;
        float shakeAmount = 1f;
        img.color = flashColor;
        float shakeFrequency = 35f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Pomeri poziciju na random mesto u malom opsegu (shakeAmount u pikselima)
            float offsetX = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            float offsetY = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            img.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;

            // Ovo kontroliše koliko često se pomera (shakeFrequency puta u sekundi)
            yield return new WaitForSeconds(1f / shakeFrequency);
           
        }
        // Vrati poziciju i boju nazad
        img.transform.localPosition = originalPos;
        img.color = originalColor;

        vecJeUKorutini1 = false;
    }

    public void openLevelPanel()
    {
        tekst2.SetActive(false);
        tekst.SetActive(false);
        dugme1.SetActive(false);
        dugme2.SetActive(false);
        dugme3.SetActive(false);
       
        LevelPanel.SetActive(true);


    }

   
   
    public void closeLevelPanel()
    {
        tekst2.SetActive(true);
        tekst.SetActive(true);
        dugme1.SetActive(true);
        dugme2.SetActive(true);
        dugme3.SetActive(true);

        LevelPanel.SetActive(false);
    }
    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void OpenPanelReferisanja()
    {

        statiziraj.Instance.NulirajTransparentnost();
        panelReferisanja.SetActive(true);

    }
    public void ClosePanelReferisanja()
    {
        panelReferisanja.SetActive(false);

    }
    public void OpenpanelPreporuke()
    {
        panelPreporuke.SetActive(true);
    }
    public void ClosepanelPreporuke()
    {
        panelPreporuke.SetActive(false);

    }
    
    public void ExitShop()
    {
        dugmeInfo.SetActive(true);
        dugmeSett.SetActive(true);

        shopPanel.SetActive(false);  
    }
    public void EnterShop()
    {
        shopPanel.SetActive(true);
        
        dugmeInfo.SetActive(false); 
        dugmeSett.SetActive(false); 


    }
    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;  // nastavlja igru
    }
    public void onClickLevels()
    {
        ScenaBiranjaLevela.SetActive(true);
    }
    public void ucitajFreePlay()
    {
        SceneManager.LoadScene(1);
    }
}
