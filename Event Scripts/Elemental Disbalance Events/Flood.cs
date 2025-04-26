using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Flood : MonoBehaviour
{
    public GenerateMap mapGenerator;
    public GameObject[,] grid;
    public bool floodIsActive = false;
    public bool floodOccured = false;
    public List<Sprite> TileSprites;
    public TickSystem tickSystem;
    public int counter = 1;
    public float counterTickLength;
    public int floodCooldown = 2400; //2400 = 10 minutes
    void Awake()
    {
        TileSprites = mapGenerator.TileSprites;
        grid = mapGenerator.grid;
    }
    IEnumerator<object> TickFlood()
    {
        while (true)
        {
            if(floodIsActive && counter == Random.Range(30, 50)) //960, 1440, 4-6 minutes, because tickLength = 0.25 seconds
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
            if(floodOccured && floodCooldown > 0)
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
        List<GameObject> floodTilesFullGrass = new List<GameObject>();
        List<GameObject> floodTilesPartGrass = new List<GameObject>();
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j] == null) continue;
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    if (grid[i, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4])
                    {
                        grid[i, j].GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
                    }
                    //FloodTilesFullGrass
                    if(i > 0 && grid[i - 1, j] != null && grid[i - 1, j].GetComponent<Tile>().type == 0) //tileOnLeft
                    {
                        floodTilesFullGrass.Add(grid[i, j]);  
                    }
                    if(i < mapGenerator.x - 1 && grid[i + 1, j] != null && grid[i + 1, j].GetComponent<Tile>().type == 0)//tile on right
                    {
                        floodTilesFullGrass.Add(grid[i, j]);                 
                    }
                    if(j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0)//tile on bottom
                    {
                        floodTilesFullGrass.Add(grid[i, j]);                 
                    }
                    if(j < mapGenerator.y - 1 && grid[i, j + 1] != null && grid[i, j + 1].GetComponent<Tile>().type == 0) //tileOnTOp
                    {        
                        floodTilesFullGrass.Add(grid[i, j]);                 
                    }
                }
            }
        }    
        foreach(GameObject tile in floodTilesFullGrass)
        {
            tile.GetComponent<Tile>().type = 0;
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[0];
        }

        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j] == null) continue;
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    //FloodTilesPartGrass
                    if(j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0)//tile on bottom
                    {
                        floodTilesPartGrass.Add(grid[i, j]);                 
                    }
                }
            }
        }
        foreach(GameObject tile in floodTilesPartGrass)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
        }
        floodOccured = true;
    }
    public void RevertFlood()
    {
        List<GameObject> revertFloodTilesFullGrass = new List<GameObject>();
        List<GameObject> revertFloodTilesPartGrass = new List<GameObject>();
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    if (grid[i, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4])
                    {
                        grid[i, j].GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
                    }
                    //RevertFloodTilesFullGrass
                    if(i > 0 && grid[i - 1, j] != null && (grid[i - 1, j].GetComponent<Tile>().type == 0 || grid[i - 1, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4])) //tileOnLeft
                    {
                        revertFloodTilesFullGrass.Add(grid[i - 1, j]);
                    }
                    if(i < mapGenerator.x - 1 && grid[i + 1, j] != null && (grid[i + 1, j].GetComponent<Tile>().type == 0 || grid[i + 1, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))//tile on right
                    {
                        revertFloodTilesFullGrass.Add(grid[i + 1, j]);
                    }
                    if(j > 0 && grid[i, j - 1] != null && (grid[i, j - 1].GetComponent<Tile>().type == 0 || grid[i, j - 1].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))//tile on bottom
                    {
                        revertFloodTilesFullGrass.Add(grid[i, j - 1]);
                    }
                    if(j < mapGenerator.y - 1 && grid[i, j + 1] != null && (grid[i, j + 1].GetComponent<Tile>().type == 0 || grid[i, j + 1].GetComponent<SpriteRenderer>().sprite == TileSprites[4])) //tileOnTOp
                    {       
                        revertFloodTilesFullGrass.Add(grid[i, j + 1]);
                    }
                }
            }
        }
        foreach(GameObject tile in revertFloodTilesFullGrass)
        {
            tile.GetComponent<Tile>().type = 1;
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
        }
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    //RevertFloodTilesPartGrass
                    if(j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0)
                    {
                        revertFloodTilesPartGrass.Add(grid[i, j]);
                    }
                }
            }
        }
        foreach(GameObject tile in revertFloodTilesPartGrass)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
        }
        Debug.Log("Flood ended!");
        floodIsActive = false;
        StartCoroutine(TickFloodCooldown());
    }
}