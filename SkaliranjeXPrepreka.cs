using System.Collections.Generic;
using UnityEngine;

public class SkaliranjeXPrepreka : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<Transform> prepreke = new List<Transform>();

    // Sirina sveta za orthographic size 6.09375 na referentnoj rezoluciji 9:20.
    private const float REFERENTNA_SIRINA_SVETA = 6.09375f * 2f * (9f / 19.5f);

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("SkaliranjeXPrepreka: Main Camera nije pronadjena.", this);
            return;
        }

        float trenutnaSirinaSveta = mainCamera.orthographicSize * 2f * mainCamera.aspect;
        float faktorSkaliranja = trenutnaSirinaSveta / REFERENTNA_SIRINA_SVETA;
        float centarKamereX = mainCamera.transform.position.x;

        foreach (Transform prepreka in prepreke)
        {
            if (prepreka == null)
                continue;

            Vector3 pozicija = prepreka.position;
            pozicija.x = centarKamereX + (pozicija.x - centarKamereX) * faktorSkaliranja;
            prepreka.position = pozicija;
        }
    }
}
