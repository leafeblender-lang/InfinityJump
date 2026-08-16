using UnityEngine;

public class RotateZ : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}