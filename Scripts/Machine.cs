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

    //Global energyConsumption for each machinbe
    public int energyConsumption;

    //Drill Parameters
    public int drillSpeed = -1;

    //Refinery Parameters
    public int refiningSpeed = -1;

    //Conveyor belt parameters
    public int conveyorSpeed;
    
    //Generator Parameters
    public int coalConsumptionSpeed = -1;
    public int energyGain = 0;
    public int maxEnergy = 300;
    public int energy = 0;

    //Input and Output for materials
    public Vector2 input;
    public Vector2 output; 

    private void Awake()
    {
        tickSystem = GameObject.Find("GlobalEvent").GetComponent<TickSystem>();
    }
    private void Update()
    {
        if (type == 0)
            Drill();
        else if (type == 1)
            Generator();
        else if(type == 3)
            Refine();
    }
    public void Drill()
    {
        if (tickActionFinished == 0 && drillSpeed != -1) tickActionFinished = tickSystem.tickTime + (ulong)drillSpeed;

        if (tickActionFinished <= tickSystem.tickTime)
        {
            Debug.Log("Drill Ticked");
            tickActionFinished = 0;
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
