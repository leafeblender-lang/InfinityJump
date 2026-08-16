using UnityEngine;
using System.Collections;

public class TreperucaSkripta : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Renderer rend;

    private float trenutniGlow;
    private float maxGlow = 3f;
    private float minGlow = 1f;
    private float vremeVidljivosti = 1.5f;

                                      // private float korak = 0.5f;

    void Start()
    {
        Debug.Log("1");
        rend = GetComponent<Renderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trenutniGlow = rend.material.GetFloat("_GlowAmount");  // <-- Obavezno isto ime kao u Shader Graph
       // Nestani();
        Debug.Log("2");
    }
    public void smanjiVremeVidljivvosti()
    {
        if (vremeVidljivosti < 1.1f) return;
        vremeVidljivosti -= 0.5f;
    }
    private void OnBecameVisible()
    {
        Nestani();
        
    }
    public void neonskoNestani()
    {
        trenutniGlow = minGlow;

        Color boja = spriteRenderer.color;
        spriteRenderer.color = new Color(boja.r, boja.g, boja.b, 1f);

        StartCoroutine(NeonskiNestani());
    }
    private IEnumerator NeonskiNestani()
    {
        
        float korak = 0.4f;
        while (trenutniGlow < maxGlow)
        {
            trenutniGlow += korak;
            rend.material.SetFloat("_GlowAmount", trenutniGlow);
            yield return new WaitForSeconds(0.2f);
        }
        gameObject.SetActive(false);
    }
    public void Nestani()
    {
        StartCoroutine(NestaniPosleVremena());
        StartCoroutine(PovecajGlowAmount());
    }
    private bool nestajemo = false;
    private IEnumerator PovecajGlowAmount()
    {

        bool povecavaj = true;

        while (!nestajemo)
        {
            if (povecavaj)
            {
                trenutniGlow = maxGlow;
                if (trenutniGlow >= maxGlow)
                {
                    trenutniGlow = maxGlow;
                    povecavaj = false;
                }
            }
            else
            {
                trenutniGlow = minGlow;
                if (trenutniGlow <= minGlow)
                {
                    trenutniGlow = minGlow;
                    povecavaj = true;
                }
            }

            rend.material.SetFloat("_GlowAmount", trenutniGlow);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator NestaniPosleVremena()
    {
        yield return new WaitForSeconds(vremeVidljivosti);
        nestajemo = true;
        // Sprite providnost
        Color boja = spriteRenderer.color;
        spriteRenderer.color = new Color(boja.r, boja.g, boja.b, 0f);

        // Glow ugasi
       
    }
}


