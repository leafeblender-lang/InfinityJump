using UnityEngine;

public class PracenjePozadine : MonoBehaviour
{
    
    public Transform kamera; // dodaj Main Camera ovde u Inspectoru
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - kamera.position;
    }

    void LateUpdate()
    {
        Vector3 novaPozicija = kamera.position + offset;
        transform.position = new Vector3(transform.position.x, novaPozicija.y, transform.position.z);
    }
}

