using UnityEngine;

public class PojaviNestajucu1 : MonoBehaviour
{
    [SerializeField] private GameObject pojavljujuca;  // Možeš menjati u Inspectoru
    [SerializeField] private int brojPojavljivanja = 1;
    [SerializeField] private GameObject pojavljujuca2;
    public void Aktiviraj()
    {
        if (pojavljujuca != null  && brojPojavljivanja>0)
        {
            if (pojavljujuca2 != null)
            {
                pojavljujuca2.SetActive(true);  
            }
            pojavljujuca.SetActive(true);
            brojPojavljivanja--;
        }

    }
}

