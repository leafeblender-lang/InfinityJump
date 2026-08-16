using UnityEngine;
using UnityEngine.UI;

public class prikazMaina : MonoBehaviour
{
    [SerializeField] private GameObject life1;
    [SerializeField] private GameObject life2;
    [SerializeField] private GameObject life3;
    [SerializeField] private GameObject BuyLifePanel;
    [SerializeField] private GameObject managerDugmadi;
    private void Start()
    {
        LifeManager_script.Instance.UpdateLives();
        changeColorOfHearts();
    }

    public void changeColorOfHearts()
    {
        Debug.Log("Usao u prommenu boje");
        LifeManager_script.Instance.UpdateLives();
        int cntActiveLifes = LifeManager_script.Instance.GetCurrentLives();
        Image sl1 = life1.GetComponent<Image>();
        Image sl2 = life2.GetComponent<Image>();
        Image sl3 = life3.GetComponent<Image>();

        // Definiši boje koje želiš
        Color inactiveColor;
        Color activeColor;

        // Convertuj heks string u Color (RGBA vrednosti od 0 do 1)
        ColorUtility.TryParseHtmlString("#795353", out inactiveColor); // tamnija
        ColorUtility.TryParseHtmlString("#FF8989", out activeColor);   // svetlija
        switch (cntActiveLifes)
        {
            case 0:
                Debug.Log("0");
                sl1.color = inactiveColor;
                sl2.color = inactiveColor;
                sl3.color = inactiveColor;
                break;
            case 1:
                Debug.Log("1");
                sl1.color = activeColor;
                sl2.color = inactiveColor;
                sl3.color = inactiveColor;
                break;
            case 2:
                Debug.Log("2");
                sl1.color = activeColor;
                sl2.color = activeColor;
                sl3.color = inactiveColor;
                break;
            case 3:
                Debug.Log("3");
                sl1.color = activeColor;
                sl2.color = activeColor;
                sl3.color = activeColor;
                break;

        }
        if (BuyLifePanel.activeSelf)managerDugmadi.GetComponent<managerDugmadi_script>().CloseOutOfLifePanel();

    }


}
