using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class FitCameraWidth : MonoBehaviour
{
    [Header("Aspect koji dizajniraš")]
    public float targetAspect = 9f / 16f; // 1080x1920

    [Header("Anchor viewporta")]
    [Range(0f, 1f)]
    public float verticalAnchor = 0f; 
    // 0 = gameplay zalepljen za dno ekrana
    // 0.5 = centriran
    // 1 = zalepljen za vrh

    [Range(0f, 1f)]
    public float horizontalAnchor = 0.5f;
    // 0.5 = centrirano levo-desno

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    void LateUpdate()
    {
        Apply();
    }

    void Apply()
    {
        if (cam == null) cam = GetComponent<Camera>();

        float windowAspect = (float)Screen.width / Screen.height;
        Rect rect = new Rect(0f, 0f, 1f, 1f);

        if (windowAspect < targetAspect)
        {
            // Ekran je uži/viši od 9:16.
            // Gameplay ostaje 9:16, višak ide gore/dole.
            float viewportHeight = windowAspect / targetAspect;

            rect.width = 1f;
            rect.height = viewportHeight;
            rect.x = 0f;
            rect.y = (1f - viewportHeight) * verticalAnchor;
        }
        else
        {
            // Ekran je širi od 9:16.
            // Gameplay ostaje 9:16, višak ide levo/desno.
            float viewportWidth = targetAspect / windowAspect;

            rect.width = viewportWidth;
            rect.height = 1f;
            rect.x = (1f - viewportWidth) * horizontalAnchor;
            rect.y = 0f;
        }

        cam.rect = rect;
    }
}