using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class ConsentManager : MonoBehaviour
{
    [SerializeField] private GameObject consentPanel;  // Referenca na tvoj UI panel

    void Start()
    {
        // Proveri da li je korisnik ve? dao saglasnost ranije
        if (PlayerPrefs.HasKey("PersonalizedAds"))
        {
            consentPanel.SetActive(false);
            InitializeAds();
        }
        else
        {
            consentPanel.SetActive(true); // Prikaži panel ako još nema izbora
        }
    }

    public void OnConsentYes()
    {
        PlayerPrefs.SetInt("PersonalizedAds", 1);
        PlayerPrefs.Save();

        consentPanel.SetActive(false);
        InitializeAds();
    }

    public void OnConsentNo()
    {
        PlayerPrefs.SetInt("PersonalizedAds", 0);
        PlayerPrefs.Save();

        consentPanel.SetActive(false);
        InitializeAds();
    }
    public static AdRequest GetAdRequest()
    {
        AdRequest request = new AdRequest();

        // Ako je korisnik rekao NE za personalizovane reklame, dodaj parametar za nepersonalizovane oglase
        if (PlayerPrefs.HasKey("PersonalizedAds") && PlayerPrefs.GetInt("PersonalizedAds") == 0)
        {
            var extras = new Dictionary<string, string>() { { "npa", "1" } };
            foreach (var kvp in extras)
            {
                request.Extras.Add(kvp.Key, kvp.Value);
            }
        }

        return request;
    }


    private void InitializeAds()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
            // Ovdje možeš odmah u?itati oglase ako želiš
        });
    }
}
