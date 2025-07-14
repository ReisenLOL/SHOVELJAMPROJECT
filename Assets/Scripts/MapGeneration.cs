using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour
{
    [Header("Settings")]
    public int width = 50; // Width of the map
    public int height = 50; // Height of the map
    public float terrainScale = 10f; // Scale of Perlin noise
    public float fluxScale = 15f;  // Different scale for ore veins
    public float wallScale = 8f;  // Walls should appear in patches
    public float fluxChance;

    [Header("Cache")] 
    public GameObject fluxGeyserObject;
    public Transform fluxGeyserFolder;
    public Tilemap tileMap;
    public TileBase groundTile;
    public TileBase waterTile;
    public TileBase deepWaterTile;
    public TileBase fluxGeyserTile;
    public TileBase walltile;
    private bool[,] fluxMap; // Stores where ore should be placed
    private bool[,] visited; // Keeps track of visited tiles
    public bool randomSeed = true;
    public NavMeshSurface navMeshSurface;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        fluxMap = new bool[width, height];
        visited = new bool[width, height];
        int newNoise = 0;
        if (randomSeed)
        {
            newNoise = Random.Range(0, 10000);
        }
        // land or water
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float terrainValue = Mathf.PerlinNoise((x + newNoise) / terrainScale, (y + newNoise) / terrainScale);
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (terrainValue > 0.2f)
                {
                    tileMap.SetTile(tilePosition, groundTile);
                }
                else if (terrainValue > 0.15f && terrainValue <= 0.25f)
                {
                    tileMap.SetTile(tilePosition, waterTile);
                }
                else
                {
                    tileMap.SetTile(tilePosition, deepWaterTile);
                }
            }
        }
        // flux or land
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (tileMap.GetTile(tilePosition) == groundTile)
                {
                    float oreValue = Mathf.PerlinNoise((x + newNoise)/ fluxScale, (y + newNoise) / fluxScale);
                    if (oreValue > fluxChance)
                    {
                        fluxMap[x, y] = true;
                    }
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (fluxMap[x, y] && !visited[x, y])
                {
                    FloodFillOrePatch(x, y, fluxGeyserTile);
                }
            }
        }
        // wall or land
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (tileMap.GetTile(tilePosition) == groundTile)
                {
                    float wallValue = Mathf.PerlinNoise((x + newNoise) / wallScale, (y + newNoise) / wallScale);
                    if (wallValue > 0.7f)
                    {
                        tileMap.SetTile(tilePosition, walltile);
                    }
                }
            }
        }
        navMeshSurface.BuildNavMesh();
    }
    void FloodFillOrePatch(int startX, int startY, TileBase oreType)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        visited[startX, startY] = true;

        while (queue.Count > 0)
        {
            Vector2Int pos = queue.Dequeue();
            int x = pos.x;
            int y = pos.y;

            tileMap.SetTile(new Vector3Int(x, y, 0), oreType);
            GameObject newGeyer = Instantiate(fluxGeyserObject, fluxGeyserFolder);
            newGeyer.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(x, y));
            Vector2Int[] neighbors = { new Vector2Int(x + 1, y), new Vector2Int(x - 1, y), new Vector2Int(x, y + 1), new Vector2Int(x, y - 1) };

            foreach (Vector2Int neighbor in neighbors)
            {
                int nx = neighbor.x;
                int ny = neighbor.y;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny] && fluxMap[nx, ny])
                {
                    visited[nx, ny] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }
}