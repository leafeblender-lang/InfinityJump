using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI comomingSoon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Uskoro()
    {
        SceneManager.LoadScene(1);

        comomingSoon.enabled = true;
        StartCoroutine(Cekaj());
    }
    private IEnumerator Cekaj()
    {
        yield return new WaitForSeconds(1.5f);

        comomingSoon.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
