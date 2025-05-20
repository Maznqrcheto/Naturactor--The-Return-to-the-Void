using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fire : MapValues
{
    public bool fireIsActive = false;
    public bool fireOccured = false;
    public List<Sprite> TileSprites;
    public List<Sprite> StructureSprites;
    public List<Sprite> FireTileSprites;
    public List<Sprite> FireStructureSprites;
    public TickSystem tickSystem;
    public int counter = 1;
    public float counterTickLength;
    public int fireCooldown = 2400; //2400 = 10 minutes
    public void Initialize()
    {
        TileSprites = mapGenerator.TileSprites;
        grid = mapGenerator.grid;
        structureGrid = mapGenerator.structureGrid;
        StructureSprites = mapGenerator.StructureSprites;
    }
    IEnumerator<object> TickFire()
    {
        while (true)
        {
            if (fireIsActive && counter == Random.Range(30, 50)) //960, 1440, 4-6 minutes, because tickLength = 0.25 seconds
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
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    if (j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0) //sprite za voda
                    {
                        tiles.Add(grid[i, j]);
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
            tile.GetComponent<SpriteRenderer>().sprite = FireTileSprites[Random.Range(2, FireTileSprites.Count)];
            tile.GetComponent<Tile>().isOnFire = true;
        }
    }
    public void AssignFirePartGrassTileSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().sprite = FireTileSprites[Random.Range(0, 2)];
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
            int fireIndex = Random.Range(0, FireStructureSprites.Count);
            structure.GetComponent<SpriteRenderer>().sprite = FireStructureSprites[fireIndex];

            if (fireIndex >= FireStructureSprites.Count / 2 && fireIndex <= FireStructureSprites.Count - 1)
            {
                structure.AddComponent<MarkedForDestruction>();
            }
        }
    }

    List<GameObject> GetRevertFireTilesFullGrass()
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
    List<GameObject> GetRevertFireTilesPartGrass()
    {
        List<GameObject> tiles = new List<GameObject>();
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    if (j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0) //sprite za voda
                    {
                        tiles.Add(grid[i, j]);
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
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
            tile.GetComponent<Tile>().isOnFire = false;
        }
    }
    public void AssignRevertFireTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
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
    public void DestroyStructuresOnFire(List<GameObject> structures)
    {
        foreach (GameObject structure in structures)
        {
            Destroy(structure);
            int x = (int)structure.transform.position.x;
            int y = (int)structure.transform.position.y;
            mapGenerator.structureGrid[x, y] = null;
        }
    }
}

public class MarkedForDestruction : MonoBehaviour
{
    // This class is used to mark structures that are set on fire and should be destroyed
}