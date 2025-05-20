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

        CheckAllGrassTiles();
        CheckAllStructures();
        
        fireOccured = true;
    }
    public void RevertFire()
    {

        Debug.Log("Fire ended!");
        fireIsActive = false;
        StartCoroutine(TickFireCooldown());
    }
    public void CheckAllGrassTiles()
    {
        Debug.Log("CheckAllGrassTiles");
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    //TO DO add list with grass sprites and fire grass sprites => Randomize the sprites for tile generation
                    if (grid[i, j] == null) continue;
                    if (j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0) //sprite za voda
                    {
                        grid[i, j].GetComponent<SpriteRenderer>().sprite = FireTileSprites[Random.Range(0, 2)];
                    }
                    else
                    {
                        grid[i, j].GetComponent<SpriteRenderer>().sprite = FireTileSprites[Random.Range(2, FireTileSprites.Count)];
                        grid[i, j].GetComponent<Tile>().isOnFire = true;
                    }
                }
            }
        }
        Debug.Log("CheckAllGrassTiles end");
    }
    public void CheckAllStructures()
    {
        Debug.Log("CheckAllStructures");
        for (int i = 0; i < mapGenerator.x; i++)
        {
            for (int j = 0; j < mapGenerator.y; j++)
            {
                try
                {
                    GameObject structureToRemove = mapGenerator.structureGrid[(int)structureGrid[i, j].transform.position.x, (int)structureGrid[i, j].transform.position.y];
                    if (structureToRemove.GetComponent<Structure>() != null)
                    {
                        Debug.Log("CheckedGetComponent<Structure>() != null");
                        if (structureToRemove.GetComponent<Structure>().type == 0) // tree
                        {
                        
                            structureToRemove.GetComponent<SpriteRenderer>().sprite = FireStructureSprites[Random.Range(0, FireStructureSprites.Count)];
                            Debug.Log("Assigned fire structure sprite");
                        }
                    }
                }
                catch
                {
                    //Debug.Log("No structure to remove");
                }              
            }
        }
        Debug.Log("CheckAllStructures end");
    }

}