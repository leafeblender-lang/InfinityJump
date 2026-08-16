using TMPro;
using UnityEngine;

public class statiziraj : MonoBehaviour
{
     public static statiziraj Instance;
    TextMeshProUGUI tekst;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // Ako postoji ve? instanca, uništi duplikat
            return;
        }
        tekst = GetComponent<TextMeshProUGUI>();

        if (tekst == null)
        {
            Debug.LogWarning("Nije prona?ena TextMeshProUGUI komponenta na ovom GameObject-u.");
        }
    }

    private void Start()
    {
        NulirajTransparentnost();
    }
    public void NulirajTransparentnost()
    {
        if (tekst != null)
        {
            this.gameObject.SetActive(false);
            Color boja = tekst.color;
            boja.a = 0f; // alfa = 0 => potpuno providno
            tekst.color = boja;
        }
    }

    // Ako želiš i da vratiš vidljivost, možeš napraviti funkciju kao:
    public void VratiTransparentnost()
    {
        if (tekst != null)
        {
            this.gameObject.SetActive(true);
            Color boja = tekst.color;
            boja.a = 1f; // alfa = 1 => potpuno neprovidno (vidljivo)
            tekst.color = boja;
        }
    }
}
