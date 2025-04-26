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
            if(droughtIsActive && counter == Random.Range(960, 1440)) //960, 1440, 4-6 minutes, because tickLength = 0.25 seconds
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
            if(droughtOccured && droughtCooldown > 0)
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
        List<GameObject> droughtTilesFullGrass = new List<GameObject>();
        List<GameObject> droughtTilesPartGrass = new List<GameObject>();
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j] == null) continue;
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    //DroughtTilesFullGrass
                    if(i > 0 && grid[i - 1, j] != null && (grid[i - 1, j].GetComponent<Tile>().type == 0 || grid[i - 1, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4])) //tileOnLeft
                    {
                        droughtTilesFullGrass.Add(grid[i - 1, j]);  
                    }
                    if(i < mapGenerator.x - 1 && grid[i + 1, j] != null && (grid[i + 1, j].GetComponent<Tile>().type == 0 || grid[i + 1, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))//tile on right
                    {
                        droughtTilesFullGrass.Add(grid[i + 1, j]);                 
                    }
                    if(j > 0 && grid[i, j - 1] != null && (grid[i, j - 1].GetComponent<Tile>().type == 0 || grid[i, j - 1].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))//tile on bottom
                    {
                        droughtTilesFullGrass.Add(grid[i, j - 1]);                 
                    }
                    if(j < mapGenerator.y - 1 && grid[i, j + 1] != null && (grid[i, j + 1].GetComponent<Tile>().type == 0 || grid[i, j + 1].GetComponent<SpriteRenderer>().sprite == TileSprites[4])) //tileOnTOp
                    {        
                        droughtTilesFullGrass.Add(grid[i, j + 1]);                 
                    }
                }
            
            }
        }
        foreach(GameObject tile in droughtTilesFullGrass)
        {
            tile.GetComponent<Tile>().type = 1;
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
        }
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j] == null) continue;
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    //DroughtTilesPartGrass
                    if(j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0)
                    {
                        droughtTilesPartGrass.Add(grid[i, j]);
                    }
                }
            }
        }
        foreach(GameObject tile in droughtTilesPartGrass)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
        }
        droughtOccured = true;
    }
    public void RevertDrought()
    {
        List<GameObject> revertDroughtTilesFullGrass = new List<GameObject>();
        List<GameObject> revertDroughtTilesPartGrass = new List<GameObject>();
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 0)
                {
                    //RevertDroughtTilesFullGrass
                    if(i > 0 && grid[i - 1, j] != null && grid[i - 1, j].GetComponent<Tile>().type == 1) //tileOnLeft
                    {
                        revertDroughtTilesFullGrass.Add(grid[i - 1, j]);
                    }
                    if(i < mapGenerator.x - 1 && grid[i + 1, j] != null && grid[i + 1, j].GetComponent<Tile>().type == 1)//tile on right
                    {
                        revertDroughtTilesFullGrass.Add(grid[i + 1, j]);
                    }
                    if(j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 1)//tile on bottom
                    {
                        revertDroughtTilesFullGrass.Add(grid[i, j - 1]);
                    }
                    if(j < mapGenerator.y - 1 && grid[i, j + 1] != null && grid[i, j + 1].GetComponent<Tile>().type == 1) //tileOnTOp
                    {       
                        revertDroughtTilesFullGrass.Add(grid[i, j + 1]);
                    }
                }
            }
        }
        foreach(GameObject tile in revertDroughtTilesFullGrass)
        {
            tile.GetComponent<Tile>().type = 0;
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[0];
        }
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 0)
                {
                    //RevertDroughtTilesPartGrass
                    if(j < mapGenerator.y - 1 && grid[i, j + 1] != null && grid[i, j + 1].GetComponent<Tile>().type == 1)
                    {
                        revertDroughtTilesPartGrass.Add(grid[i, j + 1]);
                    }
                }
            }
        }
        foreach(GameObject tile in revertDroughtTilesPartGrass)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
        }
        Debug.Log("Drought ended!");
        droughtIsActive = false;
        StartCoroutine(TickDroughtCooldown());
    }  
}