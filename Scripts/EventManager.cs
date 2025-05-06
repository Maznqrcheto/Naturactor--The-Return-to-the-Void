using UnityEngine;

public class EventManager : EventManagerComponents
{  
    [Header("Events")]
    public Drought droughtEvent;
    public Flood floodEvent;
    public GenerateMap mapGenerator;
    public GameObject[,] grid;
    public override void CheckEvents(ulong tick) // tuk sa usloviqta za vseki edin event v igrata, a v samite scriptove NQMA usloviq za protichane, tam e samo kvo se sluchva
    {
        GetElementalProgress();

        if (fireLevel - waterLevel > 20f && !droughtEvent.droughtOccured && droughtEvent.droughtCooldown == 2400) // Drought Event
        {
            droughtEvent.mapGenerator = mapGenerator;
            droughtEvent.grid = mapGenerator.grid;
            droughtEvent.StartDrought();
        }
        if (waterLevel - fireLevel > 20f && !floodEvent.floodOccured && floodEvent.floodCooldown == 2400)
        {
            floodEvent.mapGenerator = mapGenerator;
            floodEvent.grid = mapGenerator.grid;
            floodEvent.StartFlood();
        }
    }
    void Start()
    {
        SetElementalProgress(100f);
    }
}
