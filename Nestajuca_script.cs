using UnityEngine;

public class Nestajuca_script : MonoBehaviour
{
    private bool padaj = false;
    public float brzinaPada = 0.2f;
    private float pocetnaY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ukljuciPad()
    {
        padaj = true;
        pocetnaY=transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!padaj) return;
        Debug.Log("Pada");
        transform.position += Vector3.down * brzinaPada * Time.deltaTime;
        if (transform.position.y <= pocetnaY - 5f) 
        {
            Destroy(gameObject);
        }
    }
}
