using System;
using System.Collections.Generic;
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
    //6 - refinery
    //7 - lumber camp
    public int type;

    [Header("Tick System")]
    public ulong tickActionFinished = 0;
    TickSystem tickSystem;
    ItemManager itemManager;
    GenerateMap grids;
    SoundFXManager soundFXManager;

    [Header("Consumption of energy")]
    public float energyConsumption = -1;
    public bool isSupplied;

    [Header("Drill Parameters")]
    public int drillSpeed = -1;

    [Header("Refinery Parameters")]
    public int refiningSpeed = -1;

    [Header("Conveyor Parameters")]
    public int conveyorSpeed;
    public GameObject objectOnTop;
    public int rotation; //Anti-Clockwise starting from the right in 90 degree intervals

    [Header("Smelter Parameters")]
    public int smeltSpeed = -1;

    [Header("Crafter Parameters")]
    public int craftSpeed = -1;

    [Header("Generator Parameters")]
    public int fuelConsumptionSpeed = -1;
    public float energyGain = 0;
    public float maxEnergy = 300f;
    public float energy = 0;
    public int generatorRange = 0;

    [Header("Lumber camp Parameters")]
    public int lumberCampRange = 0;
    public int lumberCampSpeed = 0;

    [Header("Input and output")]
    public Vector2 input;
    public Vector2 output;

    public bool hasInput;
    public bool hasOutput;

    public bool canInputAnywhere;
    public bool canOutputAnywhere;

    [Header("Inventory parameters")]
    public Stack inventory;
    public int inventorySize = 0;
    public int[] ItemsAllowed;

    [Header("Elemental change of machine")]
    public float waterChange = 0;
    public float fireChange = 0;
    public float earthChange = 0;
    public float airChange = 0;
    
    private void Awake()
    {
        ItemsAllowed = new int[0];
        inventory = new Stack();
        tickSystem = GameObject.Find("GlobalEvent").GetComponent<TickSystem>();
        itemManager = GameObject.Find("GlobalEvent").GetComponent<ItemManager>();
        grids = GameObject.Find("GlobalEvent").GetComponent<GenerateMap>();
        soundFXManager = GameObject.Find("GlobalEvent").GetComponent<SoundFXManager>();
        UpdateInventorySize();
    }
    public void UpdateInventorySize()
    {
        switch (type)
        {
            case -1:
                inventorySize = 50;
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
            case 6:
                inventorySize = 1;
                break;
            case 7:
                inventorySize = 10;
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
                    MachineOutput();
                break;
            case 4:
                ContainerOutput();
                break;
            case 5:
                if(isSupplied == true)
                {
                    Craft();
                    if (hasInput == false)
                        MachineOutput();
                }
                break;
            case 6:
                if(isSupplied == true)
                {
                    Refine();
                    if (hasInput == false)
                        MachineOutput();
                }
                break;
            case 7:
                LumberCamp();
                if (inventory.Count > 0)
                    MachineOutput();
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
            SoundFXManager.instance.PlaySoundFXClip(soundFXManager.clips[0], this.transform, 100);
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
            MachineOutput();
            tickActionFinished = 0;
        }
    }
    public void MachineOutput()
    {
        Vector2 positionOfOutput = GetComponent<Structure>().originPosition + output;
        GameObject outputObject = grids.structureGrid[(int)positionOfOutput.x, (int)positionOfOutput.y];

        if (outputObject != null && outputObject.GetComponent<Machine>() != null
            && outputObject.GetComponent<Machine>().type == 2 && inventory.Count > 0
            && outputObject.GetComponent<Machine>().objectOnTop == null)
        {
            outputObject.GetComponent<Machine>().objectOnTop = itemManager.CreateItemEntity((Item)inventory.Pop(), outputObject);

            if (type == 3 || type == 5 || type == 6)
                hasInput = true;
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
        if (tickActionFinished == 0 && fuelConsumptionSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)fuelConsumptionSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            energyGain = 0;
            //Add energy gain
            if(inventory.Count > 0)
            {
                Item fuel = (Item)inventory.Pop();
                if (fuel.type == 0)
                    energyGain += 30;
                else if (fuel.type == 8)
                    energyGain += 40;
            }
            //Remove energy gain

            //Gain every building in range
            List<GameObject> objectsInRange = new List<GameObject>();
            Vector2 pos = GetComponent<Structure>().position;
            for (int i = (int)pos.x - generatorRange; i < (int)pos.x + generatorRange; i++)
            {
                for (int j = (int)pos.y - generatorRange; j < (int)pos.y + generatorRange; j++)
                {
                    if (grids.structureGrid[i, j] != null &&
                        objectsInRange.Contains(grids.structureGrid[i, j]) == false 
                        && grids.structureGrid[i, j].GetComponent<Machine>() != null)
                    {
                        objectsInRange.Add(grids.structureGrid[i, j]);
                    }
                }
            }
            foreach (GameObject obj in objectsInRange)
            {
                if (obj.GetComponent<Machine>().energyConsumption != -1)
                    energyGain -= obj.GetComponent<Machine>().energyConsumption;
            }

            energy += energyGain;

            //Regulate Energy
            if (energy > maxEnergy)
            {
                energy = maxEnergy;
                foreach (GameObject obj in objectsInRange)
                {
                    obj.GetComponent<Machine>().isSupplied = true;
                }
            }
            else if (energy < 0)
            {
                energy = 0;
                foreach(GameObject obj in objectsInRange)
                {
                    obj.GetComponent<Machine>().isSupplied = false;
                }
            }
            else
            {
                foreach (GameObject obj in objectsInRange)
                {
                    obj.GetComponent<Machine>().isSupplied = true;
                }
            }

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
    public void Refine()
    {
        if (tickActionFinished == 0 && refiningSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)refiningSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            if (inventory.Count == 1 && hasInput)
            {
                Item item = (Item)inventory.Pop();
                switch (item.type)
                {
                    case 3:
                        inventory.Push(new Item(8));
                        hasInput = false;
                        break;
                }
                tickActionFinished = 0;
            }
        }
    }
    public void LumberCamp()
    {
        if (tickActionFinished == 0 && lumberCampSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)lumberCampSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            lumberCampSpeed = 40;
            int speedIncrease = 0;
            Vector2 pos = GetComponent<Structure>().position;
            for (int i = (int)pos.x - lumberCampRange; i < (int)pos.x + lumberCampRange; i++)
            {
                for (int j = (int)pos.y - lumberCampRange; j < (int)pos.y + lumberCampRange; j++)
                {
                    if (grids.structureGrid[i, j] != null 
                        && grids.structureGrid[i, j].GetComponent<Structure>().type == 0) {
                    
                        speedIncrease++;

                    }
                }
            }
            if (speedIncrease > 10)
                speedIncrease = 10;

            lumberCampSpeed -= speedIncrease * 2;
            
            if(lumberCampSpeed > 0 && inventory.Count < inventorySize)
            {
                inventory.Push(new Item(3));
            }

            tickActionFinished = 0;
        }
    }
}
