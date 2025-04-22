using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Drought : MonoBehaviour
{
    public GenerateMap mapGenerator;
    public bool droughtIsActive = false;
    public bool droughtOccured = false;
    public GameObject[,] grid;
    public List<Sprite> TileSprites;

    void Awake()
    {
        TileSprites = mapGenerator.TileSprites;
        grid = mapGenerator.grid;
    }
    public void StartDrought()
    {
        if (grid == null)
    {
        Debug.LogError("Drought.cs — grid is NULL при стартиране!");
        return;
    }

    if (mapGenerator == null)
    {
        Debug.LogError("Drought.cs — mapGenerator is NULL!");
        return;
    }
        droughtIsActive = true;
        for(int i = 0; i < mapGenerator.x; i++)
        {
            for(int j = 0; j < mapGenerator.y; j++)
            {
            if (grid[i, j] == null) continue;
            if (grid[i, j].GetComponent<Tile>().type == 1)
            {
                if(i > 0 && grid[i - 1, j] != null && grid[i - 1, j].GetComponent<Tile>().type == 0) //tileOnLeft
                {
                    grid[i, j].GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
                }
                if(i < mapGenerator.x - 1 && grid[i + 1, j] != null && grid[i + 1, j].GetComponent<Tile>().type == 0)//tile on right
                {
                    grid[i, j].GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
                }
                if(j > 0 && grid[i, j - 1] != null && grid[i, j - 1].GetComponent<Tile>().type == 0)//tile on bottom
                {
                    grid[i, j].GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
                }
                if(j < mapGenerator.y - 1 && grid[i, j + 1] != null && grid[i, j + 1].GetComponent<Tile>().type == 0) //tileOnTOp
                {              
                    grid[i, j].GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
                }
            }

            
            }
        }
        droughtOccured = true;
    }   
}