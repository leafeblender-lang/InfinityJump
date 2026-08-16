using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyOfferController : MonoBehaviour
{
    [Header("UI")]

    // MysteryBox GameObject treba da ima i Image i Button komponentu.
    // U Inspectoru ovde prevlacis samo MysteryBox.
    [SerializeField] private Image mysteryBoxImage;

    // Tekst koji prikazuje "CLAIM" ili "COMING SOON".
    [SerializeField] private TextMeshProUGUI statusText;

    // Tekst "New offer in:".
    [SerializeField] private TextMeshProUGUI newOfferText;

    // Countdown, npr. 05:42:18.
    [SerializeField] private TextMeshProUGUI timerText;


    [Header("Cooldown")]

    // Koliko sati mora da prodje nakon preuzimanja nagrade
    // pre nego sto Daily Offer ponovo postane dostupan.
    [SerializeField] private int cooldownHours = 6;


    [Header("Pulse")]

    // Maksimalno povecanje MysteryBox-a tokom pulsiranja.
    // 1.08 znaci da ce biti maksimalno 8% veci.
    [SerializeField] private float pulseScale = 1.08f;

    // Brzina pulsiranja.
    [SerializeField] private float pulseSpeed = 2.5f;


    [Header("Unavailable izgled")]

    // Boja MysteryBox-a dok offer nije dostupan.
    // Image Color mnozi originalne boje sprite-a,
    // pa ce ovako kutija izgledati sivlje i tamnije.
    [SerializeField]
    private Color unavailableColor =
        new Color(0.55f, 0.55f, 0.55f, 1f);


    // Button uzimamo automatski sa istog MysteryBox objekta.
    private Button mysteryBoxButton;

    // Originalna velicina MysteryBox-a.
    private Vector3 originalScale;

    // Korutina koja pravi pulse efekat.
    private Coroutine pulseCoroutine;

    // Da li trenutno moze da se pokupi nagrada.
    private bool canClaim;

    // Ovde cuvamo vreme kada sledeci offer postaje dostupan.
    private const string NEXT_DAILY_OFFER_KEY =
        "NextDailyOfferTime";


    private void Awake()
    {
        // Button se automatski uzima sa istog objekta
        // na kojem se nalazi mysteryBoxImage.
        mysteryBoxButton =
            mysteryBoxImage.GetComponent<Button>();

        if (mysteryBoxButton == null)
        {
            Debug.LogError(
                "MysteryBox nema Button komponentu!"
            );

            return;
        }

        // Pamtimo njegovu normalnu velicinu.
        originalScale =
            mysteryBoxImage.transform.localScale;

        // Kada se klikne MysteryBox,
        // pokusaj preuzimanja Daily Offer-a.
        mysteryBoxButton.onClick.AddListener(
            ClaimDailyOffer
        );
    }


    private void OnEnable()
    {
        // Cim se Shop/DailyOffer ukljuci,
        // odmah proveri trenutno stanje.
        UpdateOfferState();
    }


    private void Update()
    {
        // Osvezava countdown dok je ovaj objekat aktivan.
        UpdateOfferState();
    }


    private void UpdateOfferState()
    {
        long currentTime = GetCurrentUnixTime();
        long nextOfferTime = GetNextOfferTime();

        // Ako offer nikada nije preuzet
        // ILI je cooldown istekao...
        if (nextOfferTime == 0 ||
            currentTime >= nextOfferTime)
        {
            SetAvailableState();
        }
        else
        {
            long remainingSeconds =
                nextOfferTime - currentTime;

            SetCooldownState(remainingSeconds);
        }
    }


    private void SetAvailableState()
    {
        canClaim = true;

        // MysteryBox moze da se klikne.
        mysteryBoxButton.interactable = true;

        // Vrati originalne boje sprite-a.
        mysteryBoxImage.color = Color.white;

        // Ako vec ne pulsira, pokreni pulsiranje.
        if (pulseCoroutine == null)
        {
            StartPulse();
        }

        statusText.text = "CLAIM";

        // Nema potrebe za "New offer in:"
        // dok je nagrada spremna.
        newOfferText.gameObject.SetActive(false);

        timerText.text = "READY!";
    }


    private void SetCooldownState(long remainingSeconds)
    {
        canClaim = false;

        // Ne dozvoli klik dok traje cooldown.
        mysteryBoxButton.interactable = false;

        // Zaustavi pulsiranje.
        StopPulse();

        // MysteryBox postaje sivlji/tamniji.
        mysteryBoxImage.color = unavailableColor;

        statusText.text = "COMING SOON";

        newOfferText.gameObject.SetActive(true);
        newOfferText.text = "New offer in:";


        // Pretvaramo preostale sekunde
        // u sate, minute i sekunde.
        TimeSpan remainingTime =
            TimeSpan.FromSeconds(remainingSeconds);

        int hours =
            (int)remainingTime.TotalHours;


        timerText.text =
            $"{hours:00}:" +
            $"{remainingTime.Minutes:00}:" +
            $"{remainingTime.Seconds:00}";
    }


    private void ClaimDailyOffer()
    {
        // Dodatna zastita.
        if (!canClaim)
            return;


        Debug.Log("Daily Offer preuzet!");


        // ==========================================
        // OVDE KASNIJE DODAJES PRAVU NAGRADU
        // ==========================================

        // Na primer 500 coins:
        //
        // MeniScoreManager_script.instance
        //     .updateCoinOnQuitePanel(500);


        // Zapamti trenutno vreme.
        long currentTime =
            GetCurrentUnixTime();

        // Izracunaj kada ce offer ponovo biti dostupan.
        long nextOfferTime =
            currentTime +
            (cooldownHours * 60L * 60L);

        // Sacuvaj vreme.
        PlayerPrefs.SetString(
            NEXT_DAILY_OFFER_KEY,
            nextOfferTime.ToString()
        );

        PlayerPrefs.Save();

        // Odmah prebaci UI u cooldown stanje.
        UpdateOfferState();
    }


    private void StartPulse()
    {
        if (pulseCoroutine != null)
            return;

        pulseCoroutine =
            StartCoroutine(PulseRoutine());
    }


    private void StopPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        // Obavezno vrati normalnu velicinu
        // kada prestane pulsiranje.
        mysteryBoxImage.transform.localScale =
            originalScale;
    }


    private IEnumerator PulseRoutine()
    {
        while (true)
        {
            // Sin funkcija pravi glatko:
            // malo -> veliko -> malo -> veliko...
            float pulse =
                (Mathf.Sin(
                    Time.unscaledTime * pulseSpeed
                ) + 1f) / 2f;

            float scale =
                Mathf.Lerp(
                    1f,
                    pulseScale,
                    pulse
                );

            mysteryBoxImage.transform.localScale =
                originalScale * scale;

            yield return null;
        }
    }


    private long GetCurrentUnixTime()
    {
        // Pravo UTC vreme.
        // Zato cooldown nastavlja da tece
        // i kada igrac ugasi igru.
        return DateTimeOffset.UtcNow
            .ToUnixTimeSeconds();
    }


    private long GetNextOfferTime()
    {
        // Ako korisnik nikada nije preuzeo offer,
        // odmah je dostupan.
        if (!PlayerPrefs.HasKey(
            NEXT_DAILY_OFFER_KEY))
        {
            return 0;
        }


        string savedTime =
            PlayerPrefs.GetString(
                NEXT_DAILY_OFFER_KEY,
                "0"
            );


        if (long.TryParse(
            savedTime,
            out long result))
        {
            return result;
        }

        return 0;
    }


    private void OnDisable()
    {
        // Kad zatvoris Shop,
        // zaustavi pulse da nepotrebno ne radi.
        StopPulse();
    }
}