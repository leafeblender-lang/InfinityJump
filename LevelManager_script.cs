using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager_script : MonoBehaviour
{
    public static LevelManager_script Instance;

    // Lista pre?enih nivoa
   // public HashSet<int> completedLevels = new HashSet<int>();

    void Awake()
    {
      /*
        PlayerPrefs.DeleteKey("SemiUnlocedLevels");
        PlayerPrefs.GetInt("SemiUnlocedLevels", 1);
        PlayerPrefs.Save();
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.GetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();*/

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ne uništava se pri u?itavanju novih scena
        }
        else
        {
            Destroy(gameObject); // Uništi duplikate
        }
    }

    public void levelCompleted(int levelIndex)
    {
        int currentUnlocked = PlayerPrefs.GetInt("SemiUnlocedLevels", 1);
        int nextLevel = levelIndex + 1;

        // Otkljucaj samo sledeci nivo ako je veci od trenutno otkljucanih
        if (nextLevel > currentUnlocked)
        {
            PlayerPrefs.SetInt("SemiUnlocedLevels", nextLevel);
            Debug.Log("Otkljucan nivo: " + nextLevel);
        }
        else
        {
            Debug.Log("Nivo " + nextLevel + " je vec otkljucan. Trenutno otkljucano: " + currentUnlocked);
        }

        PlayerPrefs.Save();
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
                if(cnt==6) break;
            }
            if((go.name == "FreeJump" || go.name == "Levels_button" || go.name == "Shop" || go.name == "InfinityJump" || go.name == "highScore") /*&& go.GetComponent<CanvasRenderer>() != null*/)
            {
                go.SetActive(false);
                cnt++;
                print("22222222");
                if (cnt == 6) break;
            }
        }

        // Odjavi event da se ne poziva više puta
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

  
}
