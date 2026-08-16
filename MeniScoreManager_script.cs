using UnityEngine;
using TMPro;

public class MeniScoreManager_script : MonoBehaviour
{
    public static MeniScoreManager_script instance;

    [Header("Main Menu UI")]
    [SerializeField] private TextMeshProUGUI savedCoins;
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private TextMeshProUGUI savedDiamonds;

    [Header("Shop UI")]
    [SerializeField] private TextMeshProUGUI shopCoins;
    [SerializeField] private TextMeshProUGUI shopDiamonds;

    private int hs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        updateCoin();

        if (highScore != null)
        {
            highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
    }

    public void updateCoin()
    {
        int coins = PlayerPrefs.GetInt("SavedCoins", 0);
        int diamonds = PlayerPrefs.GetInt("SavedDiamonds", 0);

        // Main Menu
        if (savedCoins != null)
            savedCoins.text = coins.ToString();

        if (savedDiamonds != null)
            savedDiamonds.text = diamonds.ToString();

        // Shop
        if (shopCoins != null)
            shopCoins.text = coins.ToString();

        if (shopDiamonds != null)
            shopDiamonds.text = diamonds.ToString();
    }

    public void updateCoinOnQuitePanel(int cnt)
    {
        int currentCoins = PlayerPrefs.GetInt("SavedCoins", 0);
        int newCoins = currentCoins + cnt;

        PlayerPrefs.SetInt("SavedCoins", newCoins);
        PlayerPrefs.Save();

        // Osvezi sve prikaze coins-a i diamonds-a
        updateCoin();

        Debug.Log($"💰 Dodato {cnt} coins. Ukupno: {newCoins}");
    }

    public void updateDiamondOnQuitePanel(int cnt)
    {
        int currentDiamonds = PlayerPrefs.GetInt("SavedDiamonds", 0);

        // Ako +10 nije namerno, ostavi samo + cnt
        int newDiamonds = currentDiamonds + cnt;

        PlayerPrefs.SetInt("SavedDiamonds", newDiamonds);
        PlayerPrefs.Save();

        // Osvezi sve prikaze coins-a i diamonds-a
        updateCoin();

        Debug.Log($"💎 Dodato {cnt} diamonds. Ukupno: {newDiamonds}");
    }
}