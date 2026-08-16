using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class ObstacleMovementBase : MonoBehaviour
{
    protected Rigidbody2D rb;

    protected float direction;
    protected bool movementEnabled = false;

    private Camera gameplayCamera;
    private bool hasEnteredScreen = false;

    private float debugTimer = 0f;

    [Header("Destroy")]
    [SerializeField] private float destroyViewportMargin = 0.15f;

    protected virtual void Awake()
    {
        Debug.Log(
            "[BASE] AWAKE | " +
            gameObject.name
        );

        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError(
                "[BASE] " +
                gameObject.name +
                " NEMA Rigidbody2D!"
            );

            return;
        }

        Debug.Log(
            "[BASE] Rigidbody pronadjen" +
            " | bodyType PRE=" + rb.bodyType +
            " | simulated=" + rb.simulated
        );

        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        Debug.Log(
            "[BASE] Rigidbody podeseno" +
            " | bodyType=" + rb.bodyType +
            " | gravity=" + rb.gravityScale +
            " | position=" + transform.position
        );
    }

    protected virtual void Start()
    {
        Debug.Log(
            "[BASE] START | " +
            gameObject.name +
            " | movementEnabled=" +
            movementEnabled
        );
    }

    public virtual void Initialize(
        float newDirection,
        Camera camera)
    {
        Debug.Log(
            "[BASE] INITIALIZE POZVAN | " +
            gameObject.name +
            " | newDirection=" +
            newDirection +
            " | camera=" +
            (camera != null
                ? camera.name
                : "NULL")
        );

        direction = newDirection;
        gameplayCamera = camera;

        movementEnabled = true;
        hasEnteredScreen = false;

        Debug.Log(
            "[BASE] INITIALIZE ZAVRSEN | " +
            gameObject.name +
            " | direction=" +
            direction +
            " | movementEnabled=" +
            movementEnabled +
            " | position=" +
            transform.position
        );
    }

    public virtual void StopMovement()
    {
        Debug.Log(
            "[BASE] STOP MOVEMENT | " +
            gameObject.name
        );

        movementEnabled = false;
    }

    public virtual Vector2 GetReleaseVelocity()
    {
        Debug.Log(
            "[BASE] GetReleaseVelocity osnovna verzija -> ZERO | " +
            gameObject.name
        );

        return Vector2.zero;
    }

    protected virtual void LateUpdate()
    {
        debugTimer += Time.unscaledDeltaTime;

        if (debugTimer >= 1f)
        {
            debugTimer = 0f;

            Debug.Log(
                "[BASE] STATUS | " +
                gameObject.name +
                " | movementEnabled=" +
                movementEnabled +
                " | direction=" +
                direction +
                " | pos=" +
                transform.position +
                " | timeScale=" +
                Time.timeScale
            );
        }

        if (gameplayCamera == null)
            return;

        Vector3 viewportPosition =
            gameplayCamera.WorldToViewportPoint(
                transform.position
            );

        if (!hasEnteredScreen)
        {
            if (viewportPosition.x >= 0f &&
                viewportPosition.x <= 1f)
            {
                hasEnteredScreen = true;

                Debug.Log(
                    "[BASE] " +
                    gameObject.name +
                    " JE USAO NA EKRAN | viewport=" +
                    viewportPosition
                );
            }

            return;
        }

        if (viewportPosition.x < -destroyViewportMargin ||
            viewportPosition.x > 1f + destroyViewportMargin ||
            viewportPosition.y < -destroyViewportMargin ||
            viewportPosition.y > 1f + destroyViewportMargin)
        {
            Debug.Log(
                "[BASE] DESTROY | " +
                gameObject.name +
                " | viewport=" +
                viewportPosition
            );

            Destroy(gameObject);
        }
    }
}