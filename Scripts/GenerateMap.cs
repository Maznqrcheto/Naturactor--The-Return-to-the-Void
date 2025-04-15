using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    [SerializeField] int x;
    [SerializeField] int y;
    public List<Sprite> TileSprites = new List<Sprite>();

    [SerializeField] int lakeCountMin;
    [SerializeField] int lakeCountMax;
    [SerializeField] int lakeIntensityMin;
    [SerializeField] int lakeIntensityMax;
    [SerializeField] int lakeLengthMin;
    [SerializeField] int lakeLengthMax;

    public GameObject[,] grid; //Gameobject.Find() is extremely slow so this is an optimisation technique (put all tiles in a matrix beforehand)

    void Start()
    {
        grid = new GameObject[x, y];
        GenerateMapFromScratch();
    }
    public void GenerateMapFromScratch()
    {
        GameObject mapParent = new GameObject();
        mapParent.name = "TileParent";

        //Generate all tiles
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                GameObject currentTile = new GameObject();
                currentTile.transform.parent = mapParent.transform;
                currentTile.transform.position = new Vector2(i, j);

                currentTile.AddComponent<SpriteRenderer>();
                currentTile.AddComponent<Tile>();
                currentTile.name = $"{i},{j}";

                currentTile.GetComponent<Tile>().type = 1;
                currentTile.GetComponent<SpriteRenderer>().sprite = TileSprites[1];
                grid[i, j] = currentTile;
            }
        }

        

        //Lake generation algorithm: Slow but Simple and easily modifiable
        for(int i = 0; i < Random.Range(lakeCountMin, lakeCountMax); i++)
        {    
            Vector2 startingPos = new Vector2(Random.Range(0, x), Random.Range(0, y)); //Get starting pos for lake

            GameObject startingTile = grid[(int)startingPos.x, (int)startingPos.y];

            for (int j = 0; j < Random.Range(lakeIntensityMin, lakeIntensityMax); j++) //Iterate a few times in random directions to get the circular effect of a lake
            {
                Vector2 currentPos = startingPos;
                for (int k = 0; k < Mathf.Pow(20, Random.Range(lakeLengthMin, lakeLengthMax)); k++) //Iterate here too
                {
                    try
                    {
                        GameObject currentTile = grid[(int)currentPos.x, (int)currentPos.y];

                        if (currentTile != null)
                        {
                            currentTile.GetComponent<SpriteRenderer>().sprite = TileSprites[0];
                            currentTile.GetComponent<Tile>().type = 0;
                        }
                        if (currentPos.y < y - 1 && grid[(int)currentPos.x, (int)currentPos.y + 1].GetComponent<Tile>().type == 1)
                        {
                            grid[(int)currentPos.x, (int)currentPos.y + 1].GetComponent<SpriteRenderer>().sprite = TileSprites[2];
                        }
                    }
                    catch
                    {
                        //Debug.Log("Lake out of bounds");
                    }

                    Vector2 directionToGo = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
                    currentPos += directionToGo;

                }
            }
        }

        
    }
    
}
