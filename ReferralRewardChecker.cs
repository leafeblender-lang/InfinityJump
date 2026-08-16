
/*using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
public class ReferralRewardChecker : MonoBehaviour
{
    FirebaseFirestore db;
    string myUserId;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        myUserId = PlayerPrefs.GetString("user_id", "");
        if (string.IsNullOrEmpty(myUserId))
        {
            Debug.Log("Nemam user ID!");
            return;
        }

        // Na primer, proveri odmah
        // Pokreni proveru svakih 10 sekundi
        StartCoroutine(PeriodicRewardCheck());
    }
    IEnumerator PeriodicRewardCheck()
    {
        while (true)
        {
            yield return CheckForRewardsAsync().AsCoroutine(); // async -> coroutine konverzija
            yield return new WaitForSeconds(10f); // ?ekaj 10 sekundi
        }
    }

    public async Task CheckForRewardsAsync()
    {
        var referralsRef = db.Collection("referrals");
        var query = referralsRef
            .WhereEqualTo("referrer", myUserId)
            .WhereEqualTo("rewarded", false);

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            Debug.Log($"Nadjena nerewardovana referral nagrada: {doc.Id}");

            // TODO: ovde pozovi funkciju koja dodeljuje nagradu u igri
            GiveReward();

            // Update rewarded polje na true
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "rewarded", true }
            };
            await doc.Reference.UpdateAsync(updates);

            Debug.Log($"Referral {doc.Id} je sada rewardovan.");
        }
    }

    void GiveReward()
    {
        // Ovde implementiraj logiku nagrade, npr:
        Debug.Log("Nagrada dodeljena igra?u!");
        // ... dodaj poene, iteme, itd.
    }
}
*/