using TMPro;
using UnityEngine;

public class ScoreManager_script : MonoBehaviour
{
    public static ScoreManager_script instance;
    public int score=0;
    private int highScore;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreOnQuitePaen;
    public TextMeshProUGUI scoreOnPausePanel;
    public TextMeshProUGUI coinTextOnPausePanel;
    public TextMeshProUGUI diamondTextOnPausePanel;
    public TextMeshProUGUI coinTextOnQuitePanel;
    public TextMeshProUGUI diamondTextOnQuitePanel;
    public TextMeshProUGUI ukupnoCoina;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    string savedCoins;
    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        savedCoins = PlayerPrefs.GetInt("SavedCoins", 0).ToString() ;
        scoreText.text = "0";
     
        highScore = PlayerPrefs.GetInt("HighScore", 0);

    }
    
    public void updateScore(int score)
    {
        int scoree = 1;
        this.score += scoree;
        updateScoreDisplay();
    }
    public void updateScoreOnQuitePanel()
    {
        scoreOnQuitePaen.text = "Score: " + scoreText.text;
    }

    public void updateScoreOnPausePanel()
    {
        scoreOnPausePanel.text = "Score: " + scoreText.text;
    }
    public float pomerajScora = 50f;
    private void pomeriScoreZbogZapisa()
    {
       // Debug.Log("POm");
        Vector3 pos = scoreText.transform.position;
        pos.x = pos.x - pomerajScora;
        scoreText.transform.position = pos;
    }
    public void updateScoreDisplay()
    {
       /* if (score == 10 || score==100 || score==1000 || score==10000 )
            pomeriScoreZbogZapisa();
     */
       scoreText.text = score.ToString();
    }
    
    public void updateDiamondOnQuitePanel(int cnt, int i = 0)
    {
        //cnt = 100; 
        diamondTextOnQuitePanel.text = cnt.ToString();
        if (i == 1) { updateSavedDiamond(cnt); diamondTextOnQuitePanel.text = (cnt * 2).ToString(); }
        else updateSavedDiamond(cnt);
    }
    public void updateCoinOnQuitePanel(int cnt,int i=0)
    {
        coinTextOnQuitePanel.text = cnt.ToString();
        if (i == 1) { updateSavedCoins(cnt); coinTextOnQuitePanel.text = (cnt * 2).ToString(); }
        else updateSavedCoins(cnt);
    }
    public void updateCoinOnPausePanel(int cnt)
    {
        int d = cnt;
        coinTextOnPausePanel.text = d.ToString();
        //updateSavedCoins();
    }
    public void updateDiamondOnPausePanel(int cnt)
    {
        int d = cnt;
        diamondTextOnPausePanel.text = d.ToString();
        //updateSavedCoins();
    }

    public void updateSavedCoins(int cnt)
    {
        int a = cnt + PlayerPrefs.GetInt("SavedCoins", 0);///bez +10
        PlayerPrefs.SetInt("SavedCoins",a);
        PlayerPrefs.Save();

    }
    public void updateSavedDiamond(int cnt)
    {
        int a = cnt + PlayerPrefs.GetInt("SavedDiamonds", 0);///bez +10
        PlayerPrefs.SetInt("SavedDiamonds", a);
        PlayerPrefs.Save();

    }
    
    public void postaviNoviHighScore()//ako treba postavlja novi highh score
    {
        if (score > highScore) {
            PlayerPrefs.SetInt("HighScore", score);

            PlayerPrefs.Save(); 
            highScore = score;
        }
    }
   
}
