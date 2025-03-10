using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TrophySpawner : MonoBehaviour
{
    public Tilemap floorTilemap;
    public TileBase floorTile;

    public GameObject trophyPrefab;
    public LayerMask obstacleLayer;
    public float minWallDistance = 2f;

    public Transform playerTransform;

    public CorridorFirstDungeonGenerator dungeonGenerator;

    private List<Vector3> validFloorPositions = new List<Vector3>();
    private bool trophySpawned = false;

    void Start()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.OnGenerationComplete += OnDungeonGenerationComplete;
        }
        else
        {
            Debug.LogError("Reference missing");
        }
    }

    private void OnDungeonGenerationComplete()
    {
        StartCoroutine(SpawnTrophyDelayed());
    }

    private IEnumerator SpawnTrophyDelayed()
    {
        yield return null;

        CollectFloorPositions();
        SpawnTrophy();
    }

    void CollectFloorPositions()
    {
        validFloorPositions.Clear();
        BoundsInt bounds = floorTilemap.cellBounds;

        int floorTilesFound = 0;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = floorTilemap.GetTile(cellPosition);
                if (tile == floorTile)
                {
                    floorTilesFound++;
                    Vector3 worldPos = floorTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0);
                    validFloorPositions.Add(worldPos);
                }
            }
        }

    }

    void SpawnTrophy()
    {
        if (validFloorPositions.Count == 0)
        {
            return;
        }

        validFloorPositions.Sort((a, b) => Vector3.Distance(b, playerTransform.position)
                                          .CompareTo(Vector3.Distance(a, playerTransform.position)));

        foreach (Vector3 pos in validFloorPositions)
        {
            if (IsValidTrophyPosition(pos, minWallDistance))
            {
                Instantiate(trophyPrefab, pos, Quaternion.identity);
                trophySpawned = true;
                return;
            }
        }

        float reducedDistance = minWallDistance;
        while (reducedDistance > 0.5f && !trophySpawned)
        {
            reducedDistance *= 0.5f;

            foreach (Vector3 pos in validFloorPositions)
            {
                if (IsValidTrophyPosition(pos, reducedDistance))
                {
                    Instantiate(trophyPrefab, pos, Quaternion.identity);
                    trophySpawned = true;
                    return;
                }
            }
        }

        if (!trophySpawned && validFloorPositions.Count > 0)
        {
            Vector3 fallbackPos = validFloorPositions[0];
            Instantiate(trophyPrefab, fallbackPos, Quaternion.identity);
            trophySpawned = true;
        }
    }

    bool IsValidTrophyPosition(Vector3 position, float distance)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, distance, obstacleLayer);
        return hit == null;
    }

    public void ForceSpawn()
    {
        CollectFloorPositions();
        SpawnTrophy();
    }

    private void OnDestroy()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.OnGenerationComplete -= OnDungeonGenerationComplete;
        }
    }
}