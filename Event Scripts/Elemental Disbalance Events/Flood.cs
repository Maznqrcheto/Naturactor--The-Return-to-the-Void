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
    public List<Sprite> StructureSprites;
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
                        list.Add(grid[i, j]);  
                    }
                    if(i < mapGenerator.x - 1 && grid[i + 1, j] != null && grid[i + 1, j].GetComponent<Tile>().type == 0)//tile on right
                    {
                        list.Add(grid[i, j]);                 
                    }
                    if(j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0)//tile on bottom
                    {
                        list.Add(grid[i, j]);                 
                    }
                    if(j < mapGenerator.y - 1 && grid[i, j + 1] != null && grid[i, j + 1].GetComponent<Tile>().type == 0) //tileOnTOp
                    {        
                        list.Add(grid[i, j]);                 
                    }
                }
            }
        }
        return list;    
    }
    List<GameObject> GetFloodTilesPartGrass()
    {
        List<GameObject> list = new List<GameObject>();
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
                        list.Add(grid[i, j]);                 
                    }
                }
            }
        }
        return list;
    }
    void AssignFloodTilesFullGrassSprite(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles)
        {
            tile.GetComponent<Tile>().type = 0;
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[0];
        }
    }
    void AssignFloodTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
        }
    }

    void DestroyStructuresOnWater(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles)
        {
            try
            {
                GameObject structureToRemove = mapGenerator.structureGrid[(int)tile.transform.position.x, (int)tile.transform.position.y];
                Destroy(mapGenerator.structureGrid[(int)tile.transform.position.x, (int)tile.transform.position.y]);
            }
            catch
            {
                //structure does not exist
            }
        }
    }

    List<GameObject> GetRevertFloodTilesFullGrass()
    {
        List<GameObject> list = new List<GameObject>();
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
                        list.Add(grid[i - 1, j]);
                    }
                    if(i < mapGenerator.x - 1 && grid[i + 1, j] != null && (grid[i + 1, j].GetComponent<Tile>().type == 0 || grid[i + 1, j].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))//tile on right
                    {
                        list.Add(grid[i + 1, j]);
                    }
                    if(j > 0 && grid[i, j - 1] != null && (grid[i, j - 1].GetComponent<Tile>().type == 0 || grid[i, j - 1].GetComponent<SpriteRenderer>().sprite == TileSprites[4]))//tile on bottom
                    {
                        list.Add(grid[i, j - 1]);
                    }
                    if(j < mapGenerator.y - 1 && grid[i, j + 1] != null && (grid[i, j + 1].GetComponent<Tile>().type == 0 || grid[i, j + 1].GetComponent<SpriteRenderer>().sprite == TileSprites[4])) //tileOnTOp
                    {       
                        list.Add(grid[i, j + 1]);
                    }
                }
            }
        }
        return list;    
    }

    List<GameObject> GetRevertFloodTilesPartGrass()
    {
        List<GameObject> list = new List<GameObject>();
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    //RevertFloodTilesPartGrass
                    if(j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0)
                    {
                        list.Add(grid[i, j]);
                    }
                }
            }
        }
        return list;
    }
    void AssignRevertFloodTilesFullGrassSprite(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles)
        {
            tile.GetComponent<Tile>().type = 1;
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
        }
    }

    void AssignRevertFloodTilesPartGrassSprite(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().sprite = TileSprites[4];
        }
    }
    void DestroyStructuresOnWater()
    {

    }
}