using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Kontroler Game Over panela za nivoe (Level1..LevelN).
/// FreePlay i dalje koristi svoj Watch-Ad / QuitePanel tok.
/// </summary>
public class LevelGameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;      // koren Game Over panela (neaktivan po defaultu)
    [SerializeField] private TextMeshProUGUI scoreText; // prikaz rezultata
    [SerializeField] private TextMeshProUGUI coinText;  // prikaz sakupljenih novcica
    [SerializeField] private TextMeshProUGUI diamondText;// prikaz sakupljenih dijamanata

    /// <summary>
    /// Prikazuje Game Over panel sa rezultatima.
    /// </summary>
    public void Show(int score, int coins, int diamonds)
    {
        Time.timeScale = 1f;
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (coinText != null) coinText.text = coins.ToString();
        if (diamondText != null) diamondText.text = diamonds.ToString();
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    /// <summary>
    /// Dugme Restart na Game Over panelu. Prolazi kroz GameManager
    /// koji trosi zivot; ako nema zivota prikazuje "Nema zivota" panel.
    /// </summary>
    public void OnRestart()
    {
        GameManager_script gm = FindObjectOfType<GameManager_script>();
        if (gm != null)
        {
            gm.Restart();
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// Dugme Exit na Game Over panelu -> glavni meni.
    /// </summary>
    public void OnExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
