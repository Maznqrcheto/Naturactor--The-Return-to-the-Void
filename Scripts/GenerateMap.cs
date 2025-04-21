using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateMap : MonoBehaviour
{
    public int x;
    public int y;
    public List<Sprite> TileSprites = new List<Sprite>();
    public List<Sprite> StructureSprites = new List<Sprite>();

    [Space(10)]
    [Header("Lake Generation")]
    [Space(10)]
    [SerializeField] int lakeCountMin;
    [SerializeField] int lakeCountMax;
    [SerializeField] int lakeIntensityMin;
    [SerializeField] int lakeIntensityMax;
    [SerializeField] int lakeLengthMin;
    [SerializeField] int lakeLengthMax;

    [Space(10)]
    [Header("Forest Generation")]
    [Space(10)]
    [SerializeField] int forestCountMin;
    [SerializeField] int forestCountMax;
    [SerializeField] int forestIntensityMin;
    [SerializeField] int forestIntensityMax;
    [SerializeField] int forestLengthMin;
    [SerializeField] int forestLengthMax;

    [Space(10)]
    [Header("Coal Generation")]
    [Space(10)]
    [SerializeField] int coalCountMin;
    [SerializeField] int coalCountMax;
    [SerializeField] int coalLengthMin;
    [SerializeField] int coalLenghtMax;
    [SerializeField] int coalWidthMin;
    [SerializeField] int coalWidthMax;

    [Space(10)]
    [Header("Iron Generation")]
    [Space(10)]
    [SerializeField] int ironCountMin;
    [SerializeField] int ironCountMax;
    [SerializeField] int ironLengthMin;
    [SerializeField] int ironLenghtMax;
    [SerializeField] int ironWidthMin;
    [SerializeField] int ironWidthMax;

    [Space(10)]
    [Header("Copper Generation")]
    [Space(10)]
    [SerializeField] int copperCountMin;
    [SerializeField] int copperCountMax;
    [SerializeField] int copperLengthMin;
    [SerializeField] int copperLenghtMax;
    [SerializeField] int copperWidthMin;
    [SerializeField] int copperWidthMax;

    [Space(10)]
    [Header("Volcano Generation")]
    [Space(10)]
    [SerializeField] int volcanoCountMin;
    [SerializeField] int volcanoCountMax;
    
    public GameObject[,] grid; //Gameobject.Find() is extremely slow so this is an optimisation technique (put all tiles in a matrix beforehand)
    public GameObject[,] structureGrid; // Grid for the structures like forests, mountains and others.
    private void Awake()
    {
        grid = new GameObject[x, y];
        structureGrid = new GameObject[x, y];
    }
    void Start()
    {
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
                currentTile.GetComponent<SpriteRenderer>().sprite = TileSprites[Random.Range(1, 4)];
                grid[i, j] = currentTile;
            }
        }
        //Coal generation
        for (int i = 0; i < Random.Range(coalCountMin, coalCountMax); i++)
        {
            GenerateVein(Random.Range(coalLengthMin, coalLenghtMax), coalWidthMin, coalWidthMax, TileSprites[5], 2);
        }
        //Iron generation
        for (int i = 0; i < Random.Range(ironCountMin, ironCountMax); i++)
        {
            GenerateVein(Random.Range(ironLengthMin, ironLenghtMax), ironWidthMin, ironWidthMax, TileSprites[6], 3);
        }
        //Copper generation
        for(int i = 0; i < Random.Range(copperCountMin, copperCountMax); i++)
        {
            GenerateVein(Random.Range(copperLengthMin, copperLenghtMax), copperWidthMin, copperWidthMax, TileSprites[7], 4);
        }
        //Lake generation algorithm: Slow but Simple and easily modifiable
        for (int i = 0; i < Random.Range(lakeCountMin, lakeCountMax); i++)
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
                            grid[(int)currentPos.x, (int)currentPos.y + 1].GetComponent<SpriteRenderer>().sprite = TileSprites[4];
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
        //Remove grass tiles surrounded by water
        for (int i = 1; i < x - 1; i++)
        {
            for (int j = 1; j < y - 1; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    if ( grid[i, j + 1].GetComponent<Tile>().type == 0
                    && grid[i, j - 1].GetComponent<Tile>().type == 0
                    && grid[i + 1, j].GetComponent<Tile>().type == 0
                    && grid[i - 1, j].GetComponent<Tile>().type == 0)
                    {
                        grid[i, j].GetComponent<Tile>().type = 0;
                        grid[i, j].GetComponent<SpriteRenderer>().sprite = TileSprites[0];
                    }
                }
            }
        }
       

    }
    public void GenerateVein(int length, int widthMin, int widthMax, Sprite sprite, int type)
    {
        Vector2 startingPosOfVein = new Vector2(Random.Range(0, x), Random.Range(0, y));
        //2 ways to generate a vein, going up and going right
        int randomAlgorhytm = Random.Range(0, 2);

        for (int i = 0; i < length; i++)
        {
            //Going right
            if (randomAlgorhytm == 0)
            {
                startingPosOfVein.y += (int)(Random.Range(-20, 21) / 10);
                try
                {
                    grid[i + (int)startingPosOfVein.x, (int)startingPosOfVein.y].GetComponent<Tile>().type = type;
                    grid[i + (int)startingPosOfVein.x, (int)startingPosOfVein.y].GetComponent<SpriteRenderer>().sprite = sprite;
                    int width = Random.Range(widthMin, widthMax);
                    for (int j = 1; j < width; j++)
                    {
                        grid[i + (int)startingPosOfVein.x, j + (int)startingPosOfVein.y].GetComponent<Tile>().type = type;
                        grid[i + (int)startingPosOfVein.x, j + (int)startingPosOfVein.y].GetComponent<SpriteRenderer>().sprite = sprite;
                    }
                    for (int j = -1; j > -width; j--)
                    {
                        grid[i + (int)startingPosOfVein.x, j + (int)startingPosOfVein.y].GetComponent<Tile>().type = type;
                        grid[i + (int)startingPosOfVein.x, j + (int)startingPosOfVein.y].GetComponent<SpriteRenderer>().sprite = sprite;
                    }
                }
                catch
                {
                    //Out of bounds
                }
            }
            //Going up
            else if (randomAlgorhytm == 1)
            {
                startingPosOfVein.x += (int)(Random.Range(-20, 21) / 10);
                try
                {
                    grid[(int)startingPosOfVein.x, i + (int)startingPosOfVein.y].GetComponent<Tile>().type = type;
                    grid[(int)startingPosOfVein.x, i + (int)startingPosOfVein.y].GetComponent<SpriteRenderer>().sprite = sprite;
                    for (int j = 1; j < Random.Range(widthMin, widthMax); j++)
                    {
                        grid[j + (int)startingPosOfVein.x, i + (int)startingPosOfVein.y].GetComponent<Tile>().type = type;
                        grid[j + (int)startingPosOfVein.x, i + (int)startingPosOfVein.y].GetComponent<SpriteRenderer>().sprite = sprite;
                    }
                    for (int j = -1; j > -Random.Range(widthMin, widthMax); j--)
                    {
                        grid[j + (int)startingPosOfVein.x, i + (int)startingPosOfVein.y].GetComponent<Tile>().type = type;
                        grid[j + (int)startingPosOfVein.x, i + (int)startingPosOfVein.y].GetComponent<SpriteRenderer>().sprite = sprite;
                    }
                }
                catch
                {
                    //Out of bounds
                }
            }
        }

    }
    public void GenerateStructuresFromScratch()
    {
        GenerateForestStructure();
        GenerateVolcanoes();
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
                            currentTree.GetComponent<Structure>().position = new Vector2(currentPos.x, currentPos.y);
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
    public void GenerateVolcanoes()
    {
        int volcanoCount = Random.Range(volcanoCountMin, volcanoCountMax);
        for (int i = 0; i < volcanoCount; i++)
        {
            Vector2 volcanoPosition = new Vector2();
            while (!gameObject.GetComponent<PlaceMachine>().CheckIfCanPlace(new Vector2(volcanoPosition.x - 5, volcanoPosition.y - 4), 10, 8))
            {
                volcanoPosition = new Vector2(Random.Range(5, x - 5), Random.Range(4, y - 4));
            }
            GameObject volcano = new GameObject("volcano");
            volcano.AddComponent<SpriteRenderer>();
            volcano.AddComponent<Structure>();
            volcano.GetComponent<Structure>().type = 5;
            volcano.GetComponent<Structure>().position = new Vector2(volcanoPosition.x, volcanoPosition.y);
            volcano.GetComponent<SpriteRenderer>().sprite = StructureSprites[2];
            volcano.transform.position = new Vector2(volcanoPosition.x + 0.5f, volcanoPosition.y + 0.5f);
            for(int j = (int)volcanoPosition.x - 5; j <= (int)volcanoPosition.x + 5; j++)
            {
                for(int k = (int)volcanoPosition.y - 4; k <= (int)volcanoPosition.y + 4; k++)
                {
                    structureGrid[j, k] = volcano;
                }
            }
        }
    }
    public void GenerateReactor()
    {
        Vector2 reactorPosition = new Vector2(Random.Range(0, x), Random.Range(0, y));
        while (!gameObject.GetComponent<PlaceMachine>().CheckIfCanPlace(new Vector2(reactorPosition.x-5, reactorPosition.y-5), 12, 12))
        {
            reactorPosition = new Vector2(Random.Range(0, x), Random.Range(0, y));
        }
        GameObject reactor = new GameObject("reactor");
        reactor.AddComponent<SpriteRenderer>();
        reactor.AddComponent<Structure>();
        reactor.GetComponent<Structure>().type = 1;
        reactor.GetComponent<Structure>().position = new Vector2(reactorPosition.x, reactorPosition.y);
        reactor.GetComponent<SpriteRenderer>().sprite = StructureSprites[1];
        reactor.transform.position = new Vector2(reactorPosition.x + 0.5f, reactorPosition.y + 0.5f); 
        structureGrid[(int)reactorPosition.x, (int)reactorPosition.y] = reactor;
        structureGrid[(int)reactorPosition.x + 1, (int)reactorPosition.y] = reactor;
        structureGrid[(int)reactorPosition.x + 1, (int)reactorPosition.y + 1] = reactor;
        structureGrid[(int)reactorPosition.x, (int)reactorPosition.y + 1] = reactor;
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
