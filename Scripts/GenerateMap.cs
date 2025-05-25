using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class GenerateMap : MonoBehaviour
{
    public int x;
    public int y;

    [Space(10)]
    [Header("Sprites")]
    [Space(10)]
    public Sprites spritesGetter;
    public List<Sprite> GrassSprites;
    public List<Sprite> GrassCliffSprites;
    public List<Sprite> WaterSprites;
    public List<Sprite> CoalSprites;
    public List<Sprite> IronSprites;
    public List<Sprite> CopperSprites;
    public List<Sprite> TreeSprites;
    public List<Sprite> NaturactorSprites;
    public List<Sprite> VolcanoSprites;
    public List<Sprite> FlowerSprites;
    public List<Sprite> MushroomSprites;
    public List<Sprite> FishSprites;
    public List<Sprite> FishArisingSprites;
    public List<Sprite> SandSprites;
    public List<Sprite> SandWithWaveOnTopSprites;
    public List<Sprite> SandWithWaveOnBottomSprites;
    public List<Sprite> SandWithWaveOnLeftSprites;
    public List<Sprite> SandWithWaveOnRightSprites;

    public List<Sprite> GrassyVegetationSprites;

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
        structureGrid = new GameObject[x, y];
        SetSprites();
        AddSpriteListsToComplexLists();
    }
    void Start()
    {
        //Generate tiles (grass, water and veins)
        GenerateMapTiles();

        //Generate structures (Forests, volcanoes, etc..)
        GenerateMapStructures();

        UpdateSortingOrderForStructures();
    }

    void SetSprites()
    {
        GrassSprites = spritesGetter.GrassSprites;
        GrassCliffSprites = spritesGetter.GrassCliffSprites;
        WaterSprites = spritesGetter.WaterSprites;
        CoalSprites = spritesGetter.CoalSprites;
        IronSprites = spritesGetter.IronSprites;
        CopperSprites = spritesGetter.CopperSprites;
        TreeSprites = spritesGetter.TreeSprites;
        NaturactorSprites = spritesGetter.NaturactorSprites;
        VolcanoSprites = spritesGetter.VolcanoSprites;
        FlowerSprites = spritesGetter.FlowerSprites;
        MushroomSprites = spritesGetter.MushroomSprites;
        FishSprites = spritesGetter.FishSprites;
        FishArisingSprites = spritesGetter.FishArisingSprites;
        SandSprites = spritesGetter.SandSprites;
        SandWithWaveOnTopSprites = spritesGetter.SandWithWaveOnTopSprites;
        SandWithWaveOnBottomSprites = spritesGetter.SandWithWaveOnBottomSprites;
        SandWithWaveOnLeftSprites = spritesGetter.SandWithWaveOnLeftSprites;
        SandWithWaveOnRightSprites = spritesGetter.SandWithWaveOnRightSprites;
    }
    void AddSpriteListsToComplexLists()
    {
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(GrassSprites);
        GrassyVegetationSprites.AddRange(FlowerSprites);
        GrassyVegetationSprites.AddRange(MushroomSprites);
    }
    void GenerateMapTiles()
    {
        //Create tiles
        grid = CreateTilesAndPopulateGrid();

        //Call resource vein generator
        CreateVeins();

        //Lake generation algorithm: Slow but Simple and easily modifiable
        GenerateWaterAlgorithm();

    }
    GameObject[,] CreateTilesAndPopulateGrid()
    {
        GameObject[,] currentGrid = new GameObject[x, y];
        GameObject mapParent = new GameObject();
        mapParent.name = "TileParent";

        //Generate all tiles
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject currentTile = new GameObject();
                currentTile.transform.parent = mapParent.transform;
                currentTile.transform.position = new Vector2(i, j);

                currentTile.AddComponent<SpriteRenderer>();
                currentTile.AddComponent<Tile>();
                currentTile.name = $"{i},{j}";

                currentTile.GetComponent<Tile>().type = 1;
                currentTile.GetComponent<SpriteRenderer>().sprite = GrassyVegetationSprites[Random.Range(0, GrassyVegetationSprites.Count)];
                currentGrid[i, j] = currentTile;
            }
        }
        return currentGrid;
    }

    void CreateVeins()
    {
        //Coal generation
        for (int i = 0; i < Random.Range(coalCountMin, coalCountMax); i++)
            GenerateVeinAlgorithm(Random.Range(coalLengthMin, coalLenghtMax), coalWidthMin, coalWidthMax, CoalSprites[Random.Range(0, CoalSprites.Count)], 2);

        //Iron generation
        for (int i = 0; i < Random.Range(ironCountMin, ironCountMax); i++)
            GenerateVeinAlgorithm(Random.Range(ironLengthMin, ironLenghtMax), ironWidthMin, ironWidthMax, IronSprites[Random.Range(0, IronSprites.Count)], 3);

        //Copper generation
        for (int i = 0; i < Random.Range(copperCountMin, copperCountMax); i++)
            GenerateVeinAlgorithm(Random.Range(copperLengthMin, copperLenghtMax), copperWidthMin, copperWidthMax, CopperSprites[Random.Range(0, CopperSprites.Count)], 4);
    }
    void GenerateVeinAlgorithm(int length, int widthMin, int widthMax, Sprite sprite, int type)
    {
        //Get starting position
        Vector2 startingPosOfVein = new Vector2(Random.Range(0, x), Random.Range(0, y));

        //2 ways to generate a vein, going up and going right
        int randomAlgorhytm = Random.Range(0, 2);

        GameObject[] tilesToChange = new GameObject[0];

        //Get the tiles to change based on 2 different algorithms
        if (randomAlgorhytm == 0)
            tilesToChange = GetTilesForVeinFirstWay(length, startingPosOfVein, widthMin, widthMax);
        else if (randomAlgorhytm == 1)
            tilesToChange = GetTilesForVeinSecondWay(length, startingPosOfVein, widthMin, widthMax);

        //Update all the tiles
        foreach(GameObject tile in tilesToChange)
        {
            tile.GetComponent<Tile>().type = type;
            tile.GetComponent<SpriteRenderer>().sprite = sprite;
        }

    }
    GameObject[] GetTilesForVeinFirstWay(int length, Vector2 startingPosOfVein, int widthMin, int widthMax)
    {
        List<GameObject> tilesToChange = new List<GameObject>();
        for (int i = 0; i < length; i++)
        {
            startingPosOfVein.y += (int)(Random.Range(-20, 21) / 10);
            try
            {
                tilesToChange.Add(grid[i + (int)startingPosOfVein.x, (int)startingPosOfVein.y]);
                int width = Random.Range(widthMin, widthMax);

                for (int j = 1; j < width; j++)
                    tilesToChange.Add(grid[i + (int)startingPosOfVein.x, j + (int)startingPosOfVein.y]);
                for (int j = -1; j > -width; j--)
                    tilesToChange.Add(grid[i + (int)startingPosOfVein.x, j + (int)startingPosOfVein.y]);

            }
            catch { } //Out of bounds
        }
        return tilesToChange.ToArray();
    }
    GameObject[] GetTilesForVeinSecondWay(int length, Vector2 startingPosOfVein, int widthMin, int widthMax)
    {
        List<GameObject> tilesToChange = new List<GameObject>();
        for (int i = 0; i < length; i++)
        {
            startingPosOfVein.x += (int)(Random.Range(-20, 21) / 10);
            try
            {
                tilesToChange.Add(grid[(int)startingPosOfVein.x, i + (int)startingPosOfVein.y]);

                for (int j = 1; j < Random.Range(widthMin, widthMax); j++)
                    tilesToChange.Add(grid[j + (int)startingPosOfVein.x, i + (int)startingPosOfVein.y]);
                for (int j = -1; j > -Random.Range(widthMin, widthMax); j--)
                    tilesToChange.Add(grid[j + (int)startingPosOfVein.x, i + (int)startingPosOfVein.y]);

            }
            catch{} //Out of bounds
        }
        return tilesToChange.ToArray();
    }

    void GenerateWaterAlgorithm()
    {
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
                            currentTile.GetComponent<SpriteRenderer>().sprite = WaterSprites[Random.Range(0, WaterSprites.Count)];
                            currentTile.GetComponent<Tile>().type = 0;
                        }
                        if (currentPos.y < y - 1 && grid[(int)currentPos.x, (int)currentPos.y + 1].GetComponent<Tile>().type == 1)
                        {
                            grid[(int)currentPos.x, (int)currentPos.y + 1].GetComponent<SpriteRenderer>().sprite = GrassCliffSprites[Random.Range(0, GrassCliffSprites.Count)];
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
            RemoveGrassTilesSurroundedByWater();
        }
    }
    void RemoveGrassTilesSurroundedByWater()
    {
        for (int i = 1; i < x - 1; i++)
        {
            for (int j = 1; j < y - 1; j++)
            {
                if (grid[i, j].GetComponent<Tile>().type == 1)
                {
                    GameObject currentTile = grid[i, j];
                    GameObject leftTile = grid[i - 1, j];
                    GameObject rightTile = grid[i + 1, j];
                    GameObject upTile = grid[i, j + 1];
                    GameObject downTile = grid[i, j - 1];
                    if (upTile.GetComponent<Tile>().type == 0
                    && downTile.GetComponent<Tile>().type == 0
                    && rightTile.GetComponent<Tile>().type == 0
                    && leftTile.GetComponent<Tile>().type == 0)
                    {
                        currentTile.GetComponent<Tile>().type = 0;
                        currentTile.GetComponent<SpriteRenderer>().sprite = WaterSprites[Random.Range(0, WaterSprites.Count)];
                    }
                }
            }
        }
    }

    void GenerateMapStructures()
    {
        GenerateForestStructure();

        GenerateVolcanoes();

        GenerateReactor();

        GetComponent<ShadowManager>().CreateShadowForEveryObject();
    }
    void GenerateForestStructure()
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
                            currentTree.GetComponent<SpriteRenderer>().sprite = TreeSprites[Random.Range(0, TreeSprites.Count)];
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
    void GenerateVolcanoes()
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
            volcano.GetComponent<SpriteRenderer>().sprite = VolcanoSprites[Random.Range(0, VolcanoSprites.Count)];
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

    void GenerateReactor()
    {
        Vector2 reactorPosition = new Vector2(Random.Range(0, x), Random.Range(0, y));
        while (!gameObject.GetComponent<PlaceMachine>().CheckIfCanPlace(new Vector2(reactorPosition.x-5, reactorPosition.y-5), 12, 12))
        {
            reactorPosition = new Vector2(Random.Range(0, x), Random.Range(0, y));
        }
        GameObject reactor = new GameObject("reactor");
        reactor.transform.parent = GameObject.Find("BuildingParent").transform;

        reactor.AddComponent<Machine>();
        reactor.GetComponent<Machine>().type = -1;
        reactor.GetComponent<Machine>().hasInput = false;
        reactor.GetComponent<Machine>().hasOutput = false;
        reactor.GetComponent<Machine>().UpdateInventorySize();

        reactor.AddComponent<SpriteRenderer>();
        reactor.GetComponent<SpriteRenderer>().sprite = NaturactorSprites[Random.Range(0, NaturactorSprites.Count)];

        reactor.AddComponent<Structure>();
        reactor.GetComponent<Structure>().type = 1;
        reactor.GetComponent<Structure>().position = new Vector2(reactorPosition.x, reactorPosition.y);

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
