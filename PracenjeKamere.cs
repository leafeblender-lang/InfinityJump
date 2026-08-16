using UnityEngine;

public class PracenjeKamere : MonoBehaviour
{
    
    private float newCameraY;
    private float prevCameraY = 0f;
   //neee koristis  private float promaneX = 5f;
    private bool pomeri = false;
    private bool o = false;
    private float brzina = 4f;
    public void pomeriKameru(float t) {
        pomeri = true;
      //  t = t / 2;//ovo obrisi
        if (o) {
            newCameraY = prevCameraY + t ;
  
        }
        else {
            newCameraY = prevCameraY + t - 0.3f;
            o = true;
        }
            prevCameraY = newCameraY; 
    
    }
    private void Update()
    {

        if (pomeri)
        {
            Vector3 poz=Camera.main.transform.position;
            float novaY = Mathf.Lerp(poz.y, newCameraY, brzina * Time.deltaTime);
            Camera.main.transform.position = new Vector3(poz.x, novaY, poz.z);
            if (Mathf.Abs(newCameraY - novaY) < 0.01f)
            {
                Camera.main.transform.position = new Vector3(poz.x, newCameraY, poz.z);
                pomeri = false;
            }
        }
   
    }
}

