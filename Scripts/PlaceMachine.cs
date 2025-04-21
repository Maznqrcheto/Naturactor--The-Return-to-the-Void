using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class PlaceMachine : MonoBehaviour
{
    public GenerateMap genMap;
    public PauseMenu pauseMenu;

    public GameObject currentMachineHologram;
    public Transform buildingParent;

    public List<Factory> factoryTypes;
    public List<Sprite> factorySprites;
    public int selectedfactory = 0;

    public int rotation = 0;
    private void Start()
    {
        factoryTypes = new List<Factory>();
        factoryTypes.Add(new Factory(factorySprites[0], new Vector2(-1, -1), new Vector2(1, -1), 0));
        factoryTypes.Add(new Factory(factorySprites[1], new Vector2(-1, -1), new Vector2(-1, -1), 1));
        factoryTypes.Add(new Factory(factorySprites[2], new Vector2(0 ,0), new Vector2(1, 0), 2));
        factoryTypes.Add(new Factory(factorySprites[3], new Vector2(-1, -1), new Vector2(-1, -1), 4));
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
        //Create/Destroy hologram
        if(currentMachineHologram == null && Input.GetMouseButtonDown(1))
        {
            currentMachineHologram = new GameObject("machineHologram");
            currentMachineHologram.AddComponent<SpriteRenderer>();
            currentMachineHologram.GetComponent<SpriteRenderer>().sprite = factoryTypes[selectedfactory].Sprite;
            currentMachineHologram.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else if(currentMachineHologram != null && Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(currentMachineHologram);
            currentMachineHologram = null;
        }

        //Update position of hologram (malko tupa matematika, otne mi 3 chasa iskam da se samoubiq)
        Vector2 positionOfMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionOfMouse.x = Mathf.Round(positionOfMouse.x); positionOfMouse.y = Mathf.Round(positionOfMouse.y);
        Vector2 scale = new Vector2(factoryTypes[selectedfactory].Sprite.texture.width / 32, factoryTypes[selectedfactory].Sprite.texture.height / 32);
        Vector2 offsetPositionOfMouse = new Vector2(positionOfMouse.x + (scale.x / 2) - 0.5f, positionOfMouse.y + (scale.y / 2) - 0.5f);
        if (currentMachineHologram != null)
            currentMachineHologram.transform.position = offsetPositionOfMouse;

        //Check if position is viable for placement and update hologram color
        if(currentMachineHologram != null && CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y))
            currentMachineHologram.GetComponent<SpriteRenderer>().color = Color.cyan;
        else if(currentMachineHologram != null && !CheckIfCanPlace(positionOfMouse, (int)scale.x, (int)scale.y))
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

            }
            

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
    }
    public void SelectSprite(int factoryToSelect)
    {
        if(currentMachineHologram == null)
        {
            selectedfactory = factoryToSelect;
            rotation = 0;
        }
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
