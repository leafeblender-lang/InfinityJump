using UnityEngine;

public class DeadTeiger : MonoBehaviour
{
    BouncingBall roditelj;

    private void Start()
    {
        roditelj = transform.parent.GetComponent<BouncingBall>();

    }
    private void OnTriggerEnter2D(Collider2D triger)
    {
        if (triger.gameObject.tag == "AktivirajucaZamka")
        {
            // Pristupi roditelju i njegovoj skripti JumpingBallScript
            if (roditelj != null)
            {
                roditelj.BrzinaPada = 0;
                roditelj.ubrzanje = 0;
                Debug.Log("Stop");
            }
            else
            {
                Debug.LogWarning("Nema roditeljske skripte JumpingBallScript!");
            }
        }
    }
}
