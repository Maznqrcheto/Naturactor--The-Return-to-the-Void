using System;
using System.Collections;
using UnityEngine;

public class Machine : MonoBehaviour
{
    //0 - drill
    //1 - generator
    //2 - conveyor belt
    //3 - refinery
    //4 - container
    //5 - combiner
    //6 - electrical pole
    public int type;

    //Tick System
    public ulong tickActionFinished = 0;
    TickSystem tickSystem;
    ItemManager itemManager;
    GenerateMap grids;

    //Global energyConsumption for each machinbe
    public int energyConsumption;

    //Drill Parameters
    public int drillSpeed = -1;

    //Refinery Parameters
    public int refiningSpeed = -1;

    //Conveyor belt parameters
    public int conveyorSpeed;
    public GameObject objectOnTop;
    public int rotation; //Anti-Clockwise starting from the right in 90 degree intervals

    //Generator Parameters
    public int coalConsumptionSpeed = -1;
    public int energyGain = 0;
    public int maxEnergy = 300;
    public int energy = 0;

    //Input and Output for materials
    public Vector2 input;
    public Vector2 output;

    public bool hasInput;
    public bool hasOutput;

    //Machine inventory
    public Stack inventory;
    public int inventorySize = 0;
    private void Awake()
    {
        inventory = new Stack();
        tickSystem = GameObject.Find("GlobalEvent").GetComponent<TickSystem>();
        itemManager = GameObject.Find("GlobalEvent").GetComponent<ItemManager>();
        grids = GameObject.Find("GlobalEvent").GetComponent<GenerateMap>();

        switch (type)
        {
            case 0:
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
                Generator();
                break;
            case 2:
                ConveyorBeltMove();
                break;
            case 3:
                Refine();
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

                    //if it's not
                    else if(objectToMoveTo.type != 2 && objectToMoveTo.hasInput == true 
                        && objectToMoveTo.inventory.Count < objectToMoveTo.inventorySize
                        && objectToMoveTo.input + objectToMoveTo.gameObject.GetComponent<Structure>().originPosition == positionOfOutput)
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
    public void Generator()
    {
        if (tickActionFinished == 0 && coalConsumptionSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)coalConsumptionSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            Debug.Log("Generator Ticked");
            tickActionFinished = 0;
        }
    }
    public void Refine()
    {
        if (tickActionFinished == 0 && refiningSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)refiningSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            Debug.Log("Refinery Ticked");
            tickActionFinished = 0;
        }
    }
}
