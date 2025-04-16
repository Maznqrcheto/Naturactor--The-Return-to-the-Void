using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    [SerializeField] int x;
    [SerializeField] int y;
    public List<Sprite> TileSprites = new List<Sprite>();
    public List<Sprite> StructureSprites = new List<Sprite>();

    [SerializeField] int lakeCountMin;
    [SerializeField] int lakeCountMax;
    [SerializeField] int lakeIntensityMin;
    [SerializeField] int lakeIntensityMax;
    [SerializeField] int lakeLengthMin;
    [SerializeField] int lakeLengthMax;

    [SerializeField] int forestCountMin;
    [SerializeField] int forestCountMax;
    [SerializeField] int forestIntensityMin;
    [SerializeField] int forestIntensityMax;
    [SerializeField] int forestLengthMin;
    [SerializeField] int forestLengthMax;
    [SerializeField] int reactorCount;
    
    public GameObject[,] grid; //Gameobject.Find() is extremely slow so this is an optimisation technique (put all tiles in a matrix beforehand)
    public GameObject[,] structureGrid; // Grid for the structures like forests, mountains and others.
    void Start()
    {
        grid = new GameObject[x, y];
        structureGrid = new GameObject[x, y];
        GenerateMapFromScratch();
        GenerateStructuresFromScratch();
        UpdateSortingOrderForStructures();
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
    public void GenerateStructuresFromScratch()
    {
        GenerateForestStructure();
        GenerateReactor();
    }
    public void GenerateForestStructure()
    {
        GameObject forestParent = new GameObject();
        forestParent.name = "ForestParent";
        int forestRandomCount = Random.Range(forestCountMin, forestCountMax);
        int forestRandomIntensity = Random.Range(forestIntensityMin, forestIntensityMax);
        int forestRandomLength = Random.Range(forestLengthMin, forestLengthMax);
        
        //Forest generation algorithm, similiar to the lake one
        for (int i = 0; i < forestRandomCount; i++)
        {
            Vector2 startingPos = new Vector2(Random.Range(0, x), Random.Range(0, y)); //Get starting pos for forest
            GameObject startingTile = grid[(int)startingPos.x, (int)startingPos.y];

            for (int j = 0; j < forestRandomIntensity; j++) //Iterate a few times in random directions
            {
                Vector2 currentPos = startingPos;
                for (int k = 0; k < forestRandomLength; k++) //Iterate here too
                {
                    try
                    {
                        if (structureGrid[(int)currentPos.x, (int)currentPos.y] == null
                            && grid[(int)currentPos.x, (int)currentPos.y].GetComponent<Tile>().type == 1)
                        {
                            GameObject currentTree = new GameObject();
                            currentTree.transform.parent = forestParent.transform;
                            Vector2 PositionOffset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                            currentTree.transform.position = new Vector2(currentPos.x, currentPos.y) + PositionOffset;

                            currentTree.AddComponent<SpriteRenderer>();
                            currentTree.AddComponent<Structure>();
                            currentTree.name = $"{currentPos.x},{currentPos.y}";

                            currentTree.GetComponent<Structure>().type = 0;
                            currentTree.GetComponent<SpriteRenderer>().sprite = StructureSprites[0];
                            structureGrid[(int)currentPos.x, (int)currentPos.y] = currentTree;
                        }
                    }
                    catch
                    {
                        //Debug.Log("Forest out of bounds");
                    }

                    Vector2 directionToGo = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
                    currentPos += directionToGo;

                }
            }
        }

        Vector3 treeOffset = forestParent.transform.position;
        treeOffset.y = .8f;
        forestParent.transform.position = treeOffset;
        }
    public void GenerateReactor()
    {
        Vector2 reactorPosition = new Vector2(Random.Range(0, x), Random.Range(0, y));
        while (true)
        {
            if (CanPlaceReactor(reactorPosition)) break;
            else reactorPosition = new Vector2(Random.Range(0, x), Random.Range(0, y));

        }
        GameObject reactor = new GameObject("reactor");
        reactor.AddComponent<SpriteRenderer>();
        reactor.AddComponent<Structure>();
        reactor.GetComponent<Structure>().type = 1;
        reactor.GetComponent<SpriteRenderer>().sprite = StructureSprites[1];
        reactor.transform.position = reactorPosition; 
        structureGrid[(int)reactorPosition.x, (int)reactorPosition.y] = reactor;
        UpdateSortingOrderForStructures();
    }
    public bool CanPlaceReactor(Vector2 reactorPosition)
    {
        if (grid[(int)reactorPosition.x, (int)reactorPosition.y].GetComponent<Tile>().type == 0) return false;
        if (structureGrid[(int)reactorPosition.x, (int)reactorPosition.y] != null) return false;

        return true;
    }
    public void UpdateSortingOrderForStructures()
    {
        for(int i = 0; i < y; i++)
        {
            for(int j = 0; j < x; j++)
            {
                if (structureGrid[j, i] != null)
                    structureGrid[j, i].GetComponent<SpriteRenderer>().sortingOrder = y - i;
            }
        }
    }
}
