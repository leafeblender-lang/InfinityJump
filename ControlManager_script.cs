using UnityEngine;

public class ControlManager_script : MonoBehaviour
{
    public float moveForce = 5f;
    public float maxSpeed = 5f;
    public float stopDamping = 0.9f;

    private Rigidbody2D rb;
    private bool movingLeft = false;
    private bool movingRight = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movingLeft = false;
        movingRight = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                Vector2 touchPosition = touch.position;

                if (touchPosition.x < Screen.width / 2)
                {
                    movingLeft = true;
                }
                else
                {
                    movingRight = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (movingLeft)
        {
            if (rb.linearVelocityX > -maxSpeed)
                rb.AddForce(Vector2.left * moveForce);
        }
        else if (movingRight)
        {
            if (rb.linearVelocityX< maxSpeed)
                rb.AddForce(Vector2.right * moveForce);
        }
        else
        {
            // Blago smanji brzinu kad se prst pusti
            rb.linearVelocity = new Vector2(rb.linearVelocityX * stopDamping, rb.linearVelocityY);
        }
    }
}
