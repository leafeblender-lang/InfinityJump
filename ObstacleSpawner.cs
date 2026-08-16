using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    [SerializeField] private GameObject[] obstaclePrefabs;

    [Header("Camera")]
    [SerializeField] private Camera gameplayCamera;

    [Header("Spawn Time")]
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 4f;

    [Header("Spawn Height")]
    [Range(0f, 1f)]
    [SerializeField] private float minViewportY = 0.9f;

    [Range(0f, 1f)]
    [SerializeField] private float maxViewportY = 0.1f;

    [Header("Spawn Offset")]
    [SerializeField] private float spawnOffset = 1f;

    private void Awake()
    {
        Debug.Log(
            "[SPAWNER] AWAKE | object=" + gameObject.name +
            " | active=" + gameObject.activeInHierarchy +
            " | enabled=" + enabled
        );

        if (gameplayCamera == null)
        {
            gameplayCamera = Camera.main;

            Debug.Log(
                "[SPAWNER] Camera nije bila postavljena. Camera.main = " +
                (gameplayCamera != null ? gameplayCamera.name : "NULL")
            );
        }
        else
        {
            Debug.Log(
                "[SPAWNER] Gameplay camera = " +
                gameplayCamera.name
            );
        }
    }

    private void Start()
    {
        Debug.Log(
            "[SPAWNER] START" +
            " | timeScale=" + Time.timeScale +
            " | prefabs=" +
            (obstaclePrefabs != null
                ? obstaclePrefabs.Length.ToString()
                : "NULL")
        );

        if (gameplayCamera == null)
        {
            Debug.LogError("[SPAWNER] STOP: gameplayCamera je NULL.");
            return;
        }

        if (obstaclePrefabs == null ||
            obstaclePrefabs.Length == 0)
        {
            Debug.LogError("[SPAWNER] STOP: nema prefabova.");
            return;
        }

        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            Debug.Log(
                "[SPAWNER] Prefab[" + i + "] = " +
                (obstaclePrefabs[i] != null
                    ? obstaclePrefabs[i].name
                    : "NULL")
            );
        }

        // Jedna prepreka ODMAH za test.
        SpawnObstacle();

        Debug.Log("[SPAWNER] Pokrecem SpawnLoop coroutine.");

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime =
                Random.Range(
                    minSpawnTime,
                    maxSpawnTime
                );

            Debug.Log(
                "[SPAWNER] Sledeci spawn za " +
                waitTime.ToString("F2") +
                " sekundi."
            );

            yield return new WaitForSecondsRealtime(waitTime);

            Debug.Log("[SPAWNER] Wait zavrsen -> SpawnObstacle()");

            SpawnObstacle();
        }
    }

    private void SpawnObstacle()
    {
        Debug.Log("[SPAWNER] ===== SPAWN POCETAK =====");

        if (obstaclePrefabs == null ||
            obstaclePrefabs.Length == 0)
        {
            Debug.LogError("[SPAWNER] Spawn nema prefabove.");
            return;
        }

        int randomIndex =
            Random.Range(0, obstaclePrefabs.Length);

        GameObject prefab =
            obstaclePrefabs[randomIndex];

        Debug.Log(
            "[SPAWNER] Izabran index=" +
            randomIndex +
            " | prefab=" +
            (prefab != null ? prefab.name : "NULL")
        );

        if (prefab == null)
        {
            Debug.LogError("[SPAWNER] Izabrani prefab je NULL.");
            return;
        }

        bool fromLeft =
            Random.value < 0.5f;

        float randomY =
            Random.Range(
                minViewportY,
                maxViewportY
            );

        float cameraDistance =
            Mathf.Abs(
                gameplayCamera.transform.position.z
            );

        float viewportX =
            fromLeft ? 0f : 1f;

        Vector3 spawnPosition =
            gameplayCamera.ViewportToWorldPoint(
                new Vector3(
                    viewportX,
                    randomY,
                    cameraDistance
                )
            );

        spawnPosition.z = 0f;

        if (fromLeft)
            spawnPosition.x -= spawnOffset;
        else
            spawnPosition.x += spawnOffset;

        Debug.Log(
            "[SPAWNER] Strana=" +
            (fromLeft ? "LEVA" : "DESNA") +
            " | viewportY=" +
            randomY.ToString("F3") +
            " | worldPos=" +
            spawnPosition
        );

        GameObject obstacle =
            Instantiate(
                prefab,
                spawnPosition,
                Quaternion.identity
            );

        Debug.Log(
            "[SPAWNER] Instantiate USPESAN: " +
            obstacle.name +
            " | position=" +
            obstacle.transform.position
        );

        ObstacleMovementBase movement =
            obstacle.GetComponent<ObstacleMovementBase>();

        Debug.Log(
            "[SPAWNER] Movement komponenta = " +
            (movement != null
                ? movement.GetType().Name
                : "NULL")
        );

        if (movement == null)
        {
            Debug.LogError(
                "[SPAWNER] " +
                obstacle.name +
                " NEMA komponentu koja nasledjuje ObstacleMovementBase!"
            );

            // NE UNISTAVAMO zbog debugovanja.
            return;
        }

        float direction =
            fromLeft ? 1f : -1f;

        Debug.Log(
            "[SPAWNER] Pozivam Initialize(" +
            direction +
            ") na " +
            obstacle.name
        );

        movement.Initialize(
            direction,
            gameplayCamera
        );

        Debug.Log("[SPAWNER] ===== SPAWN KRAJ =====");
    }
}