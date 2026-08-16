using UnityEngine;
using Supabase; // Ovo omogu?ava pristup Supabase Client klasama

public class SupabaseManager : MonoBehaviour
{
    private Client supabase; // Globalni Supabase client

    // Start je pozvan jednom kada skripta po?ne
    async void Start()
    {
        var url = "https://swozvfntfinssraolgzl.supabase.co";  // zameni sa tvojim Supabase URL
        var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InN3b3p2Zm50Zmluc3NyYW9sZ3psIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjEzMDcyODQsImV4cCI6MjA3Njg4MzI4NH0.svgEi7FYvyJww5KoEOLByZIGr_RrvIM5-_hIks6qhVk";              // zameni sa tvojim anon key

        supabase = new Client(url, key);
        await supabase.InitializeAsync(); // Povezivanje sa Supabase

        Debug.Log("? Supabase povezan!");
    }
}
