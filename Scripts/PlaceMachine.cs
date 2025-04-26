using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class PlaceMachine : MonoBehaviour
{
    public GenerateMap genMap;
    public PauseMenu pauseMenu;
    public InventoryManager inventoryManager;

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
        #region factorySettings
        factoryTypes = new List<Factory>();
        factoryTypes.Add(new Factory(factorySprites[0], new Vector2(-1, -1), new Vector2(1, -1), 0)
        {
            description = "Useful for mining materials.",
            fireChange = -0.2f,
            waterChange = 0.4f,
            earthChange = -0.2f
        });
        factoryTypes.Add(new Factory(factorySprites[1], new Vector2(-1, -1), new Vector2(-1, -1), 1)
        {
            description = "Need electricity? Well this is your solution.",
            airChange = -0.2f
        });
        factoryTypes.Add(new Factory(factorySprites[2], new Vector2(0, 0), new Vector2(1, 0), 2)
        { description = "Used for transporting stuff around." });
        factoryTypes.Add(new Factory(factorySprites[3], new Vector2(0, 0), new Vector3(0, -1), 3)
        {
            description = "You can smelt stuff here into more refined materials.",
            airChange = -0.2f,
            waterChange = -0.2f
        });     
        factoryTypes.Add(new Factory(factorySprites[4], new Vector2(0, 1), new Vector2(4, 1), 4)
        { description = "Stores stuff. Yeah that's about it."});
        factoryTypes.Add(new Factory(factorySprites[5], new Vector2(0, 0), new Vector2(1, -1), 5)
        {
            description = "Crafter for crafting 2 items into 1. Requires electricity to work.",
            energyConsumption = 15f,
            airChange = -0.5f,
            waterChange = -0.5f
        });
        factoryTypes.Add(new Factory(factorySprites[6], new Vector2(0, 0), new Vector2(1, -1), 6)
        {
            description = "Used to refine materials into a more pure form. Required electricity to work.",
            energyConsumption = 10f,
            earthChange = -0.2f,
            fireChange = 0.3f
        });
        factoryTypes.Add(new Factory(factorySprites[7], new Vector2(0, 0), new Vector2(1, -1), 7)
        {
            description = "Passive wood income. The limit to wood collection is your imagination.",
            earthChange = 0.1f,
            airChange = 0.1f
        });
        #endregion
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
        if (currentMachineHologram == null && selectedfactory != -1)
        {
            currentMachineHologram = new GameObject("machineHologram");
            currentMachineHologram.AddComponent<SpriteRenderer>();
            currentMachineHologram.GetComponent<SpriteRenderer>().sprite = factoryTypes[selectedfactory].Sprite;
            currentMachineHologram.GetComponent<SpriteRenderer>().sortingOrder = 1;
            currentMachineHologram.transform.eulerAngles = new Vector3(0, 0, rotation * 90);
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
        if (currentMachineHologram != null && CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y) && HasMaterialsToBuild(selectedfactory, false))
            currentMachineHologram.GetComponent<SpriteRenderer>().color = Color.cyan;
        else if (currentMachineHologram != null && (!CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y) || !HasMaterialsToBuild(selectedfactory, false)))
            currentMachineHologram.GetComponent<SpriteRenderer>().color = Color.red;

        //If position is possible, place building
        if (Input.GetMouseButton(0) && currentMachineHologram != null
            && selectedfactory == 2 && CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y)
            && HasMaterialsToBuild(selectedfactory, true))
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

            building.GetComponent<Machine>().conveyorSpeed = 4;
            building.GetComponent<Machine>().hasInput = true;
            building.GetComponent<Machine>().hasOutput = true;
            building.GetComponent<Machine>().input = factoryTypes[selectedfactory].Input;
            building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
            building.GetComponent<Machine>().ChangeConveyorRotation(rotation);

            building.GetComponent<Machine>().UpdateInventorySize();
            building.transform.position = offsetPositionOfMouse;

            UpdateStructureGrid(building, positionOfMouse, (int)scale.x, (int)scale.y);
            genMap.UpdateSortingOrderForStructures();
        }
        else if (Input.GetMouseButtonDown(0) && currentMachineHologram != null 
            && CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y) 
            && HasMaterialsToBuild(selectedfactory, true))
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

            building.GetComponent<Machine>().energyConsumption = factoryTypes[selectedfactory].energyConsumption;
            switch (building.GetComponent<Machine>().type)
            {
                case 0:
                    building.GetComponent<Machine>().drillSpeed = 30;
                    building.GetComponent<Machine>().hasInput = false;
                    building.GetComponent<Machine>().hasOutput = true;
                    building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
                    break;
                case 1:
                    building.GetComponent<Machine>().fuelConsumptionSpeed = 30;
                    building.GetComponent<Machine>().generatorRange = 5;
                    building.GetComponent<Machine>().hasInput = true;
                    building.GetComponent<Machine>().canInputAnywhere = true;
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
                case 6:
                    building.GetComponent<Machine>().refiningSpeed = 30;
                    building.GetComponent<Machine>().hasInput = true;
                    building.GetComponent<Machine>().hasOutput = true;
                    building.GetComponent<Machine>().canInputAnywhere = true;
                    building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
                    break;
                case 7:
                    building.GetComponent<Machine>().lumberCampSpeed = 40;
                    building.GetComponent<Machine>().lumberCampRange = 4;
                    building.GetComponent<Machine>().hasInput = false;
                    building.GetComponent<Machine>().hasOutput = true;
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
        else if(currentMachineHologram != null)
        {
            selectedfactory = factoryToSelect;
            Destroy(currentMachineHologram);
            currentMachineHologram = null;
            rotation = 0;
        }
    }
    public void IsChoppingTrees()
    {
        isChoppingTrees = !isChoppingTrees;
        selectedfactory = -1;
        Destroy(currentMachineHologram);
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
    public bool HasMaterialsToBuild(int factoryType, bool removeMaterials)
    {
        switch (factoryType)
        {
            case 0:
                if(inventoryManager.woodCount >= 10)
                {
                    if(removeMaterials) inventoryManager.RemoveItem(new int[] { 3 }, new int[] { 10 });
                    return true;
                }
                break;
            default:
                return true;
        }
        return false;
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
