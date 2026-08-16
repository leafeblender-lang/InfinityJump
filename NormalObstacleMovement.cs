using UnityEngine;

public class NormalObstacleMovement : ObstacleMovementBase
{
    [Header("Movement")]
    [SerializeField] private float speed = 3f;

    private void Update()
    {
        if (!movementEnabled)
            return;

        transform.position +=
            Vector3.right *
            direction *
            speed *
            Time.deltaTime;
    }

    public override Vector2 GetReleaseVelocity()
    {
        return new Vector2(
            direction * speed,
            0f
        );
    }
}