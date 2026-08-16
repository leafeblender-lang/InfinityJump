using System;
using System.Collections;
using UnityEngine;

public class OtapanjeIceBloka_scripta : MonoBehaviour
{
    public float brzinaOtapanja=10f;
    private SpriteRenderer sr;
    private void Start()
    {
        sr= GetComponent<SpriteRenderer>();
    }
    public void pocniOtapanje()
    {
        StartCoroutine(SmanjiTransparentnost());
    }

    private IEnumerator SmanjiTransparentnost()
    {
        Debug.Log("Otopi");
        Color c = sr.color;
        while (c.a > 0)
        {
            c.a -= brzinaOtapanja * Time.deltaTime;
            sr.color = c;
            yield return null;
        }
        gameObject.SetActive(false);
        c.a = 1f;
        sr.color = c;
    }
}
