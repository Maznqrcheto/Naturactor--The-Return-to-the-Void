using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fire : MapValues
{
    public bool fireIsActive = false;
    public bool fireOccured = false;
    public TickSystem tickSystem;
    public int counter = 1;
    public float counterTickLength;
    public int fireCooldown = 2400; //2400 = 10 minutes

    public List<Sprite> GrassSprites;
    public List<Sprite> GrassCliffSprites;
    public List<Sprite> TreeSprites;
    public List<Sprite> FireTreeSprites;
    public List<Sprite> FireGrassSprites;
    public List<Sprite> FireGrassCliffSprites;

    public List<Sprite> FireAndNormalGrassSprites = new List<Sprite>();
    public List<Sprite> FireAndNormalGrassCliffSprites = new List<Sprite>();
    public List<Sprite> FireAndNormalTreeSprites = new List<Sprite>();
    public void Initialize()
    {
        SetMap();
        SetSpritesForUse();
        AddSpriteListsToComplexLists();    
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
        TreeSprites = spritesGetter.TreeSprites;
        FireTreeSprites = spritesGetter.FireTreeSprites;
        FireGrassSprites = spritesGetter.FireGrassSprites;
        FireGrassCliffSprites = spritesGetter.FireGrassCliffSprites;
    }
    void AddSpriteListsToComplexLists()
    {
        FireAndNormalGrassSprites.AddRange(GrassSprites);
        FireAndNormalGrassSprites.AddRange(FireGrassSprites);
        FireAndNormalGrassCliffSprites.AddRange(GrassCliffSprites);
        FireAndNormalGrassCliffSprites.AddRange(FireGrassCliffSprites);
        FireAndNormalTreeSprites.AddRange(TreeSprites);
        FireAndNormalTreeSprites.AddRange(FireTreeSprites);
    }

    IEnumerator<object> TickFire()
    {
        while (true)
        {
            if (fireIsActive && counter == Random.Range(960, 1440)) //960, 1440, 4-6 minutes, because tickLength = 0.25 seconds
            {
                RevertFire();
                counter = 1;
                yield break;
            }
            counter++;
            yield return new WaitForSeconds(counterTickLength);
        }
    }
    IEnumerator<object> TickFireCooldown()
    {
        while (true)
        {
            if (fireOccured && fireCooldown > 0)
            {
                fireCooldown--;
            }
            else
            {
                fireOccured = false;
                fireCooldown = 2400; //reset cooldown
                yield break;
            }
            yield return new WaitForSeconds(counterTickLength);
        }
    }
    public void StartFire()
    {
        Debug.Log("Fire started!");
        fireIsActive = true;
        StartCoroutine(TickFire());

        List<GameObject> fireTilesFullGrass = GetFireTilesFullGrass();
        List<GameObject> fireTilesPartGrass = GetFireTilesPartGrass();

        AssignFireFullGrassTileSprite(fireTilesFullGrass);
        AssignFirePartGrassTileSprite(fireTilesPartGrass);

        List<GameObject> structuresToSetOnFire = GetFireStructures();
        AssignFireOnStructures(structuresToSetOnFire);

        fireOccured = true;
    }
    public void RevertFire()
    {
        List<GameObject> revertFireTilesFullGrass = GetRevertFireTilesFullGrass();
        AssignRevertFireTilesFullGrassSprite(revertFireTilesFullGrass);

        List<GameObject> revertFireTilesPartGrass = GetRevertFireTilesPartGrass();
        AssignRevertFireTilesPartGrassSprite(revertFireTilesPartGrass);

        List<GameObject> structuresSetOnFire = GetRevertFireStructures();
        List<GameObject> structuresToBeDestroyed = CheckSetOnFireStructures(structuresSetOnFire);
        DestroyStructuresOnFire(structuresToBeDestroyed);

        Debug.Log("Fire ended!");
        fireIsActive = false;
        StartCoroutine(TickFireCooldown());
    }
    List<GameObject> GetFireTilesFullGrass()
    {
        List<GameObject> tiles = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                // GameObject currentTileOfGrass = (grid[i, j].GetComponent<Tile>().type == 1)
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    tiles.Add(grid[i, j]);
                }
            }
        }
        return tiles;
    }
    List<GameObject> GetFireTilesPartGrass()
    {
        List<GameObject> tiles = new List<GameObject>();
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

                        if (j > 0 && tileOnBottom != null && tileOnBottom.GetComponent<Tile>().type == 0) //sprite za voda
                        {
                            tiles.Add(currentTile);
                        }
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }
        }
        return tiles;
    }

    public void AssignFireFullGrassTileSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().sprite = FireAndNormalGrassSprites[Random.Range(0, FireAndNormalGrassSprites.Count)];
            tile.GetComponent<Tile>().isOnFire = true;
        }
    }
    public void AssignFirePartGrassTileSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            int fireRandomIndex = Random.Range(0, FireAndNormalGrassCliffSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = FireAndNormalGrassCliffSprites[fireRandomIndex];
            tile.GetComponent<Tile>().isOnFire = true;
        }
    }

    List<GameObject> GetFireStructures()
    {
        List<GameObject> structures = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                try
                {
                    GameObject structureToBeSetOnFire = mapGenerator.structureGrid[(int)structureGrid[i, j].transform.position.x, (int)structureGrid[i, j].transform.position.y];
                    if (structureToBeSetOnFire.GetComponent<Structure>() != null)
                    {
                        if (structureToBeSetOnFire.GetComponent<Structure>().type == 0) // tree
                        {
                            structures.Add(structureToBeSetOnFire);
                        }
                    }
                }
                catch
                {
                    //Debug.Log("No structure to remove");
                }
            }
        }
        return structures;
    }

    public void AssignFireOnStructures(List<GameObject> structures)
    {
        foreach (GameObject structure in structures)
        {
            int fireRandomIndex = Random.Range(0, FireAndNormalTreeSprites.Count);
            structure.GetComponent<SpriteRenderer>().sprite = FireAndNormalTreeSprites[fireRandomIndex];

            CheckIfAStructureMustBeDestroyed(structure);  
        }
    }
    List<GameObject> GetRevertFireTilesFullGrass()
    {
        List<GameObject> tiles = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                GameObject currentTile = grid[i, j];
                bool currentTileOfGrass = (grid[i, j].GetComponent<Tile>().type == 1);
                if (currentTileOfGrass)
                {
                    tiles.Add(currentTile);
                }
            }
        }
        return tiles;
    }
    List<GameObject> GetRevertFireTilesPartGrass()
    {
        List<GameObject> tiles = new List<GameObject>();
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
                        if (j > 0 && tileOnBottom != null && tileOnBottom.GetComponent<Tile>().type == 0) //sprite za voda
                        {
                            tiles.Add(currentTile);
                        }
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }
        }
        return tiles;
    }

    public void AssignRevertFireTilesFullGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            int grassRandomIndex = Random.Range(0, GrassSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = GrassSprites[grassRandomIndex];
            tile.GetComponent<Tile>().isOnFire = false;
        }
    }
    public void AssignRevertFireTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            int grassCliffRandomIndex = Random.Range(0, GrassCliffSprites.Count);
            tile.GetComponent<SpriteRenderer>().sprite = GrassCliffSprites[grassCliffRandomIndex];
            tile.GetComponent<Tile>().isOnFire = false;
        }
    }

    List<GameObject> GetRevertFireStructures()
    {
        List<GameObject> structures = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                try
                {
                    GameObject structureToRemove = mapGenerator.structureGrid[(int)structureGrid[i, j].transform.position.x, (int)structureGrid[i, j].transform.position.y];
                    if (structureToRemove.GetComponent<Structure>() != null)
                    {
                        if (structureToRemove.GetComponent<Structure>().type == 0) // tree
                        {
                            structures.Add(structureToRemove);
                        }
                    }
                }
                catch
                {
                    //Debug.Log("No structure to remove");
                }
            }
        }
        return structures;
    }
    List<GameObject> CheckSetOnFireStructures(List<GameObject> structures)
    {
        List<GameObject> structuresToBeDestroyed = new List<GameObject>();
        foreach (GameObject structure in structures)
        {
            if (structure.GetComponent<MarkedForDestruction>() != null)
            {
                structuresToBeDestroyed.Add(structure);
            }
        }
        return structuresToBeDestroyed;
    }
    void CheckIfAStructureMustBeDestroyed(GameObject structure)
    {
        if (FireTreeSprites.Contains(structure.GetComponent<SpriteRenderer>().sprite))
        {
            structure.AddComponent<MarkedForDestruction>();
        }
    }
    public void DestroyStructuresOnFire(List<GameObject> structures)
    {
        foreach (GameObject structure in structures)
        {
            Destroy(structure);
        }
    }
}

public class MarkedForDestruction : MonoBehaviour
{

}