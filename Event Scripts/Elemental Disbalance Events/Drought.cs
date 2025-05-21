using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Drought : MapValues
{
    public bool droughtIsActive = false;
    public bool droughtOccured = false;
    public TickSystem tickSystem;
    public int counter = 1;
    public float counterTickLength;
    public int droughtCooldown = 2400; //2400 = 10 minutes

    public List<Sprite> GrassSprites;
    public List<Sprite> GrassCliffSprites;
    public List<Sprite> WaterSprites;

    public void Initialize()
    {
        SetMap();
        SetSpritesForUse();
    }
    void SetMap()
    {
        grid = mapGenerator.grid;
        structureGrid = mapGenerator.structureGrid;
    }
    void SetSpritesForUse()
    {
        GrassSprites = spritesGetter.GrassSprites;
        GrassCliffSprites = spritesGetter.GrassCliffSprites;
        WaterSprites = spritesGetter.WaterSprites;
    }

    IEnumerator<object> TickDrought()
    {
        while (true)
        {
            if (droughtIsActive && counter == Random.Range(20, 30)) // 960, 1440
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
                GameObject currentTile = grid[i, j];

                if (currentTile.GetComponent<Tile>().type == 1)
                {
                    try
                    {
                        GameObject tileOnLeft = grid[i - 1, j];
                        GameObject tileOnRight = grid[i + 1, j];
                        GameObject tileOnTop = grid[i, j + 1];
                        GameObject tileOnBottom = grid[i, j - 1];

                        if (i > 0 && tileOnLeft != null && (tileOnLeft.GetComponent<Tile>().type == 0 ||
                        GrassCliffSprites.Contains(tileOnLeft.GetComponent<SpriteRenderer>().sprite)))
                        {
                            list.Add(tileOnLeft);
                        }
                        if (i < mapGenerator.x - 1 && tileOnRight != null && (tileOnRight.GetComponent<Tile>().type == 0 ||
                        GrassCliffSprites.Contains(tileOnRight.GetComponent<SpriteRenderer>().sprite)))
                        {
                            list.Add(tileOnRight);
                        }
                        if (j > 0 && tileOnBottom != null && (tileOnBottom.GetComponent<Tile>().type == 0 ||
                        GrassCliffSprites.Contains(tileOnBottom.GetComponent<SpriteRenderer>().sprite)))
                        {
                            list.Add(tileOnBottom);
                        }
                        if (j < mapGenerator.y - 1 && tileOnTop != null && (tileOnTop.GetComponent<Tile>().type == 0 ||
                        GrassCliffSprites.Contains(tileOnTop.GetComponent<SpriteRenderer>().sprite)))
                        {
                            list.Add(tileOnTop);
                        }
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        continue;
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
                GameObject currentTile = grid[i, j];

                if (currentTile.GetComponent<Tile>().type == 1)
                {
                    try
                    {
                        GameObject tileOnBottom = grid[i, j - 1];

                        if (j > 0 && tileOnBottom != null && tileOnBottom.GetComponent<Tile>().type == 0)
                        {
                            list.Add(currentTile);
                        }
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        continue;
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
            int randomGrassIndex = Random.Range(0, GrassSprites.Count);
            tile.GetComponent<Tile>().type = 1;
            tile.GetComponent<SpriteRenderer>().sprite = GrassSprites[randomGrassIndex];
        }
    }

    void AssignDroughtTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            int randomGrassCliffIndex = Random.Range(0, GrassCliffSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = GrassCliffSprites[randomGrassCliffIndex];
        }
    }

    List<GameObject> GetRevertDroughtTilesFullGrass()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                GameObject currentTile = grid[i, j];

                if (currentTile.GetComponent<Tile>().type == 0)
                {
                    try
                    {
                        GameObject tileOnLeft = grid[i - 1, j];
                        GameObject tileOnRight = grid[i + 1, j];
                        GameObject tileOnTop = grid[i, j + 1];
                        GameObject tileOnBottom = grid[i, j - 1];

                        if (i > 0 && tileOnLeft != null && tileOnLeft.GetComponent<Tile>().type == 1)
                        {
                            list.Add(tileOnLeft);
                        }
                        if (i < mapGenerator.x - 1 && tileOnRight != null && tileOnRight.GetComponent<Tile>().type == 1)
                        {
                            list.Add(tileOnRight);
                        }
                        if (j > 0 && tileOnBottom != null && tileOnBottom.GetComponent<Tile>().type == 1)
                        {
                            list.Add(tileOnBottom);
                        }
                        if (j < mapGenerator.y - 1 && tileOnTop != null && tileOnTop.GetComponent<Tile>().type == 1)
                        {
                            list.Add(tileOnTop);
                        }
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        continue;
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
                GameObject currentTile = grid[i, j];

                if (currentTile.GetComponent<Tile>().type == 0)
                {
                    try
                    {
                        GameObject tileOnTop = grid[i, j + 1];

                        if (j < mapGenerator.y - 1 && tileOnTop != null && tileOnTop.GetComponent<Tile>().type == 1)
                        {
                            list.Add(tileOnTop);
                        }
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        continue;
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
            int randomWaterIndex = Random.Range(0, WaterSprites.Count);
            tile.GetComponent<Tile>().type = 0;
            tile.GetComponent<SpriteRenderer>().sprite = WaterSprites[randomWaterIndex];
        }
    }

    void AssignRevertDroughtTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            int randomGrassCliffIndex = Random.Range(0, GrassCliffSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = GrassCliffSprites[randomGrassCliffIndex];
        }
    }
}
