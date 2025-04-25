using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Machine : MonoBehaviour
{
    //-1 - Reactor
    //0 - drill
    //1 - generator
    //2 - conveyor belt
    //3 - smeltery
    //4 - container
    //5 - crafter
    //6 - electrical pole
    public int type;

    //Tick System
    public ulong tickActionFinished = 0;
    TickSystem tickSystem;
    ItemManager itemManager;
    GenerateMap grids;

    //Global energyConsumption for each machine
    public float energyConsumption = -1;

    //Drill Parameters
    public int drillSpeed = -1;

    //Refinery Parameters
    public int refiningSpeed = -1;

    //Conveyor belt parameters
    public int conveyorSpeed;
    public GameObject objectOnTop;
    public int rotation; //Anti-Clockwise starting from the right in 90 degree intervals

    //Smelter parameters
    public int smeltSpeed = -1;

    //Crafter parameters
    public int craftSpeed = -1;

    //Generator Parameters
    public int coalConsumptionSpeed = -1;
    public float energyGain = 0;
    public float maxEnergy = 300f;
    public float energy = 0;
    public int range = 0;

    //Input and Output for materials
    public Vector2 input;
    public Vector2 output;

    public bool hasInput;
    public bool hasOutput;

    public bool canInputAnywhere;
    public bool canOutputAnywhere;

    //Machine inventory
    public Stack inventory;
    public int inventorySize = 0;
    public int[] ItemsAllowed;

    //Elemental Change
    public float waterChange;
    public float fireChange;
    public float earthChange;
    public float airChange;
    
    private void Awake()
    {
        inventory = new Stack();
        tickSystem = GameObject.Find("GlobalEvent").GetComponent<TickSystem>();
        itemManager = GameObject.Find("GlobalEvent").GetComponent<ItemManager>();
        grids = GameObject.Find("GlobalEvent").GetComponent<GenerateMap>();

        UpdateInventorySize();
    }
    public void UpdateInventorySize()
    {
        switch (type)
        {
            case -1:
                inventorySize = 30;
                break;
            case 0:
                inventorySize = 2;
                break;
            case 1:
                inventorySize = 5;
                ItemsAllowed = new int[] { 0, 8 };
                break;
            case 3:
                inventorySize = 2;
                ItemsAllowed = new int[] { 0, 1, 2, 3 };
                break;
            case 4:
                inventorySize = 250;
                break;
            case 5:
                inventorySize = 2;
                break;
        }
    }
    private void Update()
    {
        switch (type)
        {
            case 0:
                Drill();
                break;
            case 1:
                GenerateEnergy();
                break;
            case 2:
                ConveyorBeltMove();
                break;
            case 3:
                Smelt();
                if (hasInput == false)
                    ContainerOutput();
                break;
            case 4:
                ContainerOutput();
                break;
            case 5:
                Craft();
                if (hasInput == false)
                    ContainerOutput();
                break;
        }
    }
    public void Drill()
    {
        if (tickActionFinished == 0 && drillSpeed != -1) 
            tickActionFinished = tickSystem.tickTime + (ulong)drillSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            if(inventory.Count < inventorySize)
            {
                Tile tile = grids.grid[(int)transform.position.x, (int)transform.position.y].GetComponent<Tile>();

                switch(tile.type)
                {
                    case 2:
                        Item item = new Item(0);
                        inventory.Push(item);
                        break;
                    case 3:
                        item = new Item(1);
                        inventory.Push(item);
                        break;
                    case 4:
                        item = new Item(2);
                        inventory.Push(item);
                        break;
                }

            }
            tickActionFinished = 0;
        }

        //Checks if there is somewhere to output to
        Vector2 positionOfOutput = GetComponent<Structure>().originPosition + output;

        //First check if the machine even exists, second check if all other checks pass
        if (grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y] != null &&
            grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y].GetComponent<Machine>() != null)
        {
            GameObject objToOutput = grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y];

            if(objToOutput.GetComponent<Machine>().objectOnTop == null && inventory.Count > 0)
            {
                Item item = (Item)inventory.Pop();
                GameObject itemEntity = itemManager.CreateItemEntity(item, grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y]);
                objToOutput.GetComponent<Machine>().objectOnTop = itemEntity;
                itemEntity.transform.position = positionOfOutput;
            }
        }
    }
    public void ConveyorBeltMove()
    {
        //Check if theres an object on the belt
        if(objectOnTop != null)
        {
            if (tickActionFinished == 0 && conveyorSpeed != -1) 
                tickActionFinished = tickSystem.tickTime + (ulong)conveyorSpeed;

            //After ticking start moving object
            if (tickActionFinished <= tickSystem.tickTime)
            {
                Vector2 positionOfOutput = GetComponent<Structure>().originPosition + output;

                //Check if theres a machine at the coords
                if (grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y] != null &&
                    grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y].GetComponent<Machine>() != null)
                {
                    Machine objectToMoveTo = grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y].GetComponent<Machine>();
                    
                    //If the machine outputting to is a conveyor belt
                    if (objectToMoveTo.type == 2 && objectToMoveTo.objectOnTop == null)
                    {
                        objectToMoveTo.objectOnTop = objectOnTop;
                        objectOnTop.transform.position = positionOfOutput;
                        objectOnTop = null;
                        tickActionFinished = 0;
                    }

                    //If the machine outputting to is a container
                    else if(objectToMoveTo.type == 4 && objectToMoveTo.inventory.Count < objectToMoveTo.inventorySize
                        && GetComponent<Structure>().originPosition + output == objectToMoveTo.GetComponent<Structure>().originPosition + objectToMoveTo.GetComponent<Machine>().input)
                    {
                        if(objectToMoveTo.inventory.Count > 0 && objectToMoveTo.inventory.Count < objectToMoveTo.inventorySize)
                        {
                            Item containerItemType = (Item)objectToMoveTo.inventory.Peek();
                            if(containerItemType.type == objectOnTop.GetComponent<ItemEntity>().item.type)
                            {
                                objectToMoveTo.inventory.Push(objectOnTop.GetComponent<ItemEntity>().item);
                                Destroy(objectOnTop);
                            }
                        }
                        else if(objectToMoveTo.inventory.Count == 0)
                        {
                            objectToMoveTo.inventory.Push(objectOnTop.GetComponent<ItemEntity>().item);
                            Destroy(objectOnTop);
                        }
                        tickActionFinished = 0;
                    }

                    //if it's not
                    else if(objectToMoveTo.type != 2 && objectToMoveTo.hasInput == true 
                        && objectToMoveTo.inventory.Count < objectToMoveTo.inventorySize
                        && (objectToMoveTo.canInputAnywhere || objectToMoveTo.input + objectToMoveTo.gameObject.GetComponent<Structure>().originPosition == positionOfOutput))
                    {
                        if(objectToMoveTo.ItemsAllowed.Length == 0 || objectToMoveTo.ItemsAllowed.Contains(objectOnTop.GetComponent<ItemEntity>().item.type))
                        {
                            objectToMoveTo.inventory.Push(objectOnTop.GetComponent<ItemEntity>().item);
                            Destroy(objectOnTop);
                            objectOnTop = null;
                            tickActionFinished = 0;
                        }
                    }

                }
            }
        }
    }
    public void ContainerOutput()
    {
        if (tickActionFinished == 0)
            tickActionFinished = tickSystem.tickTime + 5;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            Vector2 positionOfOutput = GetComponent<Structure>().originPosition + output;
            GameObject outputObject = grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y];

            if(outputObject != null && outputObject.GetComponent<Machine>() != null
                && outputObject.GetComponent<Machine>().type == 2 && inventory.Count > 0
                && outputObject.GetComponent<Machine>().objectOnTop == null)
            {
                outputObject.GetComponent<Machine>().objectOnTop = itemManager.CreateItemEntity((Item)inventory.Pop(), outputObject);

                if (type == 3 || type == 5)
                    hasInput = true;
                tickActionFinished = 0;
            }
        }
    }
    public void ChangeConveyorRotation(int rotation)
    {
        this.rotation = rotation;
        transform.eulerAngles = new Vector3(0, 0, 90 * rotation);

        switch(rotation)
        {
            case 1:
                output = new Vector3(output.y, output.x);
                break;
            case 2:
                output = new Vector3(-output.x, output.y);
                break;
            case 3:
                output = new Vector3(output.y, -output.x);
                break;
        }
    }
    public void GenerateEnergy()
    {
        if (tickActionFinished == 0 && coalConsumptionSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)coalConsumptionSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {

            tickActionFinished = 0;
        }
    }
    public void Smelt()
    {
        if (tickActionFinished == 0 && smeltSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)smeltSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            if(inventory.Count == 2)
            {
                hasInput = false;
                Item fuel = (Item)inventory.Pop();
                Item material = (Item)inventory.Pop();
                if(material.type == 0)
                {
                    Item item = fuel;
                    fuel = material;
                    material = item;
                }
                if(fuel.type == 0)
                {
                    switch(material.type)
                    {
                        case 1:
                            inventory.Push(new Item(4));
                            break;
                        case 2:
                            inventory.Push(new Item(5));
                            break;
                    }
                }
                else
                {
                    inventory = new Stack();
                    hasInput = true;
                }
                tickActionFinished = 0;
            }
        }
    }
    public void Craft()
    {
        if (tickActionFinished == 0 && craftSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)craftSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            if(inventory.Count == 2)
            {
                Item firstItem = (Item)inventory.Pop();
                Item secondItem = (Item)inventory.Pop();

                //Recipes
                if((firstItem.type == 4 && secondItem.type == 3) || (firstItem.type == 3 && secondItem.type == 4))
                {
                    hasInput = false;
                    inventory.Push(new Item(6));
                }
                else if((firstItem.type == 6 && secondItem.type == 5) || (firstItem.type == 5 && secondItem.type == 6))
                {
                    hasInput = false;
                    inventory.Push(new Item(7));
                }
                tickActionFinished = 0;
            }
        }
    }
}
