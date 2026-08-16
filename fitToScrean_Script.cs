using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitToScreen_Script : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastScreenSize;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (mainCamera == null || spriteRenderer == null)
        {
            Debug.LogError("Main Camera ili SpriteRenderer nisu pronađeni.");
            return;
        }

        FitToScreen();
    }

    void Update()
    {
        // Ako se veličina ekrana promenila (npr. promenjen simulator profil), ažuriraj
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            FitToScreen();
        }
    }

    public float GetFitScreenScaleX()
    {
        float zOffset = 0f;
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, zOffset);

        float distance = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distance));
        Vector3 screenWorldSize = topRight - bottomLeft;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        float scaleX = screenWorldSize.x / spriteSize.x;
     
        return scaleX;
    }
    public float GetFitScreenScaleY()
    {
        float zOffset = 0f;
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, zOffset);

        float distance = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distance));
        Vector3 screenWorldSize = topRight - bottomLeft;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        float scaleY = screenWorldSize.y / spriteSize.y;
        return scaleY;
    }
    void FitToScreen()
    {
        float zOffset = 0f;
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, zOffset);

        float distance = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distance));
        Vector3 screenWorldSize = topRight - bottomLeft;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        float scaleX = screenWorldSize.x / spriteSize.x;
        float scaleY = screenWorldSize.y / spriteSize.y;
        // Odaberi veći faktor kako bi sprite pokrio ceo ekran, ali bez izobličenja

        transform.localScale = new Vector3(scaleX, scaleY, 1);

        lastScreenSize = new Vector2(Screen.width, Screen.height);
    }
}
