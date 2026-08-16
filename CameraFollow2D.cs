using UnityEngine;

public class VerticalCameraFollow_OnlyUp : MonoBehaviour
{
    public Transform target;
    public float yThreshold = 2f;
    public float smoothSpeed = 0.1f;

    private float highestY;
    private float fixedX;

    void Start()
    {
        highestY = transform.position.y;
        fixedX = transform.position.x;
    }
    void LateUpdate()
    {
        if (target == null) return;

        float camY = transform.position.y;
        float targetY = target.position.y - yThreshold;

        // Pomera se samo ako je igrač znatno iznad trenutne visine kamere
        if (target.position.y > camY + yThreshold)
        {
            // Ali samo ako je razlika veća od 0.5f
            if (Mathf.Abs(targetY - camY) > 0.5f)
            {
                highestY = Mathf.Max(highestY, targetY);
                float newY = Mathf.Lerp(camY, highestY, smoothSpeed);
                transform.position = new Vector3(fixedX, newY, transform.position.z);
            }
        }
        else
        {
            // Kamera ostaje gde jeste
            transform.position = new Vector3(fixedX, camY, transform.position.z);
        }
    }

}
