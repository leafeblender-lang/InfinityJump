using TMPro;
using UnityEngine;

public class UpdateTimerText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI vreme;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = vreme.text;
    }
}
