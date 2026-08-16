using UnityEngine;

public class AktivirajIDeaktivirajNestajucu11 : MonoBehaviour
{
    [SerializeField] private GameObject aktivirajuca;
    [SerializeField] private GameObject deaktivirajuca;
    [SerializeField] private int brojPokretanja = 1;

    public void Pokreni()
    {
        if (brojPokretanja <= 0)
            return;

        if (aktivirajuca != null)
        {
            aktivirajuca.SetActive(true);
        }

        if (deaktivirajuca != null)
        {
            deaktivirajuca.SetActive(false);
        }

        brojPokretanja--;
    }
}