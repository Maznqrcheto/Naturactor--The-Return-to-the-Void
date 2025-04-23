using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class PlaceMachine : MonoBehaviour
{
    public GenerateMap genMap;
    public PauseMenu pauseMenu;

    public GameObject currentMachineHologram;
    public Transform buildingParent;

    public List<Factory> factoryTypes;
    public List<Sprite> factorySprites;
    public int selectedfactory = -1;

    public bool isChoppingTrees = false;
    public GameObject objectHoveringOver;

    public int rotation = 0;
    private void Start()
    {
        factoryTypes = new List<Factory>();
        factoryTypes.Add(new Factory(factorySprites[0], new Vector2(-1, -1), new Vector2(1, -1), 0)
        { description = "Useful for mining materials." });
        factoryTypes.Add(new Factory(factorySprites[1], new Vector2(-1, -1), new Vector2(-1, -1), 1)
        { description = "Need electricity? Well this is your solution." });
        factoryTypes.Add(new Factory(factorySprites[2], new Vector2(0, 0), new Vector2(1, 0), 2)
        { description = "Used for transporting stuff around." });
        factoryTypes.Add(new Factory(factorySprites[3], new Vector2(0, 0), new Vector3(0, -1), 3));     
        factoryTypes.Add(new Factory(factorySprites[4], new Vector2(0, 1), new Vector2(4, 1), 4)
        { description = "Stores stuff. Yeah that's about it."});
        factoryTypes.Add(new Factory(factorySprites[5], new Vector2(0, 0), new Vector2(1, -1), 5));
    }
    private void Awake()
    {
        buildingParent = new GameObject("BuildingParent").transform;
    }
    void Update()
    { 
        if ((pauseMenu != null && pauseMenu.isPaused) || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        #region PlaceBuilding
        //Create/Destroy hologram
        if (currentMachineHologram == null && Input.GetMouseButtonDown(1) && selectedfactory != -1)
        {
            currentMachineHologram = new GameObject("machineHologram");
            currentMachineHologram.AddComponent<SpriteRenderer>();
            currentMachineHologram.GetComponent<SpriteRenderer>().sprite = factoryTypes[selectedfactory].Sprite;
            currentMachineHologram.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentMachineHologram != null)
            {
                Destroy(currentMachineHologram);
                currentMachineHologram = null;
            }
            selectedfactory = -1;
        }

        //Update position of hologram (malko tupa matematika, otne mi 3 chasa iskam da se samoubiq)      
        Vector2 positionOfMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionOfMouse.x = Mathf.Round(positionOfMouse.x); positionOfMouse.y = Mathf.Round(positionOfMouse.y);
        Vector2 scale = new Vector2();
        Vector2 offsetPositionOfMouse = new Vector2();
        try
        {
            scale = new Vector2(factoryTypes[selectedfactory].Sprite.texture.width / 32, factoryTypes[selectedfactory].Sprite.texture.height / 32);
            offsetPositionOfMouse = new Vector2(positionOfMouse.x + (scale.x / 2) - 0.5f, positionOfMouse.y + (scale.y / 2) - 0.5f);
        }
        catch
        {
            //No factory selected
        }

        if (currentMachineHologram != null)
            currentMachineHologram.transform.position = offsetPositionOfMouse;


        //Check if position is viable for placement and update hologram color
        if (currentMachineHologram != null && CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y))
            currentMachineHologram.GetComponent<SpriteRenderer>().color = Color.cyan;
        else if (currentMachineHologram != null && !CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y))
            currentMachineHologram.GetComponent<SpriteRenderer>().color = Color.red;

        //If position is possible, place building
        if (Input.GetMouseButtonDown(0) && currentMachineHologram != null && CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y))
        {
            GameObject building = new GameObject("building");
            building.transform.parent = buildingParent;
            building.AddComponent<SpriteRenderer>();
            building.GetComponent<SpriteRenderer>().sprite = factoryTypes[selectedfactory].Sprite;

            building.AddComponent<Structure>();
            building.GetComponent<Structure>().type = -1;
            building.GetComponent<Structure>().position = new Vector2(offsetPositionOfMouse.x, offsetPositionOfMouse.y);

            building.AddComponent<Machine>();
            building.GetComponent<Machine>().type = factoryTypes[selectedfactory].Type;

            building.GetComponent<Machine>().waterChange = factoryTypes[selectedfactory].waterChange;
            building.GetComponent<Machine>().fireChange = factoryTypes[selectedfactory].fireChange;
            building.GetComponent<Machine>().earthChange = factoryTypes[selectedfactory].earthChange;
            building.GetComponent<Machine>().airChange = factoryTypes[selectedfactory].airChange;
            switch (building.GetComponent<Machine>().type)
            {
                case 0:
                    building.GetComponent<Machine>().drillSpeed = 30;
                    building.GetComponent<Machine>().hasInput = false;
                    building.GetComponent<Machine>().hasOutput = true;
                    building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
                    break;
                case 1:
                    building.GetComponent<Machine>().coalConsumptionSpeed = 30;
                    break;
                case 2:
                    building.GetComponent<Machine>().conveyorSpeed = 4;
                    building.GetComponent<Machine>().hasInput = true;
                    building.GetComponent<Machine>().hasOutput = true;
                    building.GetComponent<Machine>().input = factoryTypes[selectedfactory].Input;
                    building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
                    building.GetComponent<Machine>().ChangeConveyorRotation(rotation);
                    rotation = 0;
                    break;
                case 3:
                    building.GetComponent<Machine>().smeltSpeed = 30;
                    building.GetComponent<Machine>().hasInput = true;
                    building.GetComponent<Machine>().hasOutput = true;
                    building.GetComponent<Machine>().canInputAnywhere = true;
                    building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
                    break;
                case 4:
                    building.GetComponent<Machine>().hasInput = true;
                    building.GetComponent<Machine>().hasOutput = true;
                    building.GetComponent<Machine>().input = factoryTypes[selectedfactory].Input;
                    building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
                    break;
                case 5:
                    building.GetComponent<Machine>().craftSpeed = 30;
                    building.GetComponent<Machine>().hasInput = true;
                    building.GetComponent<Machine>().hasOutput = true;
                    building.GetComponent<Machine>().canInputAnywhere = true;
                    building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
                    break;

            }
            building.GetComponent<Machine>().UpdateInventorySize();


            building.transform.position = offsetPositionOfMouse;

            UpdateStructureGrid(building, positionOfMouse, (int)scale.x, (int)scale.y);
            Destroy(currentMachineHologram);
            currentMachineHologram = null;
            genMap.UpdateSortingOrderForStructures();
        }

        //Rotate conveyor belts
        if (currentMachineHologram != null && Input.GetKeyDown(KeyCode.R) && selectedfactory == 2)
        {
            if (rotation == 3)
                rotation = 0;
            else
                rotation++;

            currentMachineHologram.transform.eulerAngles = new Vector3(0, 0, rotation * 90);
        }
        #endregion

        #region ChoppingTrees
        if (isChoppingTrees == true)
        {
            if (Input.GetMouseButton(0))
            {
                GameObject selectedTree = genMap.structureGrid[(int)Mathf.Round(positionOfMouse.x), (int)Mathf.Round(positionOfMouse.y)];
                if (selectedTree != null && selectedTree.GetComponent<Structure>().type == 0)
                {
                    Destroy(selectedTree);

                    Machine reactor = GameObject.Find("reactor").GetComponent<Machine>();
                    if (reactor.inventory.Count < reactor.inventorySize)
                        reactor.inventory.Push(new Item(3));
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                IsChoppingTrees();
        }
        #endregion

        #region Hovering
        if (objectHoveringOver != null)
        {
            Color color = objectHoveringOver.GetComponent<SpriteRenderer>().color;
            color.r = 1f;
            color.g = 1f;
            color.b = 1f;
            objectHoveringOver.GetComponent<SpriteRenderer>().color = color;
        }
        objectHoveringOver = genMap.structureGrid[(int)positionOfMouse.x, (int)positionOfMouse.y];
        if (objectHoveringOver == null)
            objectHoveringOver = genMap.grid[(int)positionOfMouse.x, (int)positionOfMouse.y];

        if(objectHoveringOver != null)
        {
            Color color = objectHoveringOver.GetComponent<SpriteRenderer>().color;
            color.r = 0.5f;
            color.g = 0.5f;
            color.b = 0.5f;
            objectHoveringOver.GetComponent<SpriteRenderer>().color = color;
        }
        #endregion
    }
    public void SelectSprite(int factoryToSelect)
    {
        if(currentMachineHologram == null)
        {
            selectedfactory = factoryToSelect;
            rotation = 0;
            isChoppingTrees = false;
        }
    }
    public void IsChoppingTrees()
    {
        isChoppingTrees = !isChoppingTrees;
        selectedfactory = -1;
        currentMachineHologram = null;
    }
    public bool CheckIfCanPlace(Vector2 position, int xSize, int ySize) //position vector2 is the bottom right of the gameObject
    {
        for(int i = 0; i < xSize; i++)
        {
            for(int j = 0; j < ySize; j++)
            {
                Vector2 tileToCheck = new Vector2(position.x + i, position.y + j);
                try
                {
                    if (genMap.grid[(int)tileToCheck.x, (int)tileToCheck.y].GetComponent<Tile>().type == 0)
                        return false;
                    if (genMap.structureGrid[(int)tileToCheck.x, (int)tileToCheck.y] != null)
                        return false;
                }
                catch
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void UpdateStructureGrid(GameObject gObj, Vector2 position, int xSize, int ySize)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                genMap.structureGrid[(int)position.x + i, (int)position.y + j] = gObj;
            }
        }
    }
}
