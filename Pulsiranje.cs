using UnityEngine;

public class Pulsiranje: MonoBehaviour
{
    private RectTransform rectTransform;
    public float minScale = 0.9f;  // najmanja veličina
    public float maxScale = 1.1f;  // najveća veličina
    public float speed = 1.5f;     // brzina pulsiranja

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartCoroutine(Pulsiraj());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        rectTransform.localScale = Vector3.one; // reset skale
    }

    private System.Collections.IEnumerator Pulsiraj()
    {
        while (true)
        {
            // Pulsiranje tamo-amo
            float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f; // 0-1 vrednost
            float scale = Mathf.Lerp(minScale, maxScale, t);
            rectTransform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }
    }
}
