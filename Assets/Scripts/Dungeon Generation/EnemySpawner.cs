using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Tilemap floorTilemap;
    public TileBase floorTile;

    public GameObject[] enemyPrefabs;
    public int enemyCount = 5;
    public float minEnemySpacing = 2f;
    public float minDistanceFromPlayer = 20f;

    public Transform playerTransform;

    public CorridorFirstDungeonGenerator dungeonGenerator;

    private List<Vector3> validFloorPositions = new List<Vector3>();
    private List<Vector3> spawnedEnemyPositions = new List<Vector3>();

    void Start()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.OnGenerationComplete += EnableSpawner;
        }
        else
        {
            Debug.LogError("Dungeon Generator reference is missing!");
        }
    }

    private void EnableSpawner()
    {
        CollectFloorPositions();
        SpawnEnemies();
    }

    void CollectFloorPositions()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = floorTilemap.GetTile(cellPosition);

                if (tile == floorTile)
                {
                    Vector3 worldPos = floorTilemap.CellToWorld(cellPosition) + floorTilemap.tileAnchor;
                    validFloorPositions.Add(worldPos);
                }
            }
        }
    }

    void SpawnEnemies()
    {
        int spawnAttempts = 0;
        int maxAttempts = enemyCount * 10;
        int enemiesSpawned = 0;

        while (enemiesSpawned < enemyCount && spawnAttempts < maxAttempts)
        {
            spawnAttempts++;

            if (validFloorPositions.Count == 0)
            {
                Debug.LogWarning("No valid floor positions left for enemy spawning!");
                break;
            }

            int randomIndex = Random.Range(0, validFloorPositions.Count);
            Vector3 spawnPos = validFloorPositions[randomIndex];

            if (Vector3.Distance(spawnPos, playerTransform.position) < minDistanceFromPlayer)
            {
                validFloorPositions.RemoveAt(randomIndex);
                continue;
            }

            bool tooClose = false;
            foreach (Vector3 pos in spawnedEnemyPositions)
            {
                if (Vector3.Distance(spawnPos, pos) < minEnemySpacing)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose)
            {
                validFloorPositions.RemoveAt(randomIndex);
                continue;
            }

            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            spawnedEnemyPositions.Add(spawnPos);
            validFloorPositions.RemoveAt(randomIndex);
            enemiesSpawned++;
        }
    }
}
