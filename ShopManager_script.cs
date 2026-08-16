using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager_script : MonoBehaviour
{

    public List<Skin> SkinList;
    public GameObject SkinAndButtonPrefab;//prefab koji cemo kasnije preoblikovati za svaki sprite
    public Transform contentPanel;
    [SerializeField] public GameObject MeniScoreManager;

    [SerializeField] private GameObject cointext;
    private void Awake()
    { 
       // PlayerPrefs.SetString("SelektovanSprite", "WhiteBall");
        foreach (var skin in SkinList)
        {
            if (skin.skinName == "WhiteBall") continue;
           // skin.isUnlocked = PlayerPrefs.GetInt("Skin_" + skin.skinName, skin.skinName == "default" ? 1 : 1) == 1;
            skin.isUnlocked = PlayerPrefs.GetInt("Skin_" + skin.skinName, skin.skinName == "default" ? 1 : 0) == 1;
        }
    }
    void Start()
    {

        LoadShopUI();
    }

    private void LoadShopUI()
    {

      //ne koorriistis  int i = 0;
        for(int t=0;t<SkinList.Count;t++)
        {
            var skin = SkinList[t]; 
            GameObject buttonObj = Instantiate(SkinAndButtonPrefab, contentPanel);

            buttonObj.transform.GetChild(0).GetComponent<Image>().sprite = skin.skinSprite;
            buttonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = skin.price.ToString();
            TextMeshProUGUI btnText = buttonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            Skin localSkin = skin;
            GameObject localButton = buttonObj;

            if (skin.isUnlocked)
            {
                string selektovan = PlayerPrefs.GetString("SelektovanSprite", "WhiteBall");
                if (skin.skinName == selektovan)
                {
                    btnText.text = "Selected";
                }
                else
                {
                    btnText.text = "Equip";
                }
            }
            else
            {
                btnText.text = skin.price.ToString();
            }
            Button btn = localButton.GetComponent<Button>();
            btn.onClick.AddListener(() => BuySkin(localSkin, localButton));
            if (!HesmapaSkinova.mojaMapa.ContainsKey(skin.skinName))
            {
                HesmapaSkinova.mojaMapa.Add(skin.skinName, t);
  
            }


        }
    }
    private void UpdateAllButtons()
    { 
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            Transform child = contentPanel.GetChild(i);
            TextMeshProUGUI txt = child.GetChild(1).GetComponent<TextMeshProUGUI>();

            string skinName = SkinList[i].skinName;
            if (SkinList[i].isUnlocked)
            {
                if (PlayerPrefs.GetString("SelektovanSprite") == skinName)
                    txt.text = "Selected";
                else
                    txt.text = "Equip";
            }
            else
            {
                txt.text = SkinList[i].price.ToString();
            }
        }
    }
    private bool vecJeUKorutini = false;
    private void BuySkin(Skin skin, GameObject Button)
    {
        if (skin.isUnlocked)
        {
            SelectSkin(skin);
            Button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Selected";

            return;
        }

        int coins = (PlayerPrefs.GetInt("SavedCoins", 0));
        Debug.Log(coins);
        if (coins >= skin.price)
        {
            int newCoinnumber = coins - skin.price;
            PlayerPrefs.SetInt("SavedCoins", newCoinnumber);
            PlayerPrefs.SetInt("Skin_" + skin.skinName, 1);
            PlayerPrefs.Save();
            MeniScoreManager_script.instance.updateCoin();
            skin.isUnlocked = true;
            Debug.Log("Kupljen");
            Button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Equip";

           
        }
        else
        {
            if (!vecJeUKorutini)
            {
                vecJeUKorutini = true;
                StartCoroutine(FlashTextColor(cointext.GetComponentInChildren<TextMeshProUGUI>(), Color.red, 0.5f));
                
            }
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
            // Shake pozicija: nasumi?ni pomak oko originala
            float offsetX = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            float offsetY = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            text.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null; // ?ekaj slede?i frame
        }

        // Vrati boju i poziciju na original
        text.color = originalColor;
        text.transform.localPosition = originalPos;
        vecJeUKorutini = false;

    }
    void SelectSkin(Skin skin)
    {
        PlayerPrefs.SetString("SelektovanSprite", skin.skinName);
        Debug.Log("Izabran skin: " + skin.skinName);
        UpdateAllButtons();
    }
}
