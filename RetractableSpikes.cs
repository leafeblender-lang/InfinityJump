using System.Collections;
using UnityEngine;

public class RetractableSpikes : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float extendDistance = 0.15f;

    [SerializeField] private float extendTime = 0.25f;
    [SerializeField] private float stayExtendedTime = 1.0f;

    [SerializeField] private float retractTime = 0.25f;
    [SerializeField] private float stayRetractedTime = 1.0f;

    [Header("Start")]
    [SerializeField] private float initialDelay = 0f;

    private Vector3 retractedPosition;
    private Vector3 extendedPosition;

    private void Awake()
    {
        // Pozicija koju podesiš u editoru smatra se
        // potpuno uvučenom pozicijom.
        retractedPosition = transform.localPosition;

        extendedPosition =
            retractedPosition + Vector3.up * extendDistance;
    }

    private IEnumerator Start()
    {
        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // Čeka u bloku
            yield return new WaitForSeconds(stayRetractedTime);

            // Izlazi
            yield return MoveSpikes(
                retractedPosition,
                extendedPosition,
                extendTime
            );

            // Ostaje napolju
            yield return new WaitForSeconds(stayExtendedTime);

            // Vraća se
            yield return MoveSpikes(
                extendedPosition,
                retractedPosition,
                retractTime
            );
        }
    }

    private IEnumerator MoveSpikes(
        Vector3 from,
        Vector3 to,
        float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);

            // Malo prirodniji pokret nego običan linearni Lerp
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localPosition =
                Vector3.Lerp(from, to, t);

            yield return null;
        }

        transform.localPosition = to;
    }
}