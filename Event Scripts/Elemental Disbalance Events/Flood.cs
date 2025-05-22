using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Flood : MapValues
{
    public bool floodIsActive = false;
    public bool floodOccured = false;
    public TickSystem tickSystem;
    public int counter = 1;
    public float counterTickLength;
    public int floodCooldown = 2400; //2400 = 10 minutes

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

    IEnumerator<object> TickFlood()
    {
        while (true)
        {
            if (floodIsActive && counter == Random.Range(960, 1440)) //960, 1440, 4-6 minutes, because tickLength = 0.25 seconds
            {
                RevertFlood();
                counter = 1;
                yield break;
            }
            counter++;
            yield return new WaitForSeconds(counterTickLength);
        }
    }

    IEnumerator<object> TickFloodCooldown()
    {
        while (true)
        {
            if (floodOccured && floodCooldown > 0)
            {
                floodCooldown--;
            }
            else
            {
                floodOccured = false;
                floodCooldown = 2400; //reset cooldown
                yield break;
            }
            yield return new WaitForSeconds(counterTickLength);
        }
    }

    public void StartFlood()
    {
        Debug.Log("Flood started!");
        floodIsActive = true;
        StartCoroutine(TickFlood());

        List<GameObject> floodTilesFullGrass = GetFloodTilesFullGrass();
        AssignFloodTilesFullGrassSprite(floodTilesFullGrass);
        DestroyStructuresOnWater(floodTilesFullGrass);

        List<GameObject> floodTilesPartGrass = GetFloodTilesPartGrass();
        AssignFloodTilesPartGrassSprite(floodTilesPartGrass);

        floodOccured = true;
    }
    public void RevertFlood()
    {
        List<GameObject> revertFloodTilesFullGrass = GetRevertFloodTilesFullGrass();
        AssignRevertFloodTilesFullGrassSprite(revertFloodTilesFullGrass);

        List<GameObject> revertFloodTilesPartGrass = GetRevertFloodTilesPartGrass();
        AssignRevertFloodTilesPartGrassSprite(revertFloodTilesPartGrass);

        Debug.Log("Flood ended!");
        floodIsActive = false;
        StartCoroutine(TickFloodCooldown());
    }

    List<GameObject> GetFloodTilesFullGrass()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                GameObject currentTile = grid[i, j];

                if (currentTile.GetComponent<Tile>().type == 1)
                {
                    RevertGrassCliffToGrass(currentTile);

                    try
                    {
                        GameObject tileOnLeft = grid[i - 1, j];
                        GameObject tileOnRight = grid[i + 1, j];
                        GameObject tileOnTop = grid[i, j + 1];
                        GameObject tileOnBottom = grid[i, j - 1];

                        if (i > 0 && tileOnLeft.GetComponent<Tile>().type == 0) //tileOnLeft
                        {
                            list.Add(currentTile);
                        }
                        if (i < mapGenerator.x - 1 && tileOnRight.GetComponent<Tile>().type == 0)//tile on right
                        {
                            list.Add(currentTile);
                        }
                        if (j > 0 && tileOnBottom.GetComponent<Tile>().type == 0)//tile on bottom
                        {
                            list.Add(currentTile);
                        }
                        if (j < mapGenerator.y - 1 && tileOnTop.GetComponent<Tile>().type == 0) //tileOnTOp
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
    List<GameObject> GetFloodTilesPartGrass()
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

                        if (j > 0 && tileOnBottom.GetComponent<Tile>().type == 0)
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
    void AssignFloodTilesFullGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Tile>().type = 0;
            int randomWaterIndex = Random.Range(0, WaterSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = WaterSprites[randomWaterIndex];
        }
    }
    void AssignFloodTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            int randomGrassCliffIndex = Random.Range(0, GrassCliffSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = GrassCliffSprites[randomGrassCliffIndex];
        }
    }

    void DestroyStructuresOnWater(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            try
            {
                GameObject structureToRemove = mapGenerator.structureGrid[(int)tile.transform.position.x, (int)tile.transform.position.y];
                Destroy(structureToRemove);
            }
            catch (System.NullReferenceException)
            {
                continue;
            }
        }
    }

    List<GameObject> GetRevertFloodTilesFullGrass()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                GameObject currentTile = grid[i, j];

                if (currentTile.GetComponent<Tile>().type == 1)
                {
                    RevertGrassCliffToGrass(currentTile);

                    try
                    {
                        GameObject tileOnLeft = grid[i - 1, j];
                        GameObject tileOnRight = grid[i + 1, j];
                        GameObject tileOnTop = grid[i, j + 1];
                        GameObject tileOnBottom = grid[i, j - 1];

                        if (i > 0 && (tileOnLeft.GetComponent<Tile>().type == 0 ||
                        GrassCliffSprites.Contains(tileOnLeft.GetComponent<SpriteRenderer>().sprite)))
                        {
                            list.Add(tileOnLeft);
                        }
                        if (i < mapGenerator.x - 1 && (tileOnRight.GetComponent<Tile>().type == 0 ||
                        GrassCliffSprites.Contains(tileOnRight.GetComponent<SpriteRenderer>().sprite)))
                        {
                            list.Add(tileOnRight);
                        }
                        if (j > 0 && (tileOnBottom.GetComponent<Tile>().type == 0 ||
                        GrassCliffSprites.Contains(tileOnBottom.GetComponent<SpriteRenderer>().sprite)))
                        {
                            list.Add(tileOnBottom);
                        }
                        if (j < mapGenerator.y - 1 && (tileOnTop.GetComponent<Tile>().type == 0 ||
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

    List<GameObject> GetRevertFloodTilesPartGrass()
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

                        if (j > 0 && tileOnBottom.GetComponent<Tile>().type == 0)
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
    void AssignRevertFloodTilesFullGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Tile>().type = 1;
            int randomGrassIndex = Random.Range(0, GrassSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = GrassSprites[randomGrassIndex];
        }
    }

    void AssignRevertFloodTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            int randomGrassCliffIndex = Random.Range(0, GrassCliffSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = GrassCliffSprites[randomGrassCliffIndex];
        }
    }
    void RevertGrassCliffToGrass(GameObject currentTile)
    {
        if (GrassCliffSprites.Contains(currentTile.GetComponent<SpriteRenderer>().sprite))
        {   
            int randomGrassIndex = Random.Range(0, GrassSprites.Count);
            currentTile.GetComponent<SpriteRenderer>().sprite = GrassSprites[randomGrassIndex];
        }
    }
}