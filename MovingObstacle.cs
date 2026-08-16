using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class MovingObstacle : MonoBehaviour, IPointerDownHandler
{
    [Header("Padanje")]
    [SerializeField] private float fallGravity = 3f;

    [SerializeField] private bool stopHorizontalWhenClicked = false;

    [Header("Uništavanje")]
    [SerializeField] private float destroyOffset = 3f;

    private Rigidbody2D rb;
    private Camera mainCamera;

    private float direction;
    private float moveSpeed;

    private bool clicked = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;

        // Dok prepreka leti - nema gravitacije
        rb.gravityScale = 0f;

        // Da se ne okreće sama od fizike
        rb.freezeRotation = true;
    }

    public void Initialize(float newDirection, float newSpeed, Camera camera)
    {
        direction = newDirection;
        moveSpeed = newSpeed;
        mainCamera = camera;

        rb.linearVelocity =
            new Vector2(direction * moveSpeed, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clicked)
            return;

        clicked = true;

        // Sada počinje da pada
        rb.gravityScale = fallGravity;

        if (stopHorizontalWhenClicked)
        {
            rb.linearVelocity =
                new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void Update()
    {
        if (mainCamera == null)
            return;

        float distanceFromCamera =
            Mathf.Abs(mainCamera.transform.position.z);

        Vector3 bottomLeft =
            mainCamera.ViewportToWorldPoint(
                new Vector3(0f, 0f, distanceFromCamera)
            );

        Vector3 topRight =
            mainCamera.ViewportToWorldPoint(
                new Vector3(1f, 1f, distanceFromCamera)
            );

        bool outsideLeft =
            transform.position.x < bottomLeft.x - destroyOffset;

        bool outsideRight =
            transform.position.x > topRight.x + destroyOffset;

        bool outsideBottom =
            transform.position.y < bottomLeft.y - destroyOffset;

        if (outsideLeft || outsideRight || outsideBottom)
        {
            Destroy(gameObject);
        }
    }
}