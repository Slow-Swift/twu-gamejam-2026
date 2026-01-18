using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] GameObject enemyPrefab;

    [SerializeField] Transform trashPile;

    [SerializeField] float minSpawnRange;
    [SerializeField] float maxSpawnRange;
    [SerializeField] float density;
    [SerializeField] float spawnStartWaitTime = 1;

    float nextSpawnTime;

    void Start()
    {
        // Add a break before spawning the first enemy
        nextSpawnTime = Time.time + spawnStartWaitTime;
    }

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        float distance = Random.Range(minSpawnRange, maxSpawnRange);
        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = new Vector3(
            spawnDirection.x * distance, 0, spawnDirection.y * distance
        );

        GameObject enemyobj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
        Enemy enemy = enemyobj.GetComponent<Enemy>();
        enemy.SetTarget(trashPile);
        nextSpawnTime += Random.Range(0.5f, 1.5f) / density;
    }
}
