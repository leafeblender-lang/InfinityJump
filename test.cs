using UnityEngine;
using Supabase;

public class dodelaId : MonoBehaviour
{
    async void Start()
    {
        var client = new Client("https://tvoj-projekat.supabase.co", "tvoj-anon-public-key");
        await client.InitializeAsync();
        Debug.Log("? Supabase Client radi!");
    }
}
