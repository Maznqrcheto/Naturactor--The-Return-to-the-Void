using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Drought : MonoBehaviour
{
    public GenerateMap mapGenerator;
    public GameObject[,] grid;
    public bool droughtIsActive = false;
    public bool droughtOccured = false;
    public List<Sprite> TileSprites;
    public TickSystem tickSystem;
    public int counter = 1;
    public float counterTickLength;
    public int droughtCooldown = 2400; //2400 = 10 minutes

    void Awake()
    {
        TileSprites = mapGenerator.TileSprites;
        grid = mapGenerator.grid;
    }

    IEnumerator<object> TickDrought()
    {
        while (true)
        {
            if (droughtIsActive && counter == Random.Range(960, 1440))
            {
                RevertDrought();
                counter = 1;
                yield break;
            }
            counter++;
            yield return new WaitForSeconds(counterTickLength);
        }
    }

    IEnumerator<object> TickDroughtCooldown()
    {
        while (true)
        {
            if (droughtOccured && droughtCooldown > 0)
            {
                droughtCooldown--;
            }
            else
            {
                droughtOccured = false;
                droughtCooldown = 2400; //reset cooldown
                yield break;
            }
            yield return new WaitForSeconds(counterTickLength);
        }
    }

    public void StartDrought()
    {
        Debug.Log("Drought started!");
        droughtIsActive = true;
        StartCoroutine(TickDrought());

        List<GameObject> droughtTilesFullGrass = GetDroughtTilesFullGrass();
        AssignDroughtTilesFullGrassSprite(droughtTilesFullGrass);

        List<GameObject> droughtTilesPartGrass = GetDroughtTilesPartGrass();
        AssignDroughtTilesPartGrassSprite(droughtTilesPartGrass);

        droughtOccured = true;
    } 

    public void RevertDrought()
    {
        List<GameObject> revertDroughtTilesFullGrass = GetRevertDroughtTilesFullGrass();
        AssignRevertDroughtTilesFullGrassSprite(revertDroughtTilesFullGrass);

        List<GameObject> revertDroughtTilesPartGrass = GetRevertDroughtTilesPartGrass();
        AssignRevertDroughtTilesPartGrassSprite(revertDroughtTilesPartGrass);

        Debug.Log("Drought ended!");
        droughtIsActive = false;
        StartCoroutine(TickDroughtCooldown());
    }

    List<GameObject> GetDroughtTilesFullGrass()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j] == null) continue;
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    if (i > 0 && grid[i - 1, j] != null && (grid[i - 1, j].GetComponent<Tile>().type == 0 || grid[i - 1, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))
                    {
                        list.Add(grid[i - 1, j]);
                    }
                    if (i < mapGenerator.x - 1 && grid[i + 1, j] != null && (grid[i + 1, j].GetComponent<Tile>().type == 0 || grid[i + 1, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))
                    {
                        list.Add(grid[i + 1, j]);
                    }
                    if (j > 0 && grid[i, j - 1] != null && (grid[i, j - 1].GetComponent<Tile>().type == 0 || grid[i, j - 1].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))
                    {
                        list.Add(grid[i, j - 1]);
                    }
                    if (j < mapGenerator.y - 1 && grid[i, j + 1] != null && (grid[i, j + 1].GetComponent<Tile>().type == 0 || grid[i, j + 1].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))
                    {
                        list.Add(grid[i, j + 1]);
                    }        
                }
            }
        }
        return list;
    }

    List<GameObject> GetDroughtTilesPartGrass()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j] == null) continue;
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    if (j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0)
                    {
                        list.Add(grid[i, j]);
                    }
                }
            }
        }
        return list;
    }

    void AssignDroughtTilesFullGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Tile>().type = 1;
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
        }
    }

    void AssignDroughtTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
        }
    }

    List<GameObject> GetRevertDroughtTilesFullGrass()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 0)
                {
                    if (i > 0 && grid[i - 1, j] != null && grid[i - 1, j].GetComponent<Tile>().type == 1)
                    {
                        list.Add(grid[i - 1, j]);
                    }
                    if (i < mapGenerator.x - 1 && grid[i + 1, j] != null && grid[i + 1, j].GetComponent<Tile>().type == 1)
                    {
                        list.Add(grid[i + 1, j]);
                    }
                    if (j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 1)
                    {
                        list.Add(grid[i, j - 1]);
                    }
                    if (j < mapGenerator.y - 1 && grid[i, j + 1] != null && grid[i, j + 1].GetComponent<Tile>().type == 1)
                    {
                        list.Add(grid[i, j + 1]);
                    }
                }
            }
        }
        return list;
    }

    List<GameObject> GetRevertDroughtTilesPartGrass()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 0)
                {
                    if (j < mapGenerator.y - 1 && grid[i, j + 1] != null && grid[i, j + 1].GetComponent<Tile>().type == 1)
                    {
                        list.Add(grid[i, j + 1]);
                    }
                }
            }
        }
        return list;
    }

    void AssignRevertDroughtTilesFullGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Tile>().type = 0;
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[0];
        }
    }

    void AssignRevertDroughtTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
        }
    }
}
