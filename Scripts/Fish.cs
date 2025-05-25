using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fish : MonoBehaviour
{
    public GenerateMap mapGenerator;
    public GameObject[,] grid;
    public Sprites spritesGetter;
    public GameObject fishPrefab;
    public GameObject fishArisingPrefab;
    public List<Sprite> FishSprites = new List<Sprite>();
    public List<Sprite> FishArisingSprites = new List<Sprite>();
    public List<Sprite> WaterSprites = new List<Sprite>();

    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;

    private float tileSize = 1f; // Adjust if your tile size is not 1 Unity unit

    private List<Vector2Int> waterTiles = new List<Vector2Int>();

    void Awake()
    {
        SetSprites();
    }

    IEnumerator Start()
    {   
        yield return new WaitForSeconds(5f); // Ensure map is generated before accessing it
        SetMap();
        CollectWaterTiles();
        StartCoroutine(SpawnFishLoop());
    }

    void SetMap()
    {
        grid = mapGenerator.grid;
    }

    void SetSprites()
    {
        FishSprites = spritesGetter.FishSprites;
        FishArisingSprites = spritesGetter.FishArisingSprites;
        WaterSprites = spritesGetter.WaterSprites;
    }

    void CollectWaterTiles()
    {
        waterTiles.Clear();
        for (int x = 0; x < mapGenerator.x; x++)
        {
            for (int y = 0; y < mapGenerator.y; y++)
            {
                try
                {
                    GameObject currentTile = grid[x, y];
                    if (currentTile.GetComponent<Tile>().type == 0)
                    {
                        waterTiles.Add(new Vector2Int(x, y));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error accessing tile at ({x}, {y}): {e.Message}");
                }
            }
        }
    }

    IEnumerator SpawnFishLoop()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            if (waterTiles.Count == 0)
                continue;

            Vector2Int gridPos = waterTiles[Random.Range(0, waterTiles.Count)];
            Vector3 worldPos = new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0f);

            StartCoroutine(FishArise(worldPos));
        }
    }

    IEnumerator FishArise(Vector3 position)
{
    GameObject splash = Instantiate(fishArisingPrefab, position, Quaternion.identity);
    Debug.Log("Splash instantiated at " + position);

    SpriteRenderer splashRenderer = splash.GetComponentInChildren<SpriteRenderer>();
    if (splashRenderer != null && FishArisingSprites.Count > 0)
    {
        splashRenderer.sprite = FishArisingSprites[Random.Range(0, FishArisingSprites.Count)];
        Debug.Log("Splash sprite assigned");
    }
    else
    {
        Debug.LogWarning("Splash SpriteRenderer missing or FishArisingSprites empty");
    }

    yield return new WaitForSeconds(1f);

    Destroy(splash);

    GameObject fish = Instantiate(fishPrefab, position, Quaternion.identity);
    Debug.Log("Fish instantiated at " + position);

    SpriteRenderer fishRenderer = fish.GetComponentInChildren<SpriteRenderer>();
    if (fishRenderer != null && FishSprites.Count > 0)
    {
        fishRenderer.sprite = FishSprites[Random.Range(0, FishSprites.Count)];
        Debug.Log("Fish sprite assigned");
    }
    else
    {
        Debug.LogWarning("Fish SpriteRenderer missing or FishSprites empty");
    }

    Vector2 direction = Random.value < 0.5f ? Vector2.left : Vector2.right;
    float moveDuration = 2f;
    float moveSpeed = 1f;

    float timer = 0f;
    while (timer < moveDuration)
    {
        fish.transform.Translate(direction * moveSpeed * Time.deltaTime);
        timer += Time.deltaTime;
        yield return null;
    }

    Vector3 fishPosition = fish.transform.position;
    Destroy(fish);

    GameObject splashEnd = Instantiate(fishArisingPrefab, fishPosition, Quaternion.identity);

    // Assign random sprite for splash end
    SpriteRenderer splashEndRenderer = splashEnd.GetComponent<SpriteRenderer>();
    if (splashEndRenderer != null && FishArisingSprites.Count > 0)
    {
        splashEndRenderer.sprite = FishArisingSprites[Random.Range(0, FishArisingSprites.Count)];
    }

    yield return new WaitForSeconds(1f);
    Destroy(splashEnd);
}
}
