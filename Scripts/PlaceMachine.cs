using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
public class PlaceMachine : MonoBehaviour
{
    public GenerateMap genMap;
    public PauseMenu pauseMenu;
    public InventoryManager inventoryManager;
    public EventManager eventManager;

    public Transform buildingParent;

    public List<Factory> factoryTypes;
    public List<Sprite> factorySprites;
    public int selectedfactory = -1;

    public bool isChoppingTrees = false;
    public GameObject objectHoveringOver;

    public int rotation = 0;
    void Awake() 
    {
        buildingParent = new GameObject("BuildingParent").transform;

        CreateFactoryTypes();
    }
    void CreateFactoryTypes()
    {
        factoryTypes = new List<Factory>();
        factoryTypes.Add(new Factory(factorySprites[0], new Vector2(-1, -1), new Vector2(1, -1), 0)
        {
            description = "Useful for mining materials. \nCosts 10 wood to build. Outputs from the middle of the bottom.",
            fireChange = -0.2f,
            waterChange = 0.4f,
            earthChange = -0.2f
        });
        factoryTypes.Add(new Factory(factorySprites[1], new Vector2(-1, -1), new Vector2(-1, -1), 1)
        {
            description = "Need electricity? Well this is your solution. \nCosts 10 iron and copper ore. Needs fuel to function",
            airChange = -0.2f
        });
        factoryTypes.Add(new Factory(factorySprites[2], new Vector2(0, 0), new Vector2(1, 0), 2)
        { description = "Used for transporting stuff around. \nCosts 2 wood. Rotate with R" });
        factoryTypes.Add(new Factory(factorySprites[3], new Vector2(0, 0), new Vector3(0, -1), 3)
        {
            description = "You can smelt stuff here into more refined materials. \nCosts 5 iron and copper ore. Outputs from the bottom. Needs coal to function.",
            airChange = -0.2f,
            waterChange = -0.2f
        });
        factoryTypes.Add(new Factory(factorySprites[4], new Vector2(0, 1), new Vector2(4, 1), 4)
        { description = "Stores stuff. Yeah that's about it. \nCosts 20 wood. Outputs from the middle of the right side." });
        factoryTypes.Add(new Factory(factorySprites[5], new Vector2(0, 0), new Vector2(1, -1), 5)
        {
            description = "Crafter for crafting 2 items into 1. \nCosts 10 iron and copper bars. Requires electricity to work.",
            energyConsumption = 15f,
            airChange = -0.5f,
            waterChange = -0.5f
        });
        factoryTypes.Add(new Factory(factorySprites[6], new Vector2(0, 0), new Vector2(1, -1), 6)
        {
            description = "Used to refine materials into a more pure form. Needs 20 tools to be built. Requires electricity to work.",
            energyConsumption = 10f,
            earthChange = -0.2f,
            fireChange = 0.3f
        });
        factoryTypes.Add(new Factory(factorySprites[7], new Vector2(0, 0), new Vector2(1, -1), 7)
        {
            description = "Passive wood income. The limit to wood collection is your imagination. Costs 15 Advanced tools. Trees nearby increase production rate.",
            earthChange = 0.1f,
            airChange = 0.1f
        });
    }
    void Update()
    {
        if ((pauseMenu != null && pauseMenu.isPaused) || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //Update Mouse positon and scale of current factory
        Vector2[] mouseVars = UpdateMousePositionProperties();
        Vector2 positionOfMouse = mouseVars[0];
        Vector2 scale = mouseVars[1];
        Vector2 offsetPositionOfMouse = mouseVars[2];

        //Building event and rotating conveyor belt
        PlaceBuildingEvent(positionOfMouse, scale, offsetPositionOfMouse);

        if (currentMachineHologram != null && Input.GetKeyDown(KeyCode.R) && selectedfactory == 2)
            RotateConveyorBelt();

        //Tree chopping
        if (isChoppingTrees == true && Input.GetMouseButton(0)) ChopTree(positionOfMouse);
        if (isChoppingTrees == true && Input.GetKeyDown(KeyCode.Escape)) IsChoppingTrees();

        //Hover over object
        MouseHoveringOverObject(positionOfMouse);
    }
    Vector2[] UpdateMousePositionProperties()
    {
        Vector2 positionOfMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionOfMouse.x = Mathf.Round(positionOfMouse.x); positionOfMouse.y = Mathf.Round(positionOfMouse.y);

        Vector2 scale = new Vector2();
        Vector2 offsetPositionOfMouse = new Vector2();

        try
        {
            scale = new Vector2(factoryTypes[selectedfactory].Sprite.texture.width / 32, factoryTypes[selectedfactory].Sprite.texture.height / 32);
            offsetPositionOfMouse = new Vector2(positionOfMouse.x + (scale.x / 2) - 0.5f, positionOfMouse.y + (scale.y / 2) - 0.5f);
        }
        catch { }//No factory selected

        return new Vector2[] {positionOfMouse, scale, offsetPositionOfMouse};
    }
    GameObject currentMachineHologram;
    void PlaceBuildingEvent(Vector2 positionOfMouse, Vector2 scale, Vector2 offsetPositionOfMouse)
    {
        //Create/Destroy hologram
        MachineHologramAction(offsetPositionOfMouse);

        //Check if position is viable for placement and update hologram color
        bool canPlace = MachineHologramCheckIfCanPlace(positionOfMouse, scale);

        //If position is possible, place building
        if (Input.GetMouseButtonDown(0) && canPlace == true) // Placing building succesful
        {
            GameObject building = PlaceBuilding(offsetPositionOfMouse);
            UpdateStructureGrid(building, positionOfMouse, (int)scale.x, (int)scale.y);
            genMap.UpdateSortingOrderForStructures();

            SoundFXManager.instance.PlaySoundFXClip(GetComponent<SoundFXManager>().clips[3], transform, 100);
        }
        else if (Input.GetMouseButtonDown(0) && currentMachineHologram != null) // Placing building unsuccesful
            SoundFXManager.instance.PlaySoundFXClip(GetComponent<SoundFXManager>().clips[2], transform, 100);
    }
    void MachineHologramAction(Vector2 offsetPositionOfMouse)
    {
        //Create hologram
        if (currentMachineHologram == null && selectedfactory != -1)
        {
            currentMachineHologram = new GameObject("machineHologram");
            currentMachineHologram.AddComponent<SpriteRenderer>();
            currentMachineHologram.GetComponent<SpriteRenderer>().sprite = factoryTypes[selectedfactory].Sprite;
            currentMachineHologram.GetComponent<SpriteRenderer>().sortingOrder = 1;
            currentMachineHologram.transform.eulerAngles = new Vector3(0, 0, rotation * 90);
        }

        //Remove hologram
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentMachineHologram != null)
            {
                Destroy(currentMachineHologram);
                currentMachineHologram = null;
            }
            selectedfactory = -1;
        }

        //Update Hologram position
        if (currentMachineHologram != null)
            currentMachineHologram.transform.position = offsetPositionOfMouse;
    }
    bool MachineHologramCheckIfCanPlace(Vector2 positionOfMouse, Vector2 scale)
    {
        if (currentMachineHologram == null)
            return false;
        else if (currentMachineHologram != null && CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y) && HasMaterialsToBuild(selectedfactory, false))
        {
            currentMachineHologram.GetComponent<SpriteRenderer>().color = Color.cyan;
            return true;
        }
        else if (currentMachineHologram != null && (!CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y) || !HasMaterialsToBuild(selectedfactory, false)))
        {
            currentMachineHologram.GetComponent<SpriteRenderer>().color = Color.red;
            return false;
        }
        return false;
    }
    GameObject PlaceBuilding(Vector2 offsetPositionOfMouse)
    {
        GameObject building = new GameObject("building");
        building.transform.parent = buildingParent;

        //Set starting properties for the building
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
        
        //Update building Properties for specific tyoe
        UpdateBuildingProperties(offsetPositionOfMouse, building);

        //Set Building Position
        building.GetComponent<Machine>().UpdateInventorySize();
        building.transform.position = offsetPositionOfMouse;
        
        //Destroy hologram
        Destroy(currentMachineHologram);
        currentMachineHologram = null;

        return building;
    }
    void UpdateBuildingProperties(Vector2 offsetPositionOfMouse, GameObject building)
    {
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
            case 2:
                building.GetComponent<Machine>().conveyorSpeed = 4;
                building.GetComponent<Machine>().hasInput = true;
                building.GetComponent<Machine>().hasOutput = true;
                building.GetComponent<Machine>().input = factoryTypes[selectedfactory].Input;
                building.GetComponent<Machine>().output = factoryTypes[selectedfactory].Output;
                building.GetComponent<Machine>().ChangeConveyorRotation(rotation);
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
                building.GetComponent<Machine>().canInputAnywhere = true;
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
    }
    void RotateConveyorBelt()
    {

        if (rotation == 3)
            rotation = 0;
        else
            rotation++;

        currentMachineHologram.transform.eulerAngles = new Vector3(0, 0, rotation * 90);

    }

    void ChopTree(Vector2 positionOfMouse)
    {
        GameObject selectedTree = genMap.structureGrid[(int)Mathf.Round(positionOfMouse.x), (int)Mathf.Round(positionOfMouse.y)];
        if (selectedTree != null && selectedTree.GetComponent<Structure>().type == 0)
        {
            Destroy(selectedTree);

            Machine reactor = GameObject.Find("reactor").GetComponent<Machine>();
            if (reactor.inventory.Count < reactor.inventorySize)
                reactor.inventory.Push(new Item(3));
            SoundFXManager.instance.PlaySoundFXClip(GetComponent<SoundFXManager>().clips[1], transform, 100);

            eventManager.fireLevel += 0.1f;
            eventManager.earthLevel -= 0.1f;
        }
    }

    void MouseHoveringOverObject(Vector2 positionOfMouse)
    {
        if (objectHoveringOver != null)
        {
            Color color = objectHoveringOver.GetComponent<SpriteRenderer>().color;
            color.r = 1f;
            color.g = 1f;
            color.b = 1f;
            objectHoveringOver.GetComponent<SpriteRenderer>().color = color;
        }

        try
        {
            objectHoveringOver = genMap.structureGrid[(int)positionOfMouse.x, (int)positionOfMouse.y];
        }
        catch {}//mouse out of bounds

        if (objectHoveringOver == null)
            objectHoveringOver = genMap.grid[(int)positionOfMouse.x, (int)positionOfMouse.y];

        if (objectHoveringOver != null)
        {
            Color color = objectHoveringOver.GetComponent<SpriteRenderer>().color;
            color.r = 0.5f;
            color.g = 0.5f;
            color.b = 0.5f;
            objectHoveringOver.GetComponent<SpriteRenderer>().color = color;
        }
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
            case 1:
                if(inventoryManager.ironCount >= 10 && inventoryManager.copperCount >= 10)
                {
                    if (removeMaterials) inventoryManager.RemoveItem(new int[] { 1, 2 }, new int[] { 10, 10 });
                    return true;
                }
                break;
            case 2:
                if (inventoryManager.woodCount >= 2)
                {
                    if (removeMaterials) inventoryManager.RemoveItem(new int[] { 3 }, new int[] { 2 });
                    return true;
                }
                break;
            case 3:
                if (inventoryManager.ironCount >= 5 && inventoryManager.copperCount >= 5)
                {
                    if (removeMaterials) inventoryManager.RemoveItem(new int[] { 1, 2 }, new int[] { 5, 5 });
                    return true;
                }
                break;
            case 4:
                if (inventoryManager.woodCount >= 20)
                {
                    if (removeMaterials) inventoryManager.RemoveItem(new int[] { 3 }, new int[] { 20 });
                    return true;
                }
                break;
            case 5:
                if (inventoryManager.ironBarCount >= 10 && inventoryManager.copperBarCount >= 10)
                {
                    if (removeMaterials) inventoryManager.RemoveItem(new int[] { 4, 5 }, new int[] { 10, 10 });
                    return true;
                }
                break;
            case 6:
                if (inventoryManager.toolsCount >= 20)
                {
                    if (removeMaterials) inventoryManager.RemoveItem(new int[] { 6 }, new int[] { 20 });
                    return true;
                }
                break;
            case 7:
                if (inventoryManager.advToolsCount >= 15)
                {
                    if (removeMaterials) inventoryManager.RemoveItem(new int[] { 7 }, new int[] { 15 });
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
