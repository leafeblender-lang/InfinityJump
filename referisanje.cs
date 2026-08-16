using Postgrest.Exceptions;
using Supabase.Realtime;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Supabase.Realtime.PostgresChanges.PostgresChangesOptions;

public class Referisanje : MonoBehaviour
{
    [SerializeField] public TMP_InputField referralInput;

    private Supabase.Client supabase;
    private User currentUser;
    private RealtimeChannel channel;

    public event Action OnUserLoaded;

    private async void Start()
    {
        Debug.Log("Referral sistem je trenutno iskljucen.");
        return;

        supabase = new Supabase.Client(
            "https://swozvfntfinssraolgzl.supabase.co",
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InN3b3p2Zm50Zmluc3NyYW9sZ3psIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjEzMDcyODQsImV4cCI6MjA3Njg4MzI4NH0.svgEi7FYvyJww5KoEOLByZIGr_RrvIM5-_hIks6qhVk"
        );

        await supabase.InitializeAsync();
        Debug.Log("? Supabase Client radi!");

        // ? Poveži Realtime socket
        await ConnectRealtime();

        await CheckOrCreateUser();
        OnUserLoaded?.Invoke();

        // ? Subscribe na promene
        await SubscribeToUserChanges();
    }

    // ? Eksplicitna konekcija na Realtime
    private async Task ConnectRealtime()
    {
        try
        {
            Debug.Log("?? Povezujem Realtime...");
            await supabase.Realtime.ConnectAsync();
            Debug.Log("? Realtime socket povezan!");
        }
        catch (Exception e)
        {
            Debug.LogError($"? Greška pri povezivanju Realtime: {e.Message}");
        }
    }

    // ? Promenjeno u async Task
    private async Task SubscribeToUserChanges()
    {
        if (currentUser == null) return;

        try
        {
            channel = await supabase
                .From<User>()
                .On(ListenType.Updates, (sender, change) =>
                {
                    var updatedUser = change?.Model<User>();

                    if (updatedUser?.id == currentUser.id)
                    {
                        Debug.Log($"?? UPDATE detektovan za: {updatedUser.Username}");

                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            HandleDatabaseUpdate();
                        });
                    }
                });

            Debug.Log($"? Realtime listener aktivan za korisnika: {currentUser.Username}");
        }
        catch (Exception e)
        {
            Debug.LogError($"? Greška pri pokretanju Realtime: {e.Message}");
        }
    }

    private async void HandleDatabaseUpdate()
    {
        try
        {
            Debug.Log("?? Osvježavam podatke iz baze...");

            var result = await supabase.From<User>()
                .Filter("id", Postgrest.Constants.Operator.Equals, currentUser.id)
                .Single();

            if (result != null)
            {
                int oldDiamonds = currentUser.Diamonds;
                currentUser.Diamonds = result.Diamonds;

                if (currentUser.Diamonds > oldDiamonds)
                {
                    int gained = currentUser.Diamonds - oldDiamonds;
                    Debug.Log($"?? DOBIO SI +{gained} DIJAMANTA!");
                    NagradaA();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"? Greška u HandleDatabaseUpdate: {e.Message}");
        }
    }

    private async Task CheckOrCreateUser()
    {
        var result = await supabase.From<User>()
            .Filter("device_id", Postgrest.Constants.Operator.Equals, SystemInfo.deviceUniqueIdentifier)
            .Get();

        currentUser = result.Models.Count > 0 ? result.Models[0] : null;

        if (currentUser != null)
        {
            Debug.Log("Korisnik ve? postoji: " + currentUser.Username);
        }
        else
        {
            currentUser = new User
            {
                Username = "Player" + UnityEngine.Random.Range(1000, 9999),
                ReferralCode = GenerateReferralCode(),
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Diamonds = 0
            };

            var response = await supabase.From<User>().Insert(currentUser);
            currentUser = response.Models[0];

            Debug.Log("Novi korisnik kreiran: " + currentUser.Username);
        }
    }

    private string GenerateReferralCode()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        int randomNum = UnityEngine.Random.Range(100, 1000);
        return deviceId.Substring(0, 6) + randomNum;
    }

    public string GetMyCode()
    {
        return currentUser?.ReferralCode ?? "";
    }
    // ? Ova metoda se poziva kad klikneš dugme
    public async void OnApplyButtonClick()
    {
        string code = referralInput.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            Debug.Log("? Unesi kod!");
            return;
        }

        Debug.Log($"?? Proveravam kod: {code}");

        bool success = await ApplyReferralCode(code);

        if (success)
        {
            // ? Uspeh - obriši input polje
            referralInput.text = "";
            Debug.Log("? Kod uspešno primenjen!");
            GameObject.Find("ManagerDugmadi").GetComponent<managerDugmadi_script>().ClosepanelPreporuke();
        }
        else
        {
            StartCoroutine(FlashInputRed());
        }
    }
    private IEnumerator FlashInputRed()
    {
        // Sa?uvaj originalnu boju
        Color originalColor = referralInput.image.color;

        // Postavi crvenu boju
        referralInput.image.color = new Color(1f, 0.3f, 0.3f, 1f); // Svetlo crvena

        // Sa?ekaj 1 sekundu
        yield return new WaitForSeconds(1f);

        // Vrati originalnu boju
        referralInput.image.color = originalColor;
    }

    public async Task<bool> ApplyReferralCode(string code)
    {
        if (code == currentUser.ReferralCode)
        {
            Debug.Log("? Ne možeš koristiti svoj kod!");
            return false;
        }

        var referrerResult = await supabase.From<User>()
            .Filter("referral_code", Postgrest.Constants.Operator.Equals, code)
            .Get();

        if (referrerResult.Models.Count == 0)
        {
            Debug.Log("? Kod ne postoji");
            return false;
        }

        var referrer = referrerResult.Models[0];

        var userPair = currentUser.id.CompareTo(referrer.id) < 0 ? currentUser.id + "-" + referrer.id : referrer.id + "-" + currentUser.id;

        var existing = await supabase.From<Referral>()
            .Filter("user_pair", Postgrest.Constants.Operator.Equals, userPair)
            .Get();

        if (existing.Models.Count > 0)
        {
            Debug.Log("? Ve? ste koristili kod ovog korisnika");
            return false;
        }

        var newReferral = new Referral
        {
            referrer_id = referrer.id,
            referred_id = currentUser.id
        };
        try
        {
            await supabase.From<Referral>().Insert(newReferral);
        }
        catch (PostgrestException e)
        {
            Debug.LogError($"? Neuspešan insert: {e.Message}");
            return false;
        }

        referrer.Diamonds += 10;
        currentUser.Diamonds += 5;

        await supabase.From<User>().Update(referrer);
        await supabase.From<User>().Update(currentUser);

        Debug.Log("? Referral uspešan!");
        NagradaB();

        return true;
    }

    // ovo dobija ono onaj koji je poslao kod
    private void NagradaA()
    {
        int addCoin = 1000;
        int addDiamond = 100;
        MeniScoreManager_script.instance.updateCoinOnQuitePanel(addCoin);
        MeniScoreManager_script.instance.updateDiamondOnQuitePanel(addDiamond);

        LifeManager_script.Instance?.RestoreAllLives();


    }
    // ovo dobija ono onaj koji je ukucao kod
    private void NagradaB()
    {
        int addCoin = 200;
        int addDiamond = 20;
        MeniScoreManager_script.instance.updateCoinOnQuitePanel(addCoin);
        MeniScoreManager_script.instance.updateDiamondOnQuitePanel(addDiamond);
    }

    // ? Ispravljeno - bez await
    private void OnDestroy()
    {
        try
        {
            if (channel != null)
            {
                channel.Unsubscribe();  // ? Bez await
                Debug.Log("?? Channel unsubscribed");
            }

            // ? Disconnect bez async
            if (supabase?.Realtime != null)
            {
                supabase.Realtime.Disconnect();  // ? Umesto DisconnectAsync
                Debug.Log("?? Realtime socket diskonektovan");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"? Greška pri cleanup-u: {e.Message}");
        }
    }
}