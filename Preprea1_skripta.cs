using UnityEngine;


public class Preprea1_skripta : MonoBehaviour
{

    [SerializeField] private GameObject Obstacle2;
    //[SerializeField] private GameObject Obstacle2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Obstacle2.GetComponent<SpriteRenderer>().enabled = true;
        Obstacle2.GetComponent<BoxCollider2D>().enabled = true;
        this.GetComponent<SpriteRenderer>().enabled = false;    
        this.GetComponent<BoxCollider2D>().enabled = false;
       
    }
    // Update is called once per frame
    void Update()
    {
          
    }
}
