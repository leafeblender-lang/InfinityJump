using UnityEngine;
using UnityEngine.UI;

public class LevelMeni : MonoBehaviour
{
    private Button[] buttons;

    private void Start()
    {
        renderujLevelPane();
    }

    public void renderujLevelPane()
    {

        // Pribavi sve dugmi?e
        buttons = GetComponentsInChildren<Button>();

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int semiUnlockedLevels = PlayerPrefs.GetInt("SemiUnlocedLevels", 1);

        for (int i = 0; i < buttons.Length; i++)
        {

            Button btn = buttons[i];

            Transform openIcon = btn.transform.Find("OpenLevel");
            Transform semiLockIcon = btn.transform.Find("SemiLock");
            Transform lockIcon = btn.transform.Find("LockedLevel");
            if (openIcon == null && semiLockIcon == null && lockIcon == null)
            {
                continue;
            }
            if (i < unlockedLevel)
            {
                btn.interactable = true;
                openIcon.gameObject.SetActive(true);
                semiLockIcon.gameObject.SetActive(false);
                lockIcon.gameObject.SetActive(false);
            }
            else if (i < semiUnlockedLevels)
            {
                btn.interactable = true;
                openIcon.gameObject.SetActive(false);
                semiLockIcon.gameObject.SetActive(true);
                lockIcon.gameObject.SetActive(false);
            }
            else
            {
                btn.interactable = true;
                openIcon.gameObject.SetActive(false);
                semiLockIcon.gameObject.SetActive(false);
                lockIcon.gameObject.SetActive(true);
            }
        }

    }

}
